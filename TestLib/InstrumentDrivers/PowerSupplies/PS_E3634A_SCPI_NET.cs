using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Multifunction;
using Agilent.CommandExpert.ScpiNet.AgE363x_1_7;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class PS_E3634A_SCPI_NET : AgE363x, IInstrument, IPowerSupplyOutputs1, IDiagnostics {
        public enum RANGE { P25V, P50V }

        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 result;
            try {
                SCPI.TST.Query(out result);
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)result; // AgE363x returns 0 for passed, 1 for fail.
        }

        public void OutputsOff() { SCPI.OUTPut.STATe.Command(Convert.ToBoolean(STATES.off)); }

        public RANGE RangeGet() {
            SCPI.SOURce.VOLTage.RANGe.Query(out String range);
            return (RANGE)Enum.Parse(typeof(RANGE), range);
        }
        public void RangeSet(RANGE Range) { SCPI.SOURce.VOLTage.RANGe.Command($"{Range}"); }

        public (Double AmpsDC, Double VoltsDC) Get(DC DC) {
            SCPI.MEASure.CURRent.DC.Query(out Double AmpsDC);
            SCPI.MEASure.VOLTage.DC.Query(out Double VoltsDC);
            return (AmpsDC, VoltsDC);
        }

        public void SetOffOn(Double VoltsDC, Double AmpsDC, Double OVP) {
            SCPI.OUTPut.STATe.Command(false);
            SCPI.SOURce.VOLTage.PROTection.CLEar.Command();
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{MMD.MAXimum}");
            SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command($"{VoltsDC}");
            SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command($"{AmpsDC}");
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{OVP}");
            SCPI.OUTPut.STATe.Command(true);
            Thread.Sleep(500); // Allow some time for voltage to stabilize.
        }

        public STATES StateGet() {
            SCPI.OUTPut.STATe.Query(out Boolean state);
            return state ? STATES.ON : STATES.off;
        }

        public void StateSet(STATES State) {
            SCPI.OUTPut.STATe.Command(State == STATES.ON);
            Thread.Sleep(500); // Allow some time for voltage to stabilize.        
        }

        #region Diagnostics
        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_E3634A = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                Configuration.Parameter parameter = Parameters.Find(p => p.Name == "Accuracy_E3634A_VDC") ?? new Configuration.Parameter { Name = "Accuracy_E3634A_VDC", Value = "0.1" };
                Double limit = Convert.ToDouble(parameter.Value);

                MSMU_34980A_SCPI_NET MSMU = ((MSMU_34980A_SCPI_NET)(TestLib.InstrumentDrivers["MSMU1_34980A"]));

                String message =
                    $"Please connect BMC6030-5 from {Detail}/{Address}{Environment.NewLine}{Environment.NewLine}" +
                    $"to {MSMU.Detail}/{MSMU.Address}.{Environment.NewLine}{Environment.NewLine}" +
                    "Click Cancel if desired.";
                if (DialogResult.OK == MessageBox.Show(message, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                    MSMU.SCPI.INSTrument.DMM.STATe.Command(true);
                    MSMU.SCPI.INSTrument.DMM.CONNect.Command();
                    SCPI.OUTPut.STATe.Command(false);
                    SCPI.SOURce.VOLTage.PROTection.STATe.Command(false);
                    SCPI.SOURce.CURRent.PROTection.STATe.Command(false);
                    SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("MINimum");
                    SCPI.SOURce.VOLTage.LEVel.IMMediate.STEP.INCRement.Command(1D);
                    SCPI.OUTPut.STATe.Command(true);

                    Boolean passed_E3634A = true, passed_VDC;
                    for (Int32 vdcApplied = 0; vdcApplied < 50; vdcApplied++) {
                        Thread.Sleep(millisecondsTimeout: 500);
                        MSMU.SCPI.MEASure.SCALar.VOLTage.DC.Query("AUTO", $"{MMD.MAXimum}", ch_list: null, out Double[] vdcMeasured);
                        passed_VDC = Math.Abs(vdcMeasured[0] - vdcApplied) <= limit;
                        passed_E3634A &= passed_VDC;
                        result_E3634A.Details.Add(new DiagnosticsResult(Label: "OUTput: ", Message: $"Applied {vdcApplied}VDC, measured {Math.Round(vdcMeasured[0], 3, MidpointRounding.ToEven)}VDC", Event: (passed_VDC ? EVENTS.PASS : EVENTS.FAIL)));
                        SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("UP");
                    }
                    result_E3634A.Summary &= passed_E3634A;
                    message =
                        $"Please disconnect BMC6030-5 from {Detail}/{Address}{Environment.NewLine}{Environment.NewLine}" +
                        $"and {MSMU.Detail}/{MSMU.Address}.{Environment.NewLine}{Environment.NewLine}";
                    MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            return result_E3634A;
        }
        #endregion Diagnostics

        public PS_E3634A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.POWER_SUPPLY;
        }
    }
}