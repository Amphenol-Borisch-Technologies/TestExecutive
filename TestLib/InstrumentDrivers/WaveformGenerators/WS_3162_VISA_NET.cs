using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Ivi.Visa;
using Keysight.Visa;
using System;
using System.Collections.Generic;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.WaveformGenerators {
    public class WS_3162_VISA_NET : IInstrument, IDiagnostics, IDisposable {
        public UsbSession UsbSession;
        public IMessageBasedFormattedIO FormattedIO => UsbSession.FormattedIO;
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            FormattedIO.WriteLine("*RST");
            FormattedIO.WriteLine("*CLS");
        }

        public SELF_TEST_RESULTS SelfTests() {
            try {
                FormattedIO.WriteLine("*TST?");
                if (Int32.TryParse(FormattedIO.ReadLine(), out Int32 i) && i == 0) return SELF_TEST_RESULTS.PASS;
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

        public WS_3162_VISA_NET(String Address, String Detail, AccessModes AccessMode=AccessModes.None, Int32 TimeoutMilliseconds = -1) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.WAVEFORM_GENERATOR;
            UsbSession = new UsbSession(Address, AccessMode, TimeoutMilliseconds);
            ResetClear();
        }

        public void Dispose() { UsbSession.Dispose(); }

        ~WS_3162_VISA_NET() { Dispose(); }
    }
}
