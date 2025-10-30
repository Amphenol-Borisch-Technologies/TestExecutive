using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tektronix.Tkdpo2k3k4k.Interop;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_IVI_COM : Tkdpo2k3k4kClass, IInstrument, IDiagnostics, IDisposable {
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }
        private Boolean disposed = false;
        private readonly static String ValidCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=+-!@#$%^&()[]{}~‘’,";
        public void ResetClear() { Reset(); }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 TestResult = 0;
            String TestMessage = String.Empty;
            try {
                UtilityEx.SelfTest(ref TestResult, ref TestMessage);
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
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

        public void WaitForOperationComplete() {
            WriteString("*WAI");
            WriteString("*OPC?");
            if (ReadString().Trim().Trim('"') != "1") throw new Exception("MSO-3014 didn't complete SCPI command!");
        }

        public enum BUSES { B1, B2 }
        public void BusEventTableEnable(BUSES Buses) {
            switch (Buses) {
                case BUSES.B1:
                    WriteString(":FPAnel:PRESS B1;:*WAI");
                    break;
                case BUSES.B2:
                    WriteString(":FPAnel:PRESS B2;:*WAI");
                    break;
                default:
                    throw new NotImplementedException(NotImplementedMessageEnum<BUSES>(Enum.GetName(typeof(BUSES), Buses)));
            }
            WriteString(":FPAnel:PRESS BMENU7;:*WAI");
            WriteString(":FPAnel:PRESS RMENU1;:*WAI");
            WriteString(":FPAnel:PRESS MENUOff;:*WAI");
            WriteString(":FPAnel:PRESS MENUOff;:*WAI");
            WaitForOperationComplete();
        }

        public void SetupLoad(String SetupFilePath) {
            ResetClear();
            foreach (String mso_3014_SCPI_Command in File.ReadLines(SetupFilePath)) {
                WriteString(mso_3014_SCPI_Command);
                WaitForOperationComplete();
            }
        }

        public enum SETUPS { SETUP1 = 1, SETUP2 = 2, SETUP3 = 3, SETUP4 = 4, SETUP5 = 5, SETUP6 = 6, SETUP7 = 7, SETUP8 = 8, SETUP9 = 9, SETUP10 = 10 }
        public void SetupSave(SETUPS Setup, String Label) {
            if (Label.Length > 30) throw new ArgumentException("MSO-3014 Setup label length cannot exceed 30 characters!");
            if (!AllValidCharacters(Label)) throw new ArgumentException($"MSO-3014 Setup label can only contain characters in set \"{ValidCharacters}\".");
            WriteString($":{Setup}:LABEL \"{Label}\"");
            WaitForOperationComplete();
            WriteString($":{Setup}:LABEL?");
            String labelRead = ReadString().Trim().Trim('"');
            if (!labelRead.Equals(Label)) throw new Exception($"MSO-3014 {Setup} not labeled correctly!{Environment.NewLine}  Should be '{Label}'.{Environment.NewLine}  Is '{labelRead}'.");
        }

        private static Boolean AllValidCharacters(String CharacterString) {
            return CharacterString.All(new HashSet<Char>(ValidCharacters.ToCharArray()).Contains);
        }

        ~MSO_3014_IVI_COM() { Dispose(false); }

        // public override void Close() { Dispose(); } NOTE: Overriding Close() causes TestLib.GetDerivedClassnames<> to throw
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