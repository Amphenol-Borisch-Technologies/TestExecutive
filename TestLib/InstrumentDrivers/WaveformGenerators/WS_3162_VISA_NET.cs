using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Ivi.Visa;
using Keysight.Visa;
using System;
using System.Collections.Generic;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.WaveformGenerators {
    public class WS_3162_VISA_NET : IInstrument, IDiagnostics, IDisposable {
        public enum CHANNELS { C1, C2 }
        public enum CLOCK_SOURCE { INT, EXT }
        public enum COMMAND_HEADERS { OFF, SHORT, LONG }
        public enum CONFIGURATIONS { DEFAULT, LAST }
        public enum MINUTES { OFF, M1 = 1, M5 = 5, M15 = 15, M30 = 30, M60 = 60, M120 = 120, M300 = 300 }
        public enum STATUSES { OFF, ON }
        public enum VIRTUAL_KEYS { KB_BURST = 17, KB_CHANNEL = 33, KB_FUNC1 = 28, KB_FUNC2 = 23, KB_FUNC3 = 18, KB_FUNC4 = 13,
            KB_FUNC5 = 8, KB_FUNC6 = 3, KB_HELP = 12, KB_KNOB_DOWN = 176, KB_KNOB_LEFT = 177, KB_KNOB_RIGHT = 175, KB_LEFT = 44,
            KB_MOD = 15, KB_NEGATIVE = 43, KB_NUMBER_0 = 48, KB_NUMBER_1 = 49, KB_NUMBER_2 = 50, KB_NUMBER_3 = 51, KB_NUMBER_4 = 52,
            KB_NUMBER_5 = 53, KB_NUMBER_6 = 54, KB_NUMBER_7 = 55, KB_NUMBER_8 = 56, KB_NUMBER_9 = 57, KB_OUTPUT1 = 153, KB_OUTPUT2 = 152,
            KB_PARAMETER = 5, KB_POINT = 46, KB_RIGHT = 40, KB_SWEEP = 16, KB_UTILITY = 11, KB_WAVES = 4
        }

        public UsbSession UsbSession;
        public IMessageBasedFormattedIO FormattedIO => UsbSession.FormattedIO;
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void BuzzerCommand(STATUSES Status) { FormattedIO.WriteLine($"*BUZZer {Status}"); }
        public STATUSES BuzzerQuery() {
            FormattedIO.WriteLine("*BUZZer?");
            String response = FormattedIO.ReadLine();
            return (STATUSES)Enum.Parse(typeof(STATUSES), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void ChannelParameterCopyCommand(CHANNELS ChannelSource) { FormattedIO.WriteLine($"*PAraCoPy {(ChannelSource == CHANNELS.C1 ? CHANNELS.C2 : CHANNELS.C1)},{ChannelSource}"); }
        public void ClearStatusCommand() { FormattedIO.WriteLine("*CLS"); }
        public void ClockSourceCommand(CLOCK_SOURCE ClockSource) { FormattedIO.WriteLine($"*ROSCillator {ClockSource}"); }
        public CLOCK_SOURCE ClockSourceQuery() {
            FormattedIO.WriteLine("*ROSCillator?");
            String response = FormattedIO.ReadLine();
            return (CLOCK_SOURCE)Enum.Parse(typeof(CLOCK_SOURCE), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void CommandHeaderCommand(COMMAND_HEADERS CommandHeader) { FormattedIO.WriteLine($"*Comm_HeaDeR {CommandHeader}"); }
        public COMMAND_HEADERS CommandHeaderQuery() {
            FormattedIO.WriteLine("*Comm_HeaDeR?");
            String response = FormattedIO.ReadLine();
            return (COMMAND_HEADERS)Enum.Parse(typeof(COMMAND_HEADERS), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void ConfigurationCommand(CONFIGURATIONS Configuration) { FormattedIO.WriteLine($"*Sys_CFG {Configuration}"); }
        public CONFIGURATIONS ConfigurationQuery() {
            FormattedIO.WriteLine("*Sys_CFG?");
            String response = FormattedIO.ReadLine();
            return (CONFIGURATIONS)Enum.Parse(typeof(CONFIGURATIONS), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void EventStatusEnableCommand(Byte RegisterMask) { FormattedIO.WriteLine($"*ESE {RegisterMask}"); }
        public Byte EventStatusEnableQuery() {
            FormattedIO.WriteLine("*ESE?");
            return Byte.Parse(FormattedIO.ReadLine().Substring(5));
        }
        public Byte EventStatusRegisterQuery() {
            FormattedIO.WriteLine("*ESR?");
            return Byte.Parse(FormattedIO.ReadLine().Substring(5));
        }
        public String IdentityQuery() {
            FormattedIO.WriteLine("*IDN?");
            return FormattedIO.ReadLine();
        }
        public void InvertCommand(STATUSES Status) { FormattedIO.WriteLine($"*INVerT {Status}"); }
        public STATUSES InvertQuery() {
            FormattedIO.WriteLine("*INVerT?");
            String response = FormattedIO.ReadLine();
            return (STATUSES)Enum.Parse(typeof(STATUSES), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void OperationCompleteCommand() { FormattedIO.WriteLine($"*OPC"); }
        public Byte OperationCompleteQuery() {
            FormattedIO.WriteLine($"*OPC?");
            return Byte.Parse(FormattedIO.ReadLine().Substring(5));
        }
        public void ScreenSaveCommand(MINUTES Minutes) {
            if (Minutes == MINUTES.OFF) FormattedIO.WriteLine($"SCreen_SaVe {Minutes}");
            else FormattedIO.WriteLine($"SCreen_SaVe {(Int32)Minutes}");
        }
        public void ServiceRequestEnableCommand(Byte RegisterMask) { FormattedIO.WriteLine($"*SRE {RegisterMask}"); }
        public Byte ServiceRequestEnableQuery() {
            FormattedIO.WriteLine("*SRE?");
            return Byte.Parse(FormattedIO.ReadLine().Substring(5));
        }
        public Byte StatusRegisterQuery() {
            FormattedIO.WriteLine("*STB?");
            return Byte.Parse(FormattedIO.ReadLine().Substring(5));
        }
        public void SynchronizeCommand(CHANNELS Channel, STATUSES Status) { FormattedIO.WriteLine($"{Channel}:SYNC {Status}"); }
        public CHANNELS SynchronizeQuery() {
            FormattedIO.WriteLine("*SYNC?");
            String response = FormattedIO.ReadLine();
            return (CHANNELS)Enum.Parse(typeof(CHANNELS), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public String TestQuery() {
            FormattedIO.WriteLine("*TST?");
            return FormattedIO.ReadLine().Substring(5);
        }
        public void VirtualKeyCommand(VIRTUAL_KEYS VirtualKey) { FormattedIO.WriteLine($"VirtualKEY {VirtualKey},STATE,1"); }

        public void ResetCommand() { FormattedIO.WriteLine("*RST"); }
        public void WaitCommand() { FormattedIO.WriteLine("*WAI"); }
        public void ResetClear() {
            ResetCommand();
            ClearStatusCommand();
        }

        public SELF_TEST_RESULTS SelfTests() {
            try {
                FormattedIO.WriteLine("*TST?");
                if (TestQuery().Equals("0")) return SELF_TEST_RESULTS.PASS;
                return SELF_TEST_RESULTS.FAIL;
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULTS.FAIL;
            }
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_3162 = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                // TODO: Eventually; add verification measurements of the WS-3162 waveform generator using external instrumentation.
            }
            return result_3162;
        }

        public WS_3162_VISA_NET(String Address, String Detail, AccessModes AccessMode = AccessModes.None, Int32 TimeoutMilliseconds = -1) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.WAVEFORM_GENERATOR;
            UsbSession = new UsbSession(Address, AccessMode, TimeoutMilliseconds);
            ResetCommand();
            ClearStatusCommand();
            CommandHeaderCommand(COMMAND_HEADERS.LONG);
            ScreenSaveCommand(MINUTES.M5);
            BuzzerCommand(STATUSES.ON);
        }

        public void Dispose() { UsbSession.Dispose(); }

        ~WS_3162_VISA_NET() { Dispose(); }
    }
}
