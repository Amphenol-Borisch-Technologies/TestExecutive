using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Multifunction {

    public class MSMU_34980A : InstrumentDriver, IRelay {
        public readonly struct Modules {
            public static readonly String M34921A = "34921A";
            public static readonly String M34932A = "34932A";
            public static readonly String M34938A = "34938A";
            public static readonly String M34939A = "34939A";
            public static readonly String M34952A = "34952A";
        }
        public enum TEMPERATURE_UNIT { C, F, K }
        public enum RELAY_STATE { opened, CLOSED }
        public enum SLOT { S1 = 1, S2 = 2, S3 = 3, S4 = 4, S5 = 5, S6 = 6, S7 = 7, S8 = 8 }
        public static String GetSlot(SLOT Slot) { return $"SLOT{(Int32)Slot}"; }

        private readonly String _34980A;

        public void OpenAll() { Command(":ROUTe:OPEN:ALL"); }

        public MSMU_34980A(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.MULTI_FUNCTION) {
            DateTime now = DateTime.Now;
            Command($":SYSTem: DATE {now.Year},{now.Month},{now.Day}");
            Command($":SYSTem: TIME {now.Hour},{now.Minute},{Convert.ToDouble(now.Second)}");
            Command(":UNIT: TEMPerature F");
            _34980A = Identity(IDN_FIELD.Model);
        }

        public Boolean InstrumentDMM_Installed() { return Query(":INSTrument:DMM:INSTalled?") == "1"; }
        public STATE InstrumentDMM_Get() { return Query(":INSTrument:DMM:STATe?") == "1" ? STATE.ON : STATE.off; }
        public void InstrumentDMM_Set(STATE State) { Command($":INSTrument:DMM:STATe {State == STATE.ON}"); }
        public (Int32 Min, Int32 Max) ModuleChannels(SLOT Slot) {
            switch (SystemType(Slot)) {
                case String s when s == Modules.M34921A: return (Min: 1, Max: 44);
                case String s when s == Modules.M34939A: return (Min: 1, Max: 68);
                case String s when s == Modules.M34952A: return (Min: 1, Max: 7);
                default: throw new NotImplementedException(TestLib.NotImplementedMessageEnum<SLOT>(Enum.GetName(typeof(SLOT), Slot)));
            }
        }
        public void RouteCloseExclusive(String Channels) {
            ValidateChannelS(Channels);
            Command($":ROUTe:CLOSe:EXCLusive ({Channels})");
        }
        public Boolean RouteGet(String Channels, RELAY_STATE State) {
            ValidateChannelS(Channels);
            Boolean[] states = Query<Boolean[]>($":ROUTe: {(State is RELAY_STATE.opened ? "OPEN" : "CLOSe")} ? {Channels}");
            List<Boolean> lb = states.ToList();
            return lb.TrueForAll(b => b == true);
        }
        public void RouteSet(String Channels, RELAY_STATE State) {
            ValidateChannelS(Channels);
            if (State is RELAY_STATE.opened) Command($":ROUTe:OPEN {Channels}");
            else Command($":ROUTe:CLOSe {Channels}");
        }
        public String SystemDescriptionLong(SLOT Slot) { return Query($":SYSTem:CDEScription:LONG? {(Int32)Slot}"); }
        public Double SystemModuleTemperature(SLOT Slot) { return Double.Parse(Query($":SYSTem:MODule:TEMPerature? TRANsducer,{(Int32)Slot}")); }
        public String SystemType(SLOT Slot) { return Query($":SYSTem:CTYPe? {Slot}").Split(',')[(Int32)IDN_FIELD.Model]; }
        public TEMPERATURE_UNIT UnitsGet() { return (TEMPERATURE_UNIT)Enum.Parse(typeof(TEMPERATURE_UNIT), Query(":UNIT:TEMPerature?").Replace("[", "").Replace("]", "")); }
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
            if (!Enum.IsDefined(typeof(SLOT), (SLOT)slotNumber)) throw new ArgumentException($"{nameof(Channel)} '{Channel}' must have valid integer Slot in interval [{(Int32)SLOT.S1}..{(Int32)SLOT.S8}].");
            Int32 channel = Int32.Parse(Channel.Substring(2));
            (Int32 min, Int32 max) = ModuleChannels((SLOT)slotNumber);
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