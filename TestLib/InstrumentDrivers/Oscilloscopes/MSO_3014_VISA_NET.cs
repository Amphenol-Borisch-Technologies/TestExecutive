using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Generic;
using Keysight.Visa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_VISA_NET : VISA_NET, IInstrument, IDiagnostics, IDisposable, IVISA_NET {
        public enum BUS { B1, B2 }
        public enum CHANNEL { CH1, CH2 }
        public enum DRIVE_USB { E, F }
        public enum SETUP { SETUP1 = 1, SETUP2 = 2, SETUP3 = 3, SETUP4 = 4, SETUP5 = 5, SETUP6 = 6, SETUP7 = 7, SETUP8 = 8, SETUP9 = 9, SETUP10 = 10 }
        public readonly static String ValidCharactersFile = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=+-!@#$%^&()[]{}~‘’,";
        public readonly static String ValidCharactersLabel = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=≠+-±!@#$%^&*()[]{}<>/~‘’\"\\|:,.?µ∞∆°Ωσ";

        public MSO_3014_VISA_NET(String Address, String Detail) : base(Address, Detail) {
            InstrumentType = INSTRUMENT_TYPE.OSCILLOSCOPE_MIXED_SIGNAL;
            DateTime dateTime = DateTime.Now;
            UsbSession.FormattedIO.WriteLine($":TIME \"{dateTime:hh:mm:ss}\"");
            UsbSession.FormattedIO.WriteLine($":DATE \"{dateTime:yyyy-MM-dd}\"");
            UsbSession.FormattedIO.WriteLine("DISplay:CLOCk ON");
        }

        public void EventTableEnable(BUS Bus) {
            switch (Bus) {
                case BUS.B1:
                    UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS B1;:*WAI");
                    break;
                case BUS.B2:
                    UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS B2;:*WAI");
                    break;
                default:
                    throw new NotImplementedException(NotImplementedMessageEnum<BUS>(Enum.GetName(typeof(BUS), Bus)));
            }
            UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS BMENU7;:*WAI");
            UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS RMENU1;:*WAI");
            UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS MENUOff;:*WAI");
            UsbSession.FormattedIO.WriteLine(":FPAnel:PRESS MENUOff;:*WAI");
        }

        public void EventTableSave(BUS Bus, DRIVE_USB Drive_USB, String PathPC) {
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

        public Boolean SetupExists(SETUP Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            return QueryLine($":{Setup}:LABEL?").Equals(LabelString);
        }

        public void SetupLoad(SETUP Setup, String LabelString) {
            if (!SetupExists(Setup, LabelString)) throw new ArgumentException($"MSO-3014 {Setup} labled '{LabelString}' non-existent!");
            UsbSession.FormattedIO.WriteLine($":RECAll:SETUp {(Int32)Setup}");
        }

        public void SetupLoad(String SetupFilePath) {
            if (!File.Exists(SetupFilePath)) throw new FileNotFoundException($"MSO-3014 Setup file not found at path '{SetupFilePath}'!");
            foreach (String mso_3014_SCPI_Command in File.ReadLines(SetupFilePath)) UsbSession.FormattedIO.WriteLine(mso_3014_SCPI_Command);
        }

        public void SetupSave(SETUP Setup, String LabelString) {
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
    }
}