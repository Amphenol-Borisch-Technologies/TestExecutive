using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Multifunction;
using Agilent.CommandExpert.ScpiNet.AgE364xD_1_7;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class PS_E3649A_SCPI_NET : IInstrument, IPowerSupplyE3649A, IDiagnostics {
        public String Address { get; }
        public String Detail { get; }
        public AgE364xD AgE364xD { get; }
        public INSTRUMENT_TYPE InstrumentType { get; }

        public void ResetClear() {
            AgE364xD.SCPI.RST.Command();
            AgE364xD.SCPI.CLS.Command();
        }

        public SELF_TEST_RESULT SelfTests() {
            Int32 result;
            try {
                AgE364xD.SCPI.TST.Query(out result);
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULT.FAIL;
            }
            return (SELF_TEST_RESULT)result; // AgE363x returns 0 for passed, 1 for fail.
        }

        public void OutputsOff() {
            // NOTE: Most multi-output supplies like the E3649A permit individual control of outputs,
            // but the E3649A does not; all supplies are set to the same STATE, off or ON.
            AgE364xD.SCPI.OUTPut.STATe.Command(Convert.ToBoolean(STATE.off));
        }

        public OUTPUT2 Selected() {
            AgE364xD.SCPI.INSTrument.SELect.Query(out String select);
            return select == "OUTP1" ? OUTPUT2.OUTput1 : OUTPUT2.OUTput2;
        }

        public void Select(OUTPUT2 Output) { AgE364xD.SCPI.INSTrument.SELect.Command($"{Output}"); }

        public (Double AmpsDC, Double VoltsDC) Get(OUTPUT2 Output, DC DC) {
            Select(Output);
            AgE364xD.SCPI.MEASure.SCALar.CURRent.DC.Query(out Double AmpsDC);
            AgE364xD.SCPI.MEASure.SCALar.VOLTage.DC.Query(out Double VoltsDC);
            return (AmpsDC, VoltsDC);
        }

        public void SetOffOn(OUTPUT2 Output, Double VoltsDC, Double AmpsDC, Double OVP) {
            Select(Output);
            AgE364xD.SCPI.OUTPut.STATe.Command(false);
            AgE364xD.SCPI.SOURce.VOLTage.PROTection.CLEar.Command();
            AgE364xD.SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{MMD.MAXimum}");
            AgE364xD.SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command($"{VoltsDC}");
            AgE364xD.SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command($"{AmpsDC}");
            AgE364xD.SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{OVP}");
            AgE364xD.SCPI.OUTPut.STATe.Command(true);
            Thread.Sleep(500); // Allow some time for voltage to stabilize.
        }

        public STATE StateGet(OUTPUT2 Output) {
            Select(Output);
            AgE364xD.SCPI.OUTPut.STATe.Query(out Boolean state);
            return state ? STATE.ON : STATE.off;
        }

        public void StateSet(STATE State) {
            // NOTE: Most multi-output supplies like the E3649A permit individual control of outputs,
            // but the E3649A does not; all supplies are set to the same STATE, off or ON.
            AgE364xD.SCPI.OUTPut.STATe.Command(State == STATE.ON);
            Thread.Sleep(500); // Allow some time for voltage to stabilize.
        }

        #region Diagnostics
        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULT.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_E3649A = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                Configuration.Parameter parameter = Parameters.Find(p => p.Name == "Accuracy_E3649A_VDC") ?? new Configuration.Parameter { Name = "Accuracy_E3649A_VDC", Value = "0.1" };
                Double limit = Convert.ToDouble(parameter.Value);

                MSMU_34980A_SCPI_NET MSMU = ((MSMU_34980A_SCPI_NET)(TestLib.InstrumentDrivers["MSMU1_34980A"]));

                String message =
                    $"Please connect BMC6030-5 from {Detail}/{Address} Output 1{Environment.NewLine}{Environment.NewLine}" +
                    $"to {MSMU.Detail}/{MSMU.Address}.{Environment.NewLine}{Environment.NewLine}" +
                    "Click Cancel if desired.";
                if (DialogResult.OK == MessageBox.Show(message, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                    MSMU.Ag34980.SCPI.INSTrument.DMM.STATe.Command(true);
                    MSMU.Ag34980.SCPI.INSTrument.DMM.CONNect.Command();
                    TestOutput(OUTPUT2.OUTput1, ref MSMU, limit, ref result_E3649A);
                    MessageBox.Show("Please connect BMC6030-5 to Output 2.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    TestOutput(OUTPUT2.OUTput2, ref MSMU, limit, ref result_E3649A);
                    message =
                        $"Please disconnect BMC6030-5 from {Detail}/{Address}{Environment.NewLine}{Environment.NewLine}" +
                        $"and {MSMU.Detail}/{MSMU.Address}.{Environment.NewLine}{Environment.NewLine}";
                    MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            return result_E3649A;
        }

        private void TestOutput(OUTPUT2 outPut, ref MSMU_34980A_SCPI_NET MSMU, Double limit, ref (Boolean Summary, List<DiagnosticsResult> Details) result_E3649A) {
            Select(outPut);
            AgE364xD.SCPI.OUTPut.STATe.Command(false);
            AgE364xD.SCPI.SOURce.VOLTage.PROTection.STATe.Command(false);
            AgE364xD.SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("MINimum");
            AgE364xD.SCPI.SOURce.VOLTage.LEVel.IMMediate.STEP.INCRement.Command(1D);
            AgE364xD.SCPI.OUTPut.STATe.Command(true);

            Boolean passed_E3649A = true, passed_VDC;
            for (Int32 vdcApplied = 0; vdcApplied < 60; vdcApplied++) {
                Thread.Sleep(millisecondsTimeout: 500);
                MSMU.Ag34980.SCPI.MEASure.SCALar.VOLTage.DC.Query("AUTO", $"{MMD.MAXimum}", ch_list: null, out Double[] vdcMeasured);
                passed_VDC = Math.Abs(vdcMeasured[0] - vdcApplied) <= limit;
                passed_E3649A &= passed_VDC;
                result_E3649A.Details.Add(new DiagnosticsResult(Label: $"{outPut} :", Message: $"Applied {vdcApplied}VDC, measured {Math.Round(vdcMeasured[0], 3, MidpointRounding.ToEven)}VDC", Event: (passed_VDC ? EVENTS.PASS : EVENTS.FAIL)));
                AgE364xD.SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("UP");
            }
            AgE364xD.SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("MINimum");
            AgE364xD.SCPI.OUTPut.STATe.Command(false);
            result_E3649A.Summary &= passed_E3649A;
        }
        #endregion Diagnostics

        public PS_E3649A_SCPI_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            AgE364xD = new AgE364xD(Address);
            InstrumentType = INSTRUMENT_TYPE.POWER_SUPPLY;
        }
    }
}