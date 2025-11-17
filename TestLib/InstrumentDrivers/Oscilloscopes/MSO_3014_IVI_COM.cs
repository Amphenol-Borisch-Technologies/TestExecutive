using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Ivi.Visa;
using Keysight.Visa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Tektronix.Tkdpo2k3k4k.Interop;
using static ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Oscilloscopes.MSO_3014_IVI_COM;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_IVI_COM : Tkdpo2k3k4kClass, IInstrument, IDiagnostics, IDisposable {
        public String Address { get; }
        public String Detail { get; }
        public UsbSession USB_Session;
        public enum BUSES { B1, B2 }
        public enum DRIVES_USB { E, F }
        public INSTRUMENT_TYPES InstrumentType { get; }
        public enum SETUPS { SETUP1 = 1, SETUP2 = 2, SETUP3 = 3, SETUP4 = 4, SETUP5 = 5, SETUP6 = 6, SETUP7 = 7, SETUP8 = 8, SETUP9 = 9, SETUP10 = 10 }
        public readonly static String ValidCharactersFile = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=+-!@#$%^&()[]{}~‘’,";
        public readonly static String ValidCharactersLabel = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=≠+-±!@#$%^&*()[]{}<>/~‘’\"\\|:,.?µ∞∆°Ωσ";
        private Boolean _disposed = false;

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
            USB_Session = new UsbSession(Address, AccessModes.None, timeoutMilliseconds: 20000);
        }

        public void OperationCompleteQuery() {
            WriteString("*OPC?");
            if (ReadString().Trim().Trim('"') != "1") throw new InvalidOperationException("MSO-3014 didn't complete SCPI command!");
        }

        public void EventTableEnable(BUSES Bus) {
            switch (Bus) {
                case BUSES.B1:
                    WriteString(":FPAnel:PRESS B1;:*WAI");
                    break;
                case BUSES.B2:
                    WriteString(":FPAnel:PRESS B2;:*WAI");
                    break;
                default:
                    throw new NotImplementedException(NotImplementedMessageEnum<BUSES>(Enum.GetName(typeof(BUSES), Bus)));
            }
            WriteString(":FPAnel:PRESS BMENU7;:*WAI");
            WriteString(":FPAnel:PRESS RMENU1;:*WAI");
            WriteString(":FPAnel:PRESS MENUOff;:*WAI");
            WriteString(":FPAnel:PRESS MENUOff;:*WAI");
            OperationCompleteQuery();
        }

        public void EventTableSave(BUSES Bus, DRIVES_USB Drive_USB, String PathPC) {
            String pathMSO_3014 = $"\"{Drive_USB}:/{Bus}.csv\"";
            USB_Session.FormattedIO.WriteLine($"SAVe:EVENTtable:{Bus} {pathMSO_3014}"); // Save Event Table to MSO-3014 USB drive, overwriting any existing file without warning.  Can't HARDCopy Event Tables, sadly.
            OperationCompleteQuery();
            Thread.Sleep(500);                                                          // USB Drive write latency.

            USB_Session.FormattedIO.WriteLine($"FILESystem:READFile {pathMSO_3014}");   // Read Event Table from MSO-3014 USB drive.
            File.WriteAllBytes($@"{PathPC}", USB_Session.RawIO.Read());                 // Save read Event Table to PC, overwriting any existing file without warning.
            OperationCompleteQuery();

            USB_Session.FormattedIO.WriteLine($"FILESystem:DELEte {pathMSO_3014}");     // Delete Event Table from MSO-3014 USB drive.
            OperationCompleteQuery();
        }

        public void ImageLandscapePNG_Save(String PathPC) {
            USB_Session.FormattedIO.WriteLine("SAVe:IMAGe:INKSaver OFF");
            USB_Session.FormattedIO.WriteLine("SAVe:IMAGe:LAYout LANdscape");
            USB_Session.FormattedIO.WriteLine("SAVe:IMAGe:FILEFormat PNG");
            OperationCompleteQuery();
            USB_Session.FormattedIO.WriteLine("HARDCopy STARt");        // Ostensibly a printing command, actually works _best_ for saving a screenshot image to MSO-3014's USB drive.
            File.WriteAllBytes($@"{PathPC}", USB_Session.RawIO.Read()); // Read HARDCopy image from MSO-3014's USB drive, & Save HARDCopy image to PC, overwriting any existing file without warning.
            OperationCompleteQuery();
        }

        public Boolean SetupExists(SETUPS Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            WriteString($":{Setup}:LABEL?");
            return ReadString().Trim().Trim('"').Equals(LabelString);
        }

        public void SetupLoad(SETUPS Setup, String LabelString) {
            if (!SetupExists(Setup, LabelString)) throw new ArgumentException($"MSO-3014 {Setup} labled '{LabelString}' non-existent!");
            WriteString($":RECAll:SETUp {(Int32)Setup}");
        }

        public void SetupLoad(String SetupFilePath) {
            if (!File.Exists(SetupFilePath)) throw new FileNotFoundException($"MSO-3014 Setup file not found at path '{SetupFilePath}'!");
            foreach (String mso_3014_SCPI_Command in File.ReadLines(SetupFilePath)) {
                WriteString(mso_3014_SCPI_Command);
                OperationCompleteQuery();
            }
        }

        public void SetupSave(SETUPS Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            WriteString($":{Setup}:LABEL \"{LabelString}\"");
            OperationCompleteQuery();
            WriteString($":{Setup}:LABEL?");
            String labelRead = ReadString().Trim().Trim('"');
            if (!labelRead.Equals(LabelString)) throw new ArgumentException($"MSO-3014 {Setup} not labeled correctly!{Environment.NewLine}  Should be '{LabelString}'.{Environment.NewLine}  Is '{labelRead}'.");
        }

        public Boolean ValidFileCharacters(String FileString) { return FileString.All(new HashSet<Char>(ValidCharactersFile.ToCharArray()).Contains); }

        public Boolean ValidFilel(String FileString) { return ((FileString.Length < 125) && ValidFileCharacters(FileString)); }

        public Boolean ValidLabelCharacters(String LabelString) { return LabelString.All(new HashSet<Char>(ValidCharactersLabel.ToCharArray()).Contains); }

        public Boolean ValidLabel(String LabelString) { return ((LabelString.Length < 30) && ValidLabelCharacters(LabelString)); }

        private String InvalidLabelMessage(String LabelString) { return $"Invalid MSO-3014 Setup label '{LabelString}'!{Environment.NewLine}  Label cannot exceed 30 characters in length and can only contain characters in set \"{ValidCharactersLabel}\"."; }

        ~MSO_3014_IVI_COM() { Dispose(false); }

        // public override void Close() { Dispose(); } NOTE: Overriding Close() causes TestLib.GetDerivedClassnames<> to throw 'System.Reflection.ReflectionTypeLoadException' in mscorlib.dll.
        // However, Types extending from COM objects should override all methods of an interface implemented by the base COM class.

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing) {
            if (!_disposed) {
                if (disposing) USB_Session.Dispose(); // Free managed resources specific to MSO_3014_IVI_COM.
                base.Close();                         // Free unmanaged resources specific to MSO_3014_IVI_COM; invoke Tkdpo2k3k4kClass.Close().
                _disposed = true;                     // Can only invoke Dispose(Boolean disposing) once and thus only base.Close() once, as is required. 
            }
        }
    }
}