using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Agilent.CommandExpert.ScpiNet.Ag34401_11;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.MultiMeters {
    public class MM_34401A_SCPI_NET : IInstrument, IDiagnostics {
        public enum MMD { MIN, MAX, DEF }
        public enum TERMINALS { Front, Rear };
        public enum PROPERTY { AmperageAC, AmperageDC, Continuity, Frequency, Fresistance, Period, Resistance, VoltageAC, VoltageDC, VoltageDiodic }


        public String Address { get; }
        public String Detail { get; }
        public Ag34401 Ag34401 { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            Ag34401.SCPI.RST.Command();
            Ag34401.SCPI.CLS.Command();
        }

        public SELF_TEST_RESULTS SelfTests() {
            Boolean result;
            try {
                Ag34401.SCPI.TST.Query(out result);
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULTS.FAIL;
            }
            return result ? SELF_TEST_RESULTS.FAIL : SELF_TEST_RESULTS.PASS; // Ag34401 returns 0 for passed, 1 for fail, opposite of C#'s Convert.ToBoolean(Int32).
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_34401A = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                // TODO: Eventually; add verification measurements of the 34401A multi-meter using external instrumentation.
            }
            return result_34401A;
        }

        public MM_34401A_SCPI_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            Ag34401 = new Ag34401(Address);
            InstrumentType = INSTRUMENT_TYPES.MULTI_METER;
            TerminalsSetRear();
        }

        public Boolean DelayAutoIs() {
            Ag34401.SCPI.TRIGger.DELay.AUTO.Query(out Boolean state);
            return state;
        }

        public Double Get(PROPERTY property) {
            // SCPI FORMAT:DATA(ASCii/REAL) command unavailable on KS 34461A.
            switch (property) {
                case PROPERTY.AmperageAC:
                    Ag34401.SCPI.MEASure.CURRent.AC.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double acCurrent);
                    return acCurrent;
                case PROPERTY.AmperageDC:
                    Ag34401.SCPI.MEASure.CURRent.DC.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double dcCurrent);
                    return dcCurrent;
                case PROPERTY.Continuity:
                    Ag34401.SCPI.MEASure.CONTinuity.Query(out Double continuity);
                    return continuity;
                case PROPERTY.Frequency:
                    Ag34401.SCPI.MEASure.FREQuency.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double frequency);
                    return frequency;
                case PROPERTY.Fresistance:
                    Ag34401.SCPI.MEASure.FRESistance.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double fresistance);
                    return fresistance;
                case PROPERTY.Period:
                    Ag34401.SCPI.MEASure.PERiod.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double period);
                    return period;
                case PROPERTY.Resistance:
                    Ag34401.SCPI.MEASure.RESistance.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double resistance);
                    return resistance;
                case PROPERTY.VoltageAC:
                    Ag34401.SCPI.MEASure.VOLTage.AC.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double acVoltage);
                    return acVoltage;
                case PROPERTY.VoltageDC:
                    Ag34401.SCPI.MEASure.VOLTage.DC.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double dcVoltage);
                    return dcVoltage;
                case PROPERTY.VoltageDiodic:
                    Ag34401.SCPI.MEASure.DIODe.Query(out Double diodeVoltage);
                    return diodeVoltage;
                default:
                    throw new NotImplementedException(TestLib.NotImplementedMessageEnum<PROPERTY>(Enum.GetName(typeof(PROPERTY), property)));
            }
        }

        public void TerminalsSetRear() {
            if (TerminalsGet() == TERMINALS.Front) _ = MessageBox.Show("Please depress Keysight 34401A Front/Rear button.", "Paused, click OK to continue.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            Ag34401.SCPI.TRIGger.DELay.AUTO.Command(true);
        }

        public TERMINALS TerminalsGet() {
            Ag34401.SCPI.ROUTe.TERMinals.Query(out String terminals);
            return String.Equals(terminals, "REAR") ? TERMINALS.Rear : TERMINALS.Front;
        }
    }
}