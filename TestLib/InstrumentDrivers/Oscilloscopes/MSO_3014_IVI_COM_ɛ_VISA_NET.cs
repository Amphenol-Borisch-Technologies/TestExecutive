using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Ivi.Visa;
using Keysight.Visa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Tektronix.Tkdpo2k3k4k.Interop;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_IVI_COM_ɛ_VISA_NET : IInstrument, IDiagnostics, IDisposable {
        public String Address { get; }
        public String Detail { get; }
        public Tkdpo2k3k4kClass Tkdpo2k3k4kClass { get; }
        public UsbSession UsbSession { get; }
        public enum BUSES { B1, B2 }
        public enum DRIVES_USB { E, F }
        public INSTRUMENT_TYPES InstrumentType { get; }
        public enum SETUPS { SETUP1 = 1, SETUP2 = 2, SETUP3 = 3, SETUP4 = 4, SETUP5 = 5, SETUP6 = 6, SETUP7 = 7, SETUP8 = 8, SETUP9 = 9, SETUP10 = 10 }
        public readonly static String ValidCharactersFile = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=+-!@#$%^&()[]{}~‘’,";
        public readonly static String ValidCharactersLabel = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=≠+-±!@#$%^&*()[]{}<>/~‘’\"\\|:,.?µ∞∆°Ωσ";
        private Boolean _disposed = false;

        public void ResetClear() { Tkdpo2k3k4kClass.Reset(); }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 TestResult = 0;
            String TestMessage = String.Empty;
            try {
                Tkdpo2k3k4kClass.UtilityEx.SelfTest(ref TestResult, ref TestMessage);
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

        public MSO_3014_IVI_COM_ɛ_VISA_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.OSCILLOSCOPE_MIXED_SIGNAL;
            Tkdpo2k3k4kClass = new Tkdpo2k3k4kClass();
            Tkdpo2k3k4kClass.Initialize(ResourceName: Address, IdQuery: false, Reset: false, OptionString: String.Empty);
            UsbSession = new UsbSession(Address, AccessModes.None, timeoutMilliseconds: 20000);
            DateTime dateTime = DateTime.Now;
            UsbSession.FormattedIO.WriteLine($":TIME \"{dateTime:hh:mm:ss}\"");
            UsbSession.FormattedIO.WriteLine($":DATE \"{dateTime:yyyy-MM-dd}\"");
        }

        public void OperationCompleteQuery() {
            UsbSession.FormattedIO.WriteLine("*OPC?");
            if (UsbSession.FormattedIO.ReadLine().Trim().Trim('"') != "1") throw new InvalidOperationException("MSO-3014 didn't complete SCPI command!");
        }

        public void EventTableEnable(BUSES Bus) {
            switch (Bus) {
                case BUSES.B1:
                    UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS B1;:*WAI");
                    break;
                case BUSES.B2:
                    UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS B2;:*WAI");
                    break;
                default:
                    throw new NotImplementedException(NotImplementedMessageEnum<BUSES>(Enum.GetName(typeof(BUSES), Bus)));
            }
            UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS BMENU7;:*WAI");
            UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS RMENU1;:*WAI");
            UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS MENUOff;:*WAI");
            UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS MENUOff;:*WAI");
            OperationCompleteQuery();
        }

        public void EventTableSave(BUSES Bus, DRIVES_USB Drive_USB, String PathPC) {
            String pathMSO_3014 = $"\"{Drive_USB}:/{Bus}.csv\"";
            UsbSession.FormattedIO.WriteLine($"SAVe:EVENTtable:{Bus} {pathMSO_3014}"); // Save Event Table to MSO-3014 USB drive, overwriting any existing file without warning.  Can't HARDCopy Event Tables, sadly.
            OperationCompleteQuery();
            Thread.Sleep(500);                                                          // USB Drive write latency.

            UsbSession.FormattedIO.WriteLine($"FILESystem:READFile {pathMSO_3014}");   // Read Event Table from MSO-3014 USB drive.
            File.WriteAllBytes($@"{PathPC}", UsbSession.RawIO.Read());                 // Save read Event Table to PC, overwriting any existing file without warning.
            OperationCompleteQuery();

            UsbSession.FormattedIO.WriteLine($"FILESystem:DELEte {pathMSO_3014}");     // Delete Event Table from MSO-3014 USB drive.
            OperationCompleteQuery();
        }

        public void ImageLandscapePNG_Save(String PathPC) {
            UsbSession.FormattedIO.WriteLine("SAVe:IMAGe:INKSaver OFF");
            UsbSession.FormattedIO.WriteLine("SAVe:IMAGe:LAYout LANdscape");
            UsbSession.FormattedIO.WriteLine("SAVe:IMAGe:FILEFormat PNG");
            OperationCompleteQuery();
            UsbSession.FormattedIO.WriteLine("HARDCopy STARt");        // Ostensibly a printing command, actually works _best_ for saving a screenshot image to MSO-3014's USB drive.
            File.WriteAllBytes($@"{PathPC}", UsbSession.RawIO.Read()); // Read HARDCopy image from MSO-3014's USB drive, & Save HARDCopy image to PC, overwriting any existing file without warning.
            OperationCompleteQuery();
        }

        public Boolean SetupExists(SETUPS Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            UsbSession.FormattedIO.WriteLine($":{Setup}:LABEL?");
            return UsbSession.FormattedIO.ReadLine().Trim().Trim('"').Equals(LabelString);
        }

        public void SetupLoad(SETUPS Setup, String LabelString) {
            if (!SetupExists(Setup, LabelString)) throw new ArgumentException($"MSO-3014 {Setup} labled '{LabelString}' non-existent!");
            UsbSession.FormattedIO.WriteLine($":RECAll:SETUp {(Int32)Setup}");
        }

        public void SetupLoad(String SetupFilePath) {
            if (!File.Exists(SetupFilePath)) throw new FileNotFoundException($"MSO-3014 Setup file not found at path '{SetupFilePath}'!");
            foreach (String mso_3014_SCPI_Command in File.ReadLines(SetupFilePath)) {
                UsbSession.FormattedIO.WriteLine(mso_3014_SCPI_Command);
                OperationCompleteQuery();
            }
        }

        public void SetupSave(SETUPS Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            UsbSession.FormattedIO.WriteLine($":{Setup}:LABEL \"{LabelString}\"");
            OperationCompleteQuery();
            UsbSession.FormattedIO.WriteLine($":{Setup}:LABEL?");
            String labelRead = UsbSession.FormattedIO.ReadLine().Trim().Trim('"');
            if (!labelRead.Equals(LabelString)) throw new ArgumentException($"MSO-3014 {Setup} not labeled correctly!{Environment.NewLine}  Should be '{LabelString}'.{Environment.NewLine}  Is '{labelRead}'.");
        }

        public Boolean ValidFileCharacters(String FileString) { return FileString.All(new HashSet<Char>(ValidCharactersFile.ToCharArray()).Contains); }

        public Boolean ValidFilel(String FileString) { return ((FileString.Length < 125) && ValidFileCharacters(FileString)); }

        public Boolean ValidLabelCharacters(String LabelString) { return LabelString.All(new HashSet<Char>(ValidCharactersLabel.ToCharArray()).Contains); }

        public Boolean ValidLabel(String LabelString) { return ((LabelString.Length < 30) && ValidLabelCharacters(LabelString)); }

        private String InvalidLabelMessage(String LabelString) { return $"Invalid MSO-3014 Setup label '{LabelString}'!{Environment.NewLine}  Label cannot exceed 30 characters in length and can only contain characters in set \"{ValidCharactersLabel}\"."; }

        ~MSO_3014_IVI_COM_ɛ_VISA_NET() { Dispose(false); }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing) {
            if (!_disposed) {
                if (disposing) {
                    // Free managed resources here
                    UsbSession.Dispose();
                }
                // Free unmanaged resources here (if any).
                Tkdpo2k3k4kClass.Close();
                _disposed = true;
            }
        }
    }
}