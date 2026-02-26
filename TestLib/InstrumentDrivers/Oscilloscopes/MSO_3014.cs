using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014 : Instrument {
        public enum BUS { B1, B2 }
        public enum CHANNEL { CH1, CH2 }
        public enum DRIVE_USB { E, F }
        public enum SETUP { SETUP1 = 1, SETUP2 = 2, SETUP3 = 3, SETUP4 = 4, SETUP5 = 5, SETUP6 = 6, SETUP7 = 7, SETUP8 = 8, SETUP9 = 9, SETUP10 = 10 }
        public readonly static String ValidCharactersFile = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=+-!@#$%^&()[]{}~‘’,";
        public readonly static String ValidCharactersLabel = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._=≠+-±!@#$%^&*()[]{}<>/~‘’\"\\|:,.?µ∞∆°Ωσ";

        public MSO_3014(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.OSCILLOSCOPE_MIXED_SIGNAL) {
            DateTime dateTime = DateTime.Now;
            Command($":TIME \"{dateTime:hh:mm:ss}\"");
            Command($":DATE \"{dateTime:yyyy-MM-dd}\"");
            Command("DISplay:CLOCk ON");
        }

        public void EventTableEnable(BUS Bus) {
            switch (Bus) {
                case BUS.B1:
                    Command(":FPAnel:PRESS B1;:*WAI");
                    break;
                case BUS.B2:
                    Command(":FPAnel:PRESS B2;:*WAI");
                    break;
                default:
                    throw new NotImplementedException(NotImplementedMessageEnum<BUS>(Enum.GetName(typeof(BUS), Bus)));
            }
            Command(":FPAnel:PRESS BMENU7;:*WAI");
            Command(":FPAnel:PRESS RMENU1;:*WAI");
            Command(":FPAnel:PRESS MENUOff;:*WAI");
            Command(":FPAnel:PRESS MENUOff;:*WAI");
        }

        public void EventTableSave(BUS Bus, DRIVE_USB Drive_USB, String PathPC) {
            String pathMSO_3014 = $"\"{Drive_USB}:/{Bus}.csv\"";
            Command($":SAVe:EVENTtable:{Bus} {pathMSO_3014}"); // Save Event Table to MSO-3014 USB drive, overwriting any existing file without warning.  Can't HARDCopy Event Tables, sadly.
            Thread.Sleep(500);                                                          // USB Drive write latency.

            File.WriteAllBytes($@"{PathPC}", QueryRawIO($":FILESystem:READFile {pathMSO_3014}")); // Read Event Table from MSO - 3014 USB drive & save to PC, overwriting any existing file without warning.
            Command($":FILESystem:DELEte {pathMSO_3014}");               // Delete Event Table from MSO-3014 USB drive.
        }

        public void ImageLandscapePNG_Save(String PathPC) {
            Command(":SAVe:IMAGe:INKSaver OFF");
            Command(":SAVe:IMAGe:LAYout LANdscape");
            Command(":SAVe:IMAGe:FILEFormat PNG");
            File.WriteAllBytes($@"{PathPC}", QueryRawIO(":HARDCopy STARt")); // ":HARDCopy STARt" is ostensibly a printing command, but actually works _best_ for fetching a screenshot image.  Save to PC, overwriting any existing file without warning.
        }

        public Boolean SetupExists(SETUP Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            return Query($":{Setup}:LABEL?").Equals(LabelString);
        }

        public void SetupLoad(SETUP Setup, String LabelString) {
            if (!SetupExists(Setup, LabelString)) throw new ArgumentException($"MSO-3014 {Setup} labled '{LabelString}' non-existent!");
            Command($":RECAll:SETUp {(Int32)Setup}");
        }

        public void SetupLoad(String SetupFilePath) {
            if (!File.Exists(SetupFilePath)) throw new FileNotFoundException($"MSO-3014 Setup file not found at path '{SetupFilePath}'!");
            foreach (String mso_3014_SCPI_Command in File.ReadLines(SetupFilePath)) Command(mso_3014_SCPI_Command);
        }

        public void SetupSave(SETUP Setup, String LabelString) {
            if (!ValidLabel(LabelString)) throw new ArgumentException(InvalidLabelMessage(LabelString));
            Command($":{Setup}:LABEL \"{LabelString}\"");
            String labelRead = Query($":{Setup}:LABEL?");
            if (!labelRead.Equals(LabelString)) throw new ArgumentException($"MSO-3014 {Setup} not labeled correctly!{Environment.NewLine}  Should be '{LabelString}'.{Environment.NewLine}  Is '{labelRead}'.");
        }

        public Boolean ValidFileCharacters(String FileString) { return FileString.All(new HashSet<Char>(ValidCharactersFile.ToCharArray()).Contains); }

        public Boolean ValidFilel(String FileString) { return ((FileString.Length < 125) && ValidFileCharacters(FileString)); }

        public Boolean ValidLabelCharacters(String LabelString) { return LabelString.All(new HashSet<Char>(ValidCharactersLabel.ToCharArray()).Contains); }

        public Boolean ValidLabel(String LabelString) { return ((LabelString.Length < 30) && ValidLabelCharacters(LabelString)); }

        private String InvalidLabelMessage(String LabelString) { return $"Invalid MSO-3014 Setup label '{LabelString}'!{Environment.NewLine}  Label cannot exceed 30 characters in length and can only contain characters in set \"{ValidCharactersLabel}\"."; }
    }
}