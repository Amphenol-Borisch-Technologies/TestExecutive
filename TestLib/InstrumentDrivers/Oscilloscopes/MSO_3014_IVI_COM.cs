using ABT.Test.TestLib.InstrumentDrivers.Interfaces;
using System;
using System.Collections.Generic;
using Tektronix.Tkdpo2k3k4k.Interop;

namespace ABT.Test.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_IVI_COM : Tkdpo2k3k4kClass, IInstrument, IDiagnostics, IDisposable {
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }
        private Boolean disposed = false;

        public void ResetClear() { Utility.Reset(); }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 TestResult = 0;
            String TestMessage = String.Empty;
            try {
                UtilityEx.SelfTest(ref TestResult, ref TestMessage);
            } catch (Exception e) {
                Instruments.SelfTestFailure(this, e);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)TestResult; // Tkdpo2k3k4kClass returns 0 for passed, 1 for fail.
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_3014 = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                // TODO: Eventually; add verification measurements of the MSO-3014 mixed signal oscilloscope using external instrumentation.
            }
            return result_3014;
        }

        public MSO_3014_IVI_COM(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.OSCILLOSCOPE_MIXED_SIGNAL;
            Initialize(ResourceName: Address, IdQuery: false, Reset: false, OptionString: String.Empty);
        }

        ~MSO_3014_IVI_COM() { Dispose(false); }

        // public override void Close() { Dispose(); } NOTE: Overriding Close() causes Data.GetDerivedClassnames<> to throw
        // 'System.Reflection.ReflectionTypeLoadException' in mscorlib.dll.
        // Types extending from COM objects should override all methods of an interface implemented by the base COM class.

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing) {
            if (!disposed) {
                if (disposing) { } // Free managed resources specific to MSO_3014_IVI_COM; none as yet.
                base.Close();      // Free unmanaged resources specific to MSO_3014_IVI_COM; invoke Tkdpo2k3k4kClass.Close().
                disposed = true;   // Can only invoke Dispose(Boolean disposing) once and thus only base.Close() once, as is required. 
            }
        }
    }
}