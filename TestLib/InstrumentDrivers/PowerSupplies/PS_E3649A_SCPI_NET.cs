using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Multifunction;
using Agilent.CommandExpert.ScpiNet.AgE364xD_1_7;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class PS_E3649A_SCPI_NET : AgE364xD, IInstrument, IPowerSupplyE3649A, IDiagnostics {
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
            } catch (Exception e) {
                Instruments.SelfTestFailure(this, e);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)result; // AgE363x returns 0 for passed, 1 for fail.
        }

        public void OutputsOff() {
            // NOTE: Most multi-output supplies like the E3649A permit individual control of outputs,
            // but the E3649A does not; all supplies are set to the same STATE, off or ON.
            SCPI.OUTPut.STATe.Command(Convert.ToBoolean(STATES.off));
        }

        public OUTPUTS2 Selected() {
            SCPI.INSTrument.SELect.Query(out String select);
            return select == "OUTP1" ? OUTPUTS2.OUTput1 : OUTPUTS2.OUTput2;
        }

        public void Select(OUTPUTS2 Output) { SCPI.INSTrument.SELect.Command($"{Output}"); }

        public (Double AmpsDC, Double VoltsDC) Get(OUTPUTS2 Output, DC DC) {
            Select(Output);
            SCPI.MEASure.SCALar.CURRent.DC.Query(out Double AmpsDC);
            SCPI.MEASure.SCALar.VOLTage.DC.Query(out Double VoltsDC);
            return (AmpsDC, VoltsDC);
        }

        public void SetOffOn(OUTPUTS2 Output, Double Volts, Double Amps, Double OVP, STATES State) {
            Select(Output);
            SCPI.OUTPut.STATe.Command(false);
            SCPI.SOURce.VOLTage.PROTection.CLEar.Command();
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{MMD.MAXimum}");
            SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command($"{Volts}");
            SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command($"{Amps}");
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{OVP}");
            SCPI.OUTPut.STATe.Command(State == STATES.ON);
        }

        public STATES StateGet(OUTPUTS2 Output) {
            Select(Output);
            SCPI.OUTPut.STATe.Query(out Boolean state);
            return state ? STATES.ON : STATES.off;
        }

        public void StateSet(STATES State) {
            // NOTE: Most multi-output supplies like the E3649A permit individual control of outputs,
            // but the E3649A does not; all supplies are set to the same STATE, off or ON.
            SCPI.OUTPut.STATe.Command(State == STATES.ON);
        }

        #region Diagnostics
        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_E3649A = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                Configuration.Parameter parameter = Parameters.Find(p => p.Name == "Accuracy_E3649A_VDC") ?? new Configuration.Parameter { Name = "Accuracy_E3649A_VDC", Value = "0.1" };
                Double limit = Convert.ToDouble(parameter.Value);

                MSMU_34980A_SCPI_NET MSMU = ((MSMU_34980A_SCPI_NET)(Data.InstrumentDrivers["MSMU1_34980A"]));

                String message =
                    $"Please connect BMC6030-5 from {Detail}/{Address} Output 1{Environment.NewLine}{Environment.NewLine}" +
                    $"to {MSMU.Detail}/{MSMU.Address}.{Environment.NewLine}{Environment.NewLine}" +
                    "Click Cancel if desired.";
                if (DialogResult.OK == MessageBox.Show(message, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                    MSMU.SCPI.INSTrument.DMM.STATe.Command(true);
                    MSMU.SCPI.INSTrument.DMM.CONNect.Command();
                    TestOutput(OUTPUTS2.OUTput1, ref MSMU, limit, ref result_E3649A);
                    MessageBox.Show("Please connect BMC6030-5 to Output 2.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    TestOutput(OUTPUTS2.OUTput2, ref MSMU, limit, ref result_E3649A);
                    message =
                        $"Please disconnect BMC6030-5 from {Detail}/{Address}{Environment.NewLine}{Environment.NewLine}" +
                        $"and {MSMU.Detail}/{MSMU.Address}.{Environment.NewLine}{Environment.NewLine}";
                    MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            return result_E3649A;
        }

        private void TestOutput(OUTPUTS2 outPut, ref MSMU_34980A_SCPI_NET MSMU, Double limit, ref (Boolean Summary, List<DiagnosticsResult> Details) result_E3649A) {
            Select(outPut);
            SCPI.OUTPut.STATe.Command(false);
            SCPI.SOURce.VOLTage.PROTection.STATe.Command(false);
            SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("MINimum");
            SCPI.SOURce.VOLTage.LEVel.IMMediate.STEP.INCRement.Command(1D);
            SCPI.OUTPut.STATe.Command(true);

            Boolean passed_E3649A = true, passed_VDC;
            for (Int32 vdcApplied = 0; vdcApplied < 60; vdcApplied++) {
                System.Threading.Thread.Sleep(millisecondsTimeout: 500);
                MSMU.SCPI.MEASure.SCALar.VOLTage.DC.Query("AUTO", $"{MMD.MAXimum}", ch_list: null, out Double[] vdcMeasured);
                passed_VDC = Math.Abs(vdcMeasured[0] - vdcApplied) <= limit;
                passed_E3649A &= passed_VDC;
                result_E3649A.Details.Add(new DiagnosticsResult(Label: $"{outPut} :", Message: $"Applied {vdcApplied}VDC, measured {Math.Round(vdcMeasured[0], 3, MidpointRounding.ToEven)}VDC", Event: (passed_VDC ? EVENTS.PASS : EVENTS.FAIL)));
                SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("UP");
            }
            SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("MINimum");
            SCPI.OUTPut.STATe.Command(false);
            result_E3649A.Summary &= passed_E3649A;
        }
        #endregion Diagnostics

        public PS_E3649A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.POWER_SUPPLY;
        }
    }
}