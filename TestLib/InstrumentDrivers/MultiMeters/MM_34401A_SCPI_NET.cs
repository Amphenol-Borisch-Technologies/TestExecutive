using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Agilent.CommandExpert.ScpiNet.Ag34401_11;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.MultiMeters {
    public class MM_34401A_SCPI_NET : Ag34401, IInstrument, IDiagnostics {
        public enum MMDA { MIN, MAX, DEF, AUTO }
        public enum TERMINALS { Front, Rear };
        public enum PROPERTY { AmperageAC, AmperageDC, Continuity, Frequency, Fresistance, Period, Resistance, VoltageAC, VoltageDC, VoltageDiodic }


        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }

        public SELF_TEST_RESULTS SelfTests() {
            Boolean result;
            try {
                SCPI.TST.Query(out result);
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

        public MM_34401A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.MULTI_METER;
            TerminalsSetRear();
        }

        public Boolean DelayAutoIs() {
            SCPI.TRIGger.DELay.AUTO.Query(out Boolean state);
            return state;
        }

        public Double Get(PROPERTY property) {
            // SCPI FORMAT:DATA(ASCii/REAL) command unavailable on KS 34461A.
            switch (property) {
                case PROPERTY.AmperageAC:
                    SCPI.MEASure.CURRent.AC.Query($"{MMDA.DEF}", $"{MMDA.DEF}", out Double acCurrent);
                    return acCurrent;
                case PROPERTY.AmperageDC:
                    SCPI.MEASure.CURRent.DC.Query($"{MMDA.DEF}", $"{MMDA.DEF}", out Double dcCurrent);
                    return dcCurrent;
                case PROPERTY.Continuity:
                    SCPI.MEASure.CONTinuity.Query(out Double continuity);
                    return continuity;
                case PROPERTY.Frequency:
                    SCPI.MEASure.FREQuency.Query($"{MMDA.DEF}", $"{MMDA.DEF}", out Double frequency);
                    return frequency;
                case PROPERTY.Fresistance:
                    SCPI.MEASure.FRESistance.Query($"{MMDA.DEF}", $"{MMDA.DEF}", out Double fresistance);
                    return fresistance;
                case PROPERTY.Period:
                    SCPI.MEASure.PERiod.Query($"{MMDA.DEF}", $"{MMDA.DEF}", out Double period);
                    return period;
                case PROPERTY.Resistance:
                    SCPI.MEASure.RESistance.Query($"{MMDA.DEF}", $"{MMDA.DEF}", out Double resistance);
                    return resistance;
                case PROPERTY.VoltageAC:
                    SCPI.MEASure.VOLTage.AC.Query($"{MMDA.DEF}", $"{MMDA.DEF}", out Double acVoltage);
                    return acVoltage;
                case PROPERTY.VoltageDC:
                    SCPI.MEASure.VOLTage.DC.Query($"{MMDA.DEF}", $"{MMDA.DEF}", out Double dcVoltage);
                    return dcVoltage;
                case PROPERTY.VoltageDiodic:
                    SCPI.MEASure.DIODe.Query(out Double diodeVoltage);
                    return diodeVoltage;
                default:
                    throw new NotImplementedException(TestLib.NotImplementedMessageEnum<PROPERTY>(Enum.GetName(typeof(PROPERTY), property)));
            }
        }

        public void TerminalsSetRear() {
            if (TerminalsGet() == TERMINALS.Front) _ = MessageBox.Show("Please depress Keysight 34401A Front/Rear button.", "Paused, click OK to continue.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            SCPI.TRIGger.DELay.AUTO.Command(true);
        }

        public TERMINALS TerminalsGet() {
            SCPI.ROUTe.TERMinals.Query(out String terminals);
            return String.Equals(terminals, "REAR") ? TERMINALS.Rear : TERMINALS.Front;
        }
    }
}