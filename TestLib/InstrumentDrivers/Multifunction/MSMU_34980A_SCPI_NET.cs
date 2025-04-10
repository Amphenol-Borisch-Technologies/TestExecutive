using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Generic;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Agilent.CommandExpert.ScpiNet.Ag34980_2_43;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
// using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Multifunction {

    public class MSMU_34980A_SCPI_NET : Ag34980, IInstrument, IRelay, IDiagnostics {
        public enum SLOTS { S1 = 1, S2 = 2, S3 = 3, S4 = 4, S5 = 5, S6 = 6, S7 = 7, S8 = 8 }
        public struct Modules {
            public const String M34921A = "34921A";
            public const String M34932A = "34932A";
            public const String M34938A = "34938A";
            public const String M34939A = "34939A";
            public const String M34952A = "34952A";
        }
        public enum TEMPERATURE_UNITS { C, F, K }
        public enum RELAY_STATES { opened, CLOSED }

        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }
        private readonly String _34980A;

        public void ResetClear() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }

        public SELF_TEST_RESULTS SelfTests() {
            if (DialogResult.Cancel == MessageBox.Show($"Please disconnect Address Bus DB9 & all Module/Slot terminal blocks/connectors from {Detail}/{Address}.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            }

            Int32 result;
            try {
                SCPI.TST.Query(out result);
            } catch (Exception e) {
                Instruments.SelfTestFailure(this, e);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)result; // Ag34980 returns 0 for passed, 1 for fail.
        }

        public void OpenAll() { SCPI.ROUTe.OPEN.ALL.Command(null); }

        #region Diagnostics // NOTE: Update MODULES & Modules as necessary, along with Diagnostics region.
        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_34980A = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                (Boolean summary, List<DiagnosticsResult> details) result_Slot;
                Configuration.Parameter parameter = Parameters.Find(p => p.Name == "ModuleType");
                String module = (parameter != null) ? parameter.Value : String.Empty;

                foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) {
                    Data.CT_EmergencyStop.ThrowIfCancellationRequested();
                    Data.CT_Cancel.ThrowIfCancellationRequested();
                    if (String.Equals(module, SystemType(slot)) || String.Equals(module, String.Empty)) {
                        switch (SystemType(slot)) {
                            case Modules.M34921A:
                                result_Slot = Diagnostic_34921A(slot, Parameters);
                                break;
                            case Modules.M34932A:
                                result_Slot = Diagnostic_34932A(slot, Parameters);
                                break;
                            case Modules.M34938A:
                                result_Slot = Diagnostic_34938A(slot, Parameters);
                                break;
                            case Modules.M34939A:
                                result_Slot = Diagnostic_34939A(slot, Parameters);
                                break;
                            case Modules.M34952A:
                                result_Slot = Diagnostic_34952A(slot, Parameters);
                                break;
                            case "0":
                                result_Slot = (true, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: $"Slot '{slot}':", Message: "Empty.", Event: EVENTS.INFORMATION) });
                                break;
                            default:
                                const Int32 PR = 12;
                                String message =
                                    $"Diagnostic test for module '{SystemType(slot)}' unimplemented.{Environment.NewLine}{Environment.NewLine}" +
                                    $"{nameof(SystemDescriptionLong)}".PadRight(PR) + $": '{SystemDescriptionLong(slot)}'.{Environment.NewLine}" +
                                    $"{nameof(Address)}".PadRight(PR) + $": '{Address}'.{Environment.NewLine}" +
                                    $"{nameof(Detail)}".PadRight(PR) + $": '{Detail}'.{Environment.NewLine}";
                                result_Slot = (true, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: $"Slot '{slot}':", Message: message, Event: EVENTS.INFORMATION) });
                                break;
                        }
                        result_34980A.Summary &= result_Slot.summary;
                        result_34980A.Details.AddRange(result_Slot.details);
                    }
                }
            }
            return result_34980A;
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34921As(List<Configuration.Parameter> Parameters) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34921A) Results.Add(slot, Diagnostic_34921A(slot, Parameters));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34921A(SLOTS Slot, List<Configuration.Parameter> Parameters) {
            String S = ((Int32)Slot).ToString("D1");
            if (DialogResult.Cancel == MessageBox.Show($"Please connect BMC6030-1 diagnostic terminal block to {_34980A} SLOT {S}.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            }

            SCPI.ROUTe.OPEN.ALL.Command(null);
            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();

            List<DiagnosticsResult> results = new List<DiagnosticsResult>();
            Boolean passed_34921A = true;
            String D = nameof(Diagnostic_34921A);

            // NOTE: Can't get this code to measure current reliably; varies from 0.02A to 0.12A.
            //PS_E3634A_SCPI_NET PS3_E3634A = ((PS_E3634A_SCPI_NET)(Data.InstrumentDrivers["PS3_E3634A"]));
            //String message = $"Please connect BMC6030-1 DB9 to{Environment.NewLine}" +
            //    $"BMC6030-5 & {PS3_E3634A.Detail}/{PS3_E3634A.Address}.{Environment.NewLine}{Environment.NewLine}" +
            //    $"Click Cancel to skip optional current testing with BMC6030-5 & {PS3_E3634A.Detail}.";
            //if (DialogResult.OK == MessageBox.Show(message, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
            //    // Mayn't have a Keysight E3634A power supply; offer to forego current testing.
            //    Test_ADC(D, String.Empty, (A_low: 0D, A_high: 5E-3D), ref PS3_E3634A, ref passed_34921A, ref results); // Verify relays aren't stuck closed.
            //    Test_ADC(D, $"@{S}041", (A_low: 0D, A_high: 5E-3D), ref PS3_E3634A, ref passed_34921A, ref results);
            //    Test_ADC(D, $"@{S}042", (A_low: 0D, A_high: 5E-3D), ref PS3_E3634A, ref passed_34921A, ref results);
            //    Test_ADC(D, $"@{S}043", (A_low: 0D, A_high: 5E-3D), ref PS3_E3634A, ref passed_34921A, ref results);
            //    Test_ADC(D, $"@{S}044", (A_low: 0D, A_high: 5E-3D), ref PS3_E3634A, ref passed_34921A, ref results);
            //    Test_ADC(D, $"@{S}931", (A_low: 0D, A_high: 5E-3D), ref PS3_E3634A, ref passed_34921A, ref results);

            //    Configuration.Parameter A_low = Parameters.Find(p => p.Name == "Current_34921A_LowADC") ?? new Configuration.Parameter { Name = "Current_34921A_LowA", Value = "0.75" };
            //    Configuration.Parameter A_high = Parameters.Find(p => p.Name == "Current_34921A_HighADC") ?? new Configuration.Parameter { Name = "Current_34921A_HighA", Value = "0.125" };
            //    (Double A_low, Double A_high) LimitsA = (Convert.ToDouble(A_low.Value), Convert.ToDouble(A_high.Value));
            //    Test_ADC(D, $"@{S}041,{S}931", LimitsA, ref PS3_E3634A, ref passed_34921A, ref results);
            //    Test_ADC(D, $"@{S}042,{S}931", LimitsA, ref PS3_E3634A, ref passed_34921A, ref results);
            //    Test_ADC(D, $"@{S}043,{S}931", LimitsA, ref PS3_E3634A, ref passed_34921A, ref results);
            //    Test_ADC(D, $"@{S}044,{S}931", LimitsA, ref PS3_E3634A, ref passed_34921A, ref results);
            //}

            Configuration.Parameter celciusLow = Parameters.Find(p => p.Name == "FRTD_34921A_Low°C") ?? new Configuration.Parameter { Name = "FRTD_34921A_Low°C", Value = "15.5" };
            Configuration.Parameter celciusHigh = Parameters.Find(p => p.Name == "FRTD_34921A_High°C") ?? new Configuration.Parameter { Name = "FRTD_34921A_High°C", Value = "29.5" };
            (Double celciusLow, Double celciusHigh) LimitsCelcius = (Convert.ToDouble(celciusLow.Value), Convert.ToDouble(celciusHigh.Value));
            Test_FRTD(D, $"@{S}020", LimitsCelcius, ref passed_34921A, ref results);

            Configuration.Parameter Ω_closed = Parameters.Find(p => p.Name == "ResistanceRelay_34921A_ClosedΩ") ?? new Configuration.Parameter { Name = "ResistanceRelay_34921A_ClosedΩ", Value = "3" };
            Configuration.Parameter Ω_open = Parameters.Find(p => p.Name == "ResistanceRelay_34921A_OpenΩ") ?? new Configuration.Parameter { Name = "ResistanceRelay_34921A_OpenΩ", Value = "1E9" };
            (Double Ω_closed, Double Ω_open) LimitsΩ = (Convert.ToDouble(Ω_closed.Value), Convert.ToDouble(Ω_open.Value));

            Test_Ω(D, kelvin: false, closed: false, String.Empty, LimitsΩ, ref passed_34921A, ref results);
            Test_Ω(D, kelvin: false, closed: false, $"@{S}911", LimitsΩ, ref passed_34921A, ref results); // DMM Measure.
            Test_Ω(D, kelvin: false, closed: false, $"@{S}921", LimitsΩ, ref passed_34921A, ref results); // DMM Measure.

            SCPI.ROUTe.CLOSe.Command($"@{S}001:{S}019"); // Bank 1 all relays connected to Bank 1 diagnostic shorting connector except FRTD's 020.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}911", LimitsΩ, ref passed_34921A, ref results);               // ABus1 COM1 directly connected to all Bank 1 relays and thus diagnostic shorting connector.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}921,{S}912,{S}922", LimitsΩ, ref passed_34921A, ref results); // ABus1 COM2 indirectly connected through ABus2, to test ABus2.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}921,{S}913,{S}923", LimitsΩ, ref passed_34921A, ref results); // ABus1 COM2 indirectly connected through ABus3, to test ABus3.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}921,{S}914,{S}924", LimitsΩ, ref passed_34921A, ref results); // ABus1 COM2 indirectly connected through ABus4, to test ABus4.
            SCPI.ROUTe.OPEN.Command($"@{S}001:{S}019"); // Reference 'Keysight 34921A-34925A Low Frequency Multiplexer Modules', '34921A Simplified Schematic'.

            SCPI.ROUTe.CLOSe.Command($"@{S}911"); // DMM Measure.
            for (Int32 i = 1; i <= 19; i++) Test_Ω(D, kelvin: false, closed: true, $"@{S}{i:D3}", LimitsΩ, ref passed_34921A, ref results); // Bank 1 individual relays except FRTD's 020.
            SCPI.ROUTe.OPEN.Command($"@{S}911");

            SCPI.ROUTe.CLOSe.Command($"@{S}021:{S}039"); // Bank 2 all relays connected to Bank 2 diagnostic shorting connector except FRTD's 040.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}921", LimitsΩ, ref passed_34921A, ref results);               // ABus1 COM2 directly connected to all Bank 2 relays and thus diagnostic shorting connector.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}911,{S}912,{S}922", LimitsΩ, ref passed_34921A, ref results); // ABus1 COM1 indirectly connected through ABus2, to test ABus2.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}911,{S}913,{S}923", LimitsΩ, ref passed_34921A, ref results); // ABus1 COM1 indirectly connected through ABus3, to test ABus3.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}911,{S}914,{S}924", LimitsΩ, ref passed_34921A, ref results); // ABus1 COM1 indirectly connected through ABus4, to test ABus4.
            SCPI.ROUTe.OPEN.Command($"@{S}021:{S}039"); // Reference 'Keysight 34921A-34925A Low Frequency Multiplexer Modules', '34921A Simplified Schematic'.

            SCPI.ROUTe.CLOSe.Command($"@{S}921"); // DMM Measure.
            for (Int32 i = 21; i <= 39; i++) Test_Ω(D, kelvin: false, closed: true, $"@{S}{i:D3}", LimitsΩ, ref passed_34921A, ref results); // Bank 2 individual relays except FRTD's 040.
            SCPI.ROUTe.OPEN.Command($"@{S}921");

            SCPI.INSTrument.DMM.DISConnect.Command();

            MessageBox.Show($"Please disconnect BMC6030-1 diagnostic terminal block from {_34980A} SLOT {S}.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            return (Summary: passed_34921A, Details: results);
        }

        //private void Test_ADC(String diagnostic, String channels, (Double A_low, Double A_high) Limits, ref PS_E3634A_SCPI_NET PS1_E3634A, ref Boolean passed, ref List<DiagnosticsResult> results) {
        //    if (!String.Equals(channels, String.Empty)) SCPI.ROUTe.CLOSe.Command(channels);
        //    PS1_E3634A.SCPI.APPLy.Command(0.25D, null); // NOTE: BMC6030-5 has a 1Ω series resistor for current measurement, and the 34921A & 34980A have ≈ 1Ω internal resistance..
        //    PS1_E3634A.SCPI.OUTPut.STATe.Command(true);
        //    System.Threading.Thread.Sleep(millisecondsTimeout: 500);
        //    SCPI.MEASure.SCALar.CURRent.DC.QueryQuery(0.15D, "A", "MAXimum", null, out Double[] adc);
        //    PS1_E3634A.SCPI.OUTPut.STATe.Command(false);
        //    PS1_E3634A.SCPI.APPLy.Command(0, null);
        //    System.Threading.Thread.Sleep(millisecondsTimeout: 250);
        //    if (!String.Equals(channels, String.Empty)) SCPI.ROUTe.OPEN.Command(channels);
        //    Boolean passed_A = (Limits.A_low <= adc[0] && adc[0] <= Limits.A_high);
        //    passed &= passed_A;
        //    results.Add(new DiagnosticsResult(Label: $"{diagnostic} channel(s) {channels}: ", Message: $"{Math.Round(adc[0], 3, MidpointRounding.ToEven)} Amperes DC", Event: (passed_A ? EVENTS.PASS : EVENTS.FAIL)));
        //}

        private void Test_FRTD(String diagnostic, String channels, (Double celciusLow, Double celciusHigh) Limits, ref Boolean passed, ref List<DiagnosticsResult> results) {
            SCPI.UNIT.TEMPerature.Command("C", channels);
            SCPI.SENSe.TEMPerature.TRANsducer.FRTD.TYPE.Command(85, channels);
            SCPI.SENSe.TEMPerature.TRANsducer.FRTD.RESistance.REFerence.Command(100.0, channels);
            SCPI.MEASure.SCALar.TEMPerature.Query("FRTD", 85, 110D, "MAXimum", channels, out Double[] degreesC);
            Boolean passed_FRTD = (Limits.celciusLow <= degreesC[0] && degreesC[0] <= Limits.celciusHigh);
            passed &= passed_FRTD;
            results.Add(new DiagnosticsResult(Label: $"{diagnostic} FRTD channel(s) {channels}: ", Message: $"{Math.Round(degreesC[0], 3, MidpointRounding.ToEven)}°C", Event: (passed_FRTD ? EVENTS.PASS : EVENTS.FAIL)));
        }

        private void Test_Ω(String diagnostic, Boolean kelvin, Boolean closed, String channels, (Double Ω_closed, Double Ω_open) Limits, ref Boolean passed, ref List<DiagnosticsResult> results) {
            if (!String.Equals(channels, String.Empty)) SCPI.ROUTe.CLOSe.Command(channels);
            Double[] resistance;
            if (kelvin) SCPI.MEASure.SCALar.FRESistance.Query(25D, $"{MMD.MAXimum}", out resistance);
            else SCPI.MEASure.SCALar.RESistance.Query(25D, $"{MMD.MAXimum}", out resistance);
            if (!String.Equals(channels, String.Empty)) SCPI.ROUTe.OPEN.Command(channels);
            Boolean passed_Ω;
            if (closed) passed_Ω = (0 <= resistance[0] && resistance[0] <= Limits.Ω_closed);
            else passed_Ω = (resistance[0] >= Limits.Ω_open);
            passed &= passed_Ω;
            results.Add(new DiagnosticsResult(Label: $"{diagnostic} channel(s) {channels}: ", Message: $"{Math.Round(resistance[0], 3, MidpointRounding.ToEven)}Ω", Event: (passed_Ω ? EVENTS.PASS : EVENTS.FAIL)));
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34932As(List<Configuration.Parameter> Parameters) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34932A) Results.Add(slot, Diagnostic_34932A(slot, Parameters));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34932A(SLOTS Slot, List<Configuration.Parameter> Parameters) {
            String S = ((Int32)Slot).ToString("D1");
            if (DialogResult.Cancel == MessageBox.Show($"Please connect BMC6030-2 diagnostic terminal block to {_34980A} SLOT {S} and Analog Busses.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            }

            SCPI.ROUTe.OPEN.ALL.Command(null);
            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();

            List<DiagnosticsResult> results = new List<DiagnosticsResult>();
            Boolean passed_34932A = true;

            Configuration.Parameter Ω_closed = Parameters.Find(p => p.Name == "ResistanceRelay_34932A_ClosedΩ") ?? new Configuration.Parameter { Name = "ResistanceRelay_34932A_ClosedΩ", Value = "3" };
            Configuration.Parameter Ω_open = Parameters.Find(p => p.Name == "ResistanceRelay_34932A_OpenΩ") ?? new Configuration.Parameter { Name = "ResistanceRelay_34932A_OpenΩ", Value = "1E9" };
            (Double Ω_closed, Double Ω_open) Limits = (Convert.ToDouble(Ω_closed.Value), Convert.ToDouble(Ω_open.Value));
            String D = nameof(Diagnostic_34932A);

            Test_Ω(D, kelvin: false, closed: false, String.Empty, Limits, ref passed_34932A, ref results);
            Test_Ω(D, kelvin: false, closed: false, $"@{S}921", Limits, ref passed_34932A, ref results); // DMM Measure.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}921,{S}101:{S}116,{S}501:{S}516", Limits, ref passed_34932A, ref results);
            Test_Ω(D, kelvin: false, closed: false, $"@{S}922", Limits, ref passed_34932A, ref results); // DMM Measure.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}922,{S}201:{S}216,{S}601:{S}616", Limits, ref passed_34932A, ref results);
            Test_Ω(D, kelvin: false, closed: false, $"@{S}923", Limits, ref passed_34932A, ref results); // DMM Measure.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}923,{S}301:{S}316,{S}701:{S}716", Limits, ref passed_34932A, ref results);
            Test_Ω(D, kelvin: false, closed: false, $"@{S}924", Limits, ref passed_34932A, ref results); // DMM Measure.
            Test_Ω(D, kelvin: false, closed: true, $"@{S}924,{S}401:{S}416,{S}801:{S}816", Limits, ref passed_34932A, ref results);

            Int32 matrix1 = 100, matrix2 = 500;
            for (Int32 relayAbus = 921; relayAbus <= 924; relayAbus++) {
                SCPI.ROUTe.CLOSe.Command($"@{S}{relayAbus}"); // DMM Measure.
                for (Int32 relayMatrix1 = matrix1 + 1; relayMatrix1 <= matrix1 + 16; relayMatrix1++) Test_Ω(D, kelvin: false, closed: true, $"@{S}{relayMatrix1}", Limits, ref passed_34932A, ref results);
                for (Int32 relayMatrix2 = matrix2 + 1; relayMatrix2 <= matrix2 + 16; relayMatrix2++) Test_Ω(D, kelvin: false, closed: true, $"@{S}{relayMatrix2}", Limits, ref passed_34932A, ref results);
                matrix1 += 100; matrix2 += 100;
                SCPI.ROUTe.OPEN.Command($"@{S}{relayAbus}");
            }

            SCPI.INSTrument.DMM.DISConnect.Command();

            MessageBox.Show($"Please disconnect BMC6030-2 diagnostic terminal block from {_34980A} SLOT {S} and Analog Busses.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            return (Summary: passed_34932A, Details: results);
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34938As(List<Configuration.Parameter> Parameters) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34938A) Results.Add(slot, Diagnostic_34938A(slot, Parameters));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34938A(SLOTS Slot, List<Configuration.Parameter> Parameters) {
            String S = ((Int32)Slot).ToString("D1");
            if (DialogResult.Cancel == MessageBox.Show($"Please connect BMC6030-3 diagnostic terminal block to {_34980A} SLOT {S} and Analog Busses.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            }

            SCPI.ROUTe.OPEN.ALL.Command(null);
            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();

            List<DiagnosticsResult> results = new List<DiagnosticsResult>();
            Boolean passed_34938A = true;

            Configuration.Parameter Ω_closed = Parameters.Find(p => p.Name == "ResistanceRelay_34938A_ClosedΩ") ?? new Configuration.Parameter { Name = "ResistanceRelay_34938A_ClosedΩ", Value = "3" };
            Configuration.Parameter Ω_open = Parameters.Find(p => p.Name == "ResistanceRelay_34938A_OpenΩ") ?? new Configuration.Parameter { Name = "ResistanceRelay_34938A_OpenΩ", Value = "1E9" };
            (Double Ω_closed, Double Ω_open) Limits = (Convert.ToDouble(Ω_closed.Value), Convert.ToDouble(Ω_open.Value));
            String D = nameof(Diagnostic_34938A);

            Test_Ω(D, kelvin: true, closed: false, String.Empty, Limits, ref passed_34938A, ref results);
            Test_Ω(D, kelvin: true, closed: true, $"@{S}001:{S}020", Limits, ref passed_34938A, ref results);
            for (Int32 i = 1; i <= 20; i++) Test_Ω(D, kelvin: true, closed: true, $"@{S}{i:D3}", Limits, ref passed_34938A, ref results);

            SCPI.INSTrument.DMM.DISConnect.Command();

            MessageBox.Show($"Please disconnect BMC6030-3 diagnostic terminal block from {_34980A} SLOT {S} and Analog Busses.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            return (Summary: passed_34938A, Details: results);
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34939As(List<Configuration.Parameter> Parameters) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34939A) Results.Add(slot, Diagnostic_34939A(slot, Parameters));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34939A(SLOTS Slot, List<Configuration.Parameter> Parameters) {
            String S = ((Int32)Slot).ToString("D1");
            if (DialogResult.Cancel == MessageBox.Show($"Please connect BMC6030-TBD diagnostic terminal block to {_34980A} SLOT {S} and Analog Busses.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            }

            SCPI.ROUTe.OPEN.ALL.Command(null);
            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();

            List<DiagnosticsResult> results = new List<DiagnosticsResult>();
            Boolean passed_34939A = true;

            Configuration.Parameter Ω_closed = Parameters.Find(p => p.Name == "ResistanceRelay_34939A_ClosedΩ") ?? new Configuration.Parameter { Name = "ResistanceRelay_34939A_ClosedΩ", Value = "3" };
            Configuration.Parameter Ω_open = Parameters.Find(p => p.Name == "ResistanceRelay_34939A_OpenΩ") ?? new Configuration.Parameter { Name = "ResistanceRelay_34939A_OpenΩ", Value = "1E9" };
            (Double Ω_closed, Double Ω_open) Limits = (Convert.ToDouble(Ω_closed.Value), Convert.ToDouble(Ω_open.Value));
            String D = nameof(Diagnostic_34939A);

            Test_Ω(D, kelvin: true, closed: false, String.Empty, Limits, ref passed_34939A, ref results);
            Test_Ω(D, kelvin: true, closed: true, $"@{S}001:{S}020", Limits, ref passed_34939A, ref results);
            for (Int32 i = 1; i <= 20; i++) Test_Ω(D, kelvin: true, closed: true, $"@{S}{i:D3}", Limits, ref passed_34939A, ref results);

            SCPI.INSTrument.DMM.DISConnect.Command();

            MessageBox.Show($"Please disconnect BMC6030-TBD diagnostic terminal block from {_34980A} SLOT {S} and Analog Busses.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            return (Summary: passed_34939A, Details: results);
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34952As(List<Configuration.Parameter> Parameters) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34952A) Results.Add(slot, Diagnostic_34952A(slot, Parameters));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34952A(SLOTS Slot, List<Configuration.Parameter> Parameters) {
            String S = ((Int32)Slot).ToString("D1");
            if (DialogResult.Cancel == MessageBox.Show($"Please connect BMC6030-4 diagnostic terminal block to {_34980A} SLOT {S} and its DAC1 to Analog Busses.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            }

            SCPI.ROUTe.OPEN.ALL.Command(null);
            List<DiagnosticsResult> results = new List<DiagnosticsResult>();
            Boolean passed_34952A = true;

            Configuration.Parameter AccuracyDAC_34952A_VDC = Parameters.Find(p => p.Name == "AccuracyDAC_34952A_VDC") ?? new Configuration.Parameter { Name = "AccuracyDAC_34952A_VDC", Value = "0.1" };
            Double Limit = Convert.ToDouble(AccuracyDAC_34952A_VDC.Value);
            String D = nameof(Diagnostic_34952A);

            Diagnostic_34952A_DIO(D, $"@{S}001", $"@{S}002", 0b0000_0000, ref passed_34952A, ref results); // Write bits 0-7, Read 8-15.
            Diagnostic_34952A_DIO(D, $"@{S}001", $"@{S}002", 0b1111_1111, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}001", $"@{S}002", 0b0101_0101, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}001", $"@{S}002", 0b1010_1010, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}002", $"@{S}001", 0b0000_0000, ref passed_34952A, ref results); // Write bits 8-15, Read 0-7.
            Diagnostic_34952A_DIO(D, $"@{S}002", $"@{S}001", 0b1111_1111, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}002", $"@{S}001", 0b0101_0101, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}002", $"@{S}001", 0b1010_1010, ref passed_34952A, ref results);

            Diagnostic_34952A_DIO(D, $"@{S}003", $"@{S}004", 0b0000_0000, ref passed_34952A, ref results); // Write bits 16-23, Read 24-31.
            Diagnostic_34952A_DIO(D, $"@{S}003", $"@{S}004", 0b1111_1111, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}003", $"@{S}004", 0b0101_0101, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}003", $"@{S}004", 0b1010_1010, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}004", $"@{S}003", 0b0000_0000, ref passed_34952A, ref results); // Write bits 24-31, Read 16-23.
            Diagnostic_34952A_DIO(D, $"@{S}004", $"@{S}003", 0b1111_1111, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}004", $"@{S}003", 0b0101_0101, ref passed_34952A, ref results);
            Diagnostic_34952A_DIO(D, $"@{S}004", $"@{S}003", 0b1010_1010, ref passed_34952A, ref results);

            SCPI.RST.Command(); // Diagnostic_34952A_Totalizer() fails without reset after Diagnostic_34952A_DIO().
            Diagnostic_34952A_Totalizer(D, $"@{S}005", "POSitive", 256, ref passed_34952A, ref results);
            Diagnostic_34952A_Totalizer(D, $"@{S}005", "NEGative", 256, ref passed_34952A, ref results);

            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();
            for (Double d = -12; d <= 12; d += 0.5) Diagnostic_34952A_DAC(D, $"@{S}006", d, Limit, ref passed_34952A, ref results);
            MessageBox.Show($"Please disconnect DAC1 & connect DAC2 to Analog Busses.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            for (Double d = -12; d <= 12; d += 0.1) Diagnostic_34952A_DAC(D, $"@{S}007", d, Limit, ref passed_34952A, ref results);
            MessageBox.Show($"Please disconnect BMC6030-4 diagnostic terminal block and its DAC2.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            return (Summary: passed_34952A, Details: results);
        }

        private void Diagnostic_34952A_DIO(String diagnostic, String channelWrite, String channelRead, Byte byteWrite, ref Boolean passed, ref List<DiagnosticsResult> results) {
            Int32 int32Write = Convert.ToInt32(byteWrite);
            SCPI.CONFigure.DIGital.DIRection.Command("OUTPut", channelWrite);
            SCPI.CONFigure.DIGital.DIRection.Command("INPut", channelRead);
            SCPI.SOURce.DIGital.DATA.BYTE.Command(int32Write, channelWrite);
            SCPI.SENSe.DIGital.DATA.BYTE.Query(null, channelRead, out Int32 int32Read);
            Byte byteRead = Convert.ToByte(int32Read);
            Boolean passed_DIO = (byteRead == byteWrite);
            passed &= passed_DIO;
            results.Add(new DiagnosticsResult(Label: $"{diagnostic} Write {channelWrite}, Read {channelRead}: ", Message: $"Wrote: 0b{Convert.ToString(byteWrite, 2).PadLeft(8, '0')}, Read: 0b{Convert.ToString(byteRead, 2).PadLeft(8, '0')}", Event: passed_DIO ? EVENTS.PASS : EVENTS.FAIL));
        }

        private void Diagnostic_34952A_Totalizer(String diagnostic, String channel, String Slope, Int32 countsWrite, ref Boolean passed, ref List<DiagnosticsResult> results) {
            SCPI.SENSe.TOTalize.SLOPe.Command(Slope, channel);
            SCPI.SENSe.TOTalize.THReshold.MODE.Command("TTL", channel);
            SCPI.SENSe.TOTalize.TYPE.Command("READ", channel);
            SCPI.SENSe.TOTalize.CLEar.IMMediate.Command(channel);
            String Slot = channel.Substring(1, 1);
            SCPI.CONFigure.DIGital.DIRection.Command("OUTPut", $"@{Slot}001");

            for (Int32 i = 0; i < countsWrite; i++) {
                SCPI.SOURce.DIGital.DATA.BIT.Command(state: 0, bit: 0, $"@{Slot}001");
                SCPI.SOURce.DIGital.DATA.BIT.Command(state: 1, bit: 0, $"@{Slot}001");
                System.Threading.Thread.Sleep(1);
            }
            SCPI.SENSe.TOTalize.DATA.Query(channel, out Double[] counts);

            Int32 countsRead = Convert.ToInt32(counts[0]);
            Boolean passed_Totalizer = (countsRead == countsWrite);
            passed &= passed_Totalizer;
            results.Add(new DiagnosticsResult(Label: $"{diagnostic} channel {channel} Slope {Slope}: ", Message: $"Wrote: {countsWrite}, Read: {countsRead}", Event: passed_Totalizer ? EVENTS.PASS : EVENTS.FAIL));
        }

        private void Diagnostic_34952A_DAC(String diagnostic, String channel, Double voltsSourced, Double Limit, ref Boolean passed, ref List<DiagnosticsResult> results) {
            SCPI.SOURce.VOLTage.LEVel.Command(voltsSourced, channel);
            SCPI.MEASure.SCALar.VOLTage.DC.Query("AUTO", $"{MMD.MAXimum}", out Double[] voltsMeasured);
            Boolean passed_DAC = Math.Abs(voltsSourced - voltsMeasured[0]) <= Limit;
            passed &= passed_DAC;
            results.Add(new DiagnosticsResult(Label: $"{diagnostic} channel {channel}: ", Message: $"Volts Sourced: {Math.Round(voltsSourced, 3, MidpointRounding.ToEven)}, Volts Measured: {Math.Round(voltsMeasured[0], 3, MidpointRounding.ToEven)}", Event: passed_DAC ? EVENTS.PASS : EVENTS.FAIL));
        }
        #endregion Diagnostics

        public MSMU_34980A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.MULTI_FUNCTION;
            DateTime now = DateTime.Now;
            SCPI.SYSTem.DATE.Command(now.Year, now.Month, now.Day);
            SCPI.SYSTem.TIME.Command(now.Hour, now.Minute, Convert.ToDouble(now.Second));
            SCPI.UNIT.TEMPerature.Command($"{TEMPERATURE_UNITS.F}");
            SCPI.IDN.Query(out String idn);
            _34980A = idn.Split(',')[(Int32)SCPI_NET.IDN_FIELDS.Model];

        }

        public Boolean InstrumentDMM_Installed() {
            SCPI.INSTrument.DMM.INSTalled.Query(out Boolean installed);
            return installed;
        }
        public STATES InstrumentDMM_Get() {
            SCPI.INSTrument.DMM.STATe.Query(out Boolean mode);
            return mode ? STATES.ON : STATES.off;
        }
        public void InstrumentDMM_Set(STATES State) { SCPI.INSTrument.DMM.STATe.Command(State == STATES.ON); }
        public (Int32 Min, Int32 Max) ModuleChannels(SLOTS Slot) {
            switch (SystemType(Slot)) {
                case Modules.M34921A: return (Min: 1, Max: 44);
                case Modules.M34939A: return (Min: 1, Max: 68);
                case Modules.M34952A: return (Min: 1, Max: 7);
                default: throw new NotImplementedException($"Module Type '{SystemType(Slot)}' not implemented.");
            }
        }
        public void RouteCloseExclusive(String Channels) {
            ValidateChannelS(Channels);
            SCPI.ROUTe.CLOSe.EXCLusive.Command($"({Channels})");
        }
        public Boolean RouteGet(String Channels, RELAY_STATES State) {
            ValidateChannelS(Channels);
            Boolean[] states;
            if (State is RELAY_STATES.opened) SCPI.ROUTe.OPEN.Query(Channels, out states);
            else SCPI.ROUTe.CLOSe.Query(Channels, out states);
            List<Boolean> lb = states.ToList();
            return lb.TrueForAll(b => b == true);
        }
        public void RouteSet(String Channels, RELAY_STATES State) {
            ValidateChannelS(Channels);
            if (State is RELAY_STATES.opened) SCPI.ROUTe.OPEN.Command(Channels);
            else SCPI.ROUTe.CLOSe.Command(Channels);
        }
        public String SystemDescriptionLong(SLOTS Slot) {
            SCPI.SYSTem.CDEScription.LONG.Query((Int32)Slot, out String description);
            return description;
        }
        public Double SystemModuleTemperature(SLOTS Slot) {
            SCPI.SYSTem.MODule.TEMPerature.Query("TRANsducer", (Int32)Slot, out Double temperature);
            return temperature;
        }
        public String SystemType(SLOTS Slot) {
            SCPI.SYSTem.CTYPe.Query((Int32)Slot, out String identity);
            return identity.Split(',')[(Int32)Generic.SCPI_NET.IDN_FIELDS.Model];
        }
        public TEMPERATURE_UNITS UnitsGet() {
            SCPI.UNIT.TEMPerature.Query(out String[] units);
            return (TEMPERATURE_UNITS)Enum.Parse(typeof(TEMPERATURE_UNITS), String.Join("", units).Replace("[", "").Replace("]", ""));
        }
        public void ValidateChannelS(String Channels) {
            if (!Regex.IsMatch(Channels, @"^@\d{4}((,|:)\d{4})*$")) { // https://regex101.com/.
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Invalid syntax for {nameof(Channels)} '{Channels}'.");
                stringBuilder.AppendLine(" - Must be in form of 1 or more discrete channels and/or ranges preceded by '@'.");
                stringBuilder.AppendLine(" - Channel:  '@####':       Discrete channels must be separated by commas; '@1001,1002'.");
                stringBuilder.AppendLine(" - Range:    '@####:####':  Channel ranges must be separated by colons; '@1001:1002'.");
                stringBuilder.AppendLine(" - Examples: '@1001', '@1001,2001,2005', '@1001,2001:2005' & '@1001,2001:2005,2017,3001:3015,3017' all valid.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Caveats:");
                stringBuilder.AppendLine(" - Whitespace not permitted; '@1001, 1005', '@1001 ,1005' '& '@1001: 1005' all invalid.");
                stringBuilder.AppendLine(" - Range cannot include ABus channels, denoted as #9##.  Thus range '@1001:1902' invalid, but discretes '@1001,1902' valid.");
                stringBuilder.AppendLine(" - First & only first channel begins with '@'.  Thus '1001,2001' & '@1001,@2001' both invalid.");
                throw new ArgumentException(stringBuilder.ToString());
            }
            if (Regex.IsMatch(Channels, @":\d{4}:")) throw new ArgumentException($"Invalid syntax for Channels '{Channels}'.  Invalid range ':####:'.");
            Channels = Channels.Replace("@", String.Empty);

            if (Channels.Length == 4) {
                ValidateChannel(Channels);
                return;
            }

            String[] channelsOrRanges = Channels.Split(new Char[] { ',' }, StringSplitOptions.None);
            foreach (String channelOrRange in channelsOrRanges) {
                if (Regex.IsMatch(channelOrRange, ":")) ValidateRange(channelOrRange);
                else ValidateChannel(channelOrRange);
            }
        }
        public void ValidateChannel(String Channel) {
            Int32 slotNumber = Int32.Parse(Channel.Substring(0, 2));
            if (!Enum.IsDefined(typeof(SLOTS), (SLOTS)slotNumber)) throw new ArgumentException($"{nameof(Channel)} '{Channel}' must have valid integer Slot in interval [{(Int32)SLOTS.S1}..{(Int32)SLOTS.S8}].");
            Int32 channel = Int32.Parse(Channel.Substring(2));
            (Int32 min, Int32 max) = ModuleChannels((SLOTS)slotNumber);
            if (channel < min || max < channel) throw new ArgumentException($"{nameof(Channel)} '{Channel}' must have valid integer {nameof(Channel)} in interval [{min:D3}..{max:D3}].");
        }
        public void ValidateRange(String Range) {
            String[] channels = Range.Split(new Char[] { ':' }, StringSplitOptions.None);
            if (channels[0][1].Equals('9') || channels[1][1].Equals('9')) throw new ArgumentException($"{nameof(Range)} '{Range}' cannot include ABus #9##.");
            ValidateChannel(channels[0]);
            ValidateChannel(channels[1]);
            if (Convert.ToInt32(channels[0]) >= Convert.ToInt32(channels[1])) throw new ArgumentException($"{nameof(Range)} '{Range}' start {nameof(channels)} '{channels[0]}' must be < end {nameof(Range)} '{channels[1]}'.");
        }
    }
}