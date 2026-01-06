using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Keysight.Visa;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_VISA_NET : IInstrument, IDiagnostics, IDisposable, IVISA_NET {
        public String Address { get; }
        public String Detail { get; }
        public UsbSession UsbSession { get; }
        public enum BUSES { B1, B2 }
        public enum CHANNELS { CH1, CH2 }
        public enum DRIVES_USB { E, F }
        public INSTRUMENT_TYPES InstrumentType { get; }
        public enum SETUPS { SETUP1 = 1, SETUP2 = 2, SETUP3 = 3, SETUP4 = 4, SETUP5 = 5, SETUP6 = 6, SETUP7 = 7, SETUP8 = 8, SETUP9 = 9, SETUP10 = 10 }
        public readonly static String ValidCharactersFile = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=+-!@#$%^&()[]{}~‘’,";
        public readonly static String ValidCharactersLabel = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=≠+-±!@#$%^&*()[]{}<>/~‘’\"\\|:,.?µ∞∆°Ωσ";
        private Boolean _disposed = false;
        private readonly Object _lock = new Object();

        public void ResetClear() { UsbSession.FormattedIO.WriteLine("*RST;*CLR"); }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 selfTestResult;
            try {
                selfTestResult = Int32.Parse(QueryLine("*TST?"));
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)selfTestResult;
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

        public MSO_3014_VISA_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.OSCILLOSCOPE_MIXED_SIGNAL;
            UsbSession = new UsbSession(Address) {
                TerminationCharacter = 0x0A,
                TerminationCharacterEnabled = true
            };
            DateTime dateTime = DateTime.Now;
            UsbSession.FormattedIO.WriteLine($":TIME \"{dateTime:hh:mm:ss}\"");
            UsbSession.FormattedIO.WriteLine($":DATE \"{dateTime:yyyy-MM-dd}\"");
            UsbSession.FormattedIO.WriteLine("DISplay:CLOCk ON");
        }

        public String QueryLine(String scpiCommand) {
            lock (_lock) {
                UsbSession.TerminationCharacterEnabled = true;
                UsbSession.FormattedIO.WriteLine(scpiCommand);
                return UsbSession.FormattedIO.ReadLine().Trim();
            }
        }

        public Byte[] QueryBinaryBlockOfByte(String scpiCommand) {
            lock (_lock) {
                UsbSession.TerminationCharacterEnabled = false;
                UsbSession.FormattedIO.WriteLine(scpiCommand);
                return UsbSession.FormattedIO.ReadBinaryBlockOfByte();
            }
        }

        public Byte[] QueryRawIO(String scpiCommand) {
            lock (_lock) {
                UsbSession.TerminationCharacterEnabled = false;
                UsbSession.FormattedIO.WriteLine(scpiCommand);
                return UsbSession.RawIO.Read();
            }
        }

        public void OperationCompleteQuery(String scpiCommand) { if (!QueryLine("*OPC?").Equals("1")) throw new InvalidOperationException($"{Detail}, Address '{Address}' didn't complete SCPI command '{scpiCommand}'!"); }

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
        }

        public void EventTableSave(BUSES Bus, DRIVES_USB Drive_USB, String PathPC) {
            String pathMSO_3014 = $"\"{Drive_USB}:/{Bus}.csv\"";
            UsbSession.FormattedIO.WriteLine($":SAVe:EVENTtable:{Bus} {pathMSO_3014}"); // Save Event Table to MSO-3014 USB drive, overwriting any existing file without warning.  Can't HARDCopy Event Tables, sadly.
            Thread.Sleep(500);                                                          // USB Drive write latency.

            File.WriteAllBytes($@"{PathPC}", QueryRawIO($":FILESystem:READFile {pathMSO_3014}")); // Read Event Table from MSO - 3014 USB drive & save to PC, overwriting any existing file without warning.
            UsbSession.FormattedIO.WriteLine($":FILESystem:DELEte {pathMSO_3014}");               // Delete Event Table from MSO-3014 USB drive.
        }

        public void ImageLandscapePNG_Save(String PathPC) {
            UsbSession.FormattedIO.WriteLine(":SAVe:IMAGe:INKSaver OFF");
            UsbSession.FormattedIO.WriteLine(":SAVe:IMAGe:LAYout LANdscape");
            UsbSession.FormattedIO.WriteLine(":SAVe:IMAGe:FILEFormat PNG");
            File.WriteAllBytes($@"{PathPC}", QueryRawIO(":HARDCopy STARt")); // ":HARDCopy STARt" is ostensibly a printing command, but actually works _best_ for fetching a screenshot image.  Save to PC, overwriting any existing file without warning.
        }

        public Boolean SetupExists(SETUPS Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            return QueryLine($":{Setup}:LABEL?").Equals(LabelString);
        }

        public void SetupLoad(SETUPS Setup, String LabelString) {
            if (!SetupExists(Setup, LabelString)) throw new ArgumentException($"MSO-3014 {Setup} labled '{LabelString}' non-existent!");
            UsbSession.FormattedIO.WriteLine($":RECAll:SETUp {(Int32)Setup}");
        }

        public void SetupLoad(String SetupFilePath) {
            if (!File.Exists(SetupFilePath)) throw new FileNotFoundException($"MSO-3014 Setup file not found at path '{SetupFilePath}'!");
            foreach (String mso_3014_SCPI_Command in File.ReadLines(SetupFilePath)) UsbSession.FormattedIO.WriteLine(mso_3014_SCPI_Command);
        }

        public void SetupSave(SETUPS Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            UsbSession.FormattedIO.WriteLine($":{Setup}:LABEL \"{LabelString}\"");
            String labelRead = QueryLine($":{Setup}:LABEL?");
            if (!labelRead.Equals(LabelString)) throw new ArgumentException($"MSO-3014 {Setup} not labeled correctly!{Environment.NewLine}  Should be '{LabelString}'.{Environment.NewLine}  Is '{labelRead}'.");
        }

        public Boolean ValidFileCharacters(String FileString) { return FileString.All(new HashSet<Char>(ValidCharactersFile.ToCharArray()).Contains); }

        public Boolean ValidFilel(String FileString) { return ((FileString.Length < 125) && ValidFileCharacters(FileString)); }

        public Boolean ValidLabelCharacters(String LabelString) { return LabelString.All(new HashSet<Char>(ValidCharactersLabel.ToCharArray()).Contains); }

        public Boolean ValidLabel(String LabelString) { return ((LabelString.Length < 30) && ValidLabelCharacters(LabelString)); }

        private String InvalidLabelMessage(String LabelString) { return $"Invalid MSO-3014 Setup label '{LabelString}'!{Environment.NewLine}  Label cannot exceed 30 characters in length and can only contain characters in set \"{ValidCharactersLabel}\"."; }

        ~MSO_3014_VISA_NET() { Dispose(false); }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing) {
            if (!_disposed) {
                if (disposing) {
                    // Free managed resources here
                }
                // Free unmanaged resources here (if any).
                UsbSession.Dispose();
                _disposed = true;
            }
        }
    }
}