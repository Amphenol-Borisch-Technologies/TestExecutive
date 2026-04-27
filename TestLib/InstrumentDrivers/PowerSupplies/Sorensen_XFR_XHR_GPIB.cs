using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class Sorensen_XFR_XHR_GPIB : InstrumentDriver, IPowerSupplyDC_Outputs1 {
        // NOTE: Includes all non-calibration commands & queries defined in:
        // - "Sorensen Internal GPIB Interface for XHR/XFR Series Programmable DC Power Supplies Operation Manual GPIB-XHR, GPIB-XFR & GPIB-XFR3", TM-GPRF-01XN, February 2009 Revision B.
        // NOTE: Excludes all calibration commands & queries.

        [Flags] public enum ASTS { NONE = 0, CV = 1, CC = 2, unused = 4, OV = 8, OT = 16, SD = 32, FOLD = 64, ERR = 128, PON = 256, REM = 512, ACF = 1024, OPF = 2048, SNSP = 4096, ALL = 8191 }

        public enum FOLD { OFF = 0, CV = 1, CC = 2 }

        public enum COMMAND { AUXA, AUXB, CLR, DLY, FOLD, HOLD, IMAX, ISET, MASK, OUT, OVSET, RST, SRQ, TRG, UNMASK, VMAX, VSET }

        public enum QUERY { ASTS, AUXA, AUXB, DLY, ERR, FAULT, FOLD, HOLD, ID, IMAX, IOUT, ISET, OUT, OVSET, ROM, SRQ, STS, UNMASK, VMAX, VOUT, VSET }

        public void Command(COMMAND Command, String arg = "") {
            switch (Command) {
                case COMMAND.AUXA:
                case COMMAND.AUXB:
                case COMMAND.HOLD:
                case COMMAND.OUT:
                case COMMAND.SRQ:
                    base.Command($"{Command} {Enum.Parse(typeof(STATE), arg)}");
                    break;
                case COMMAND.CLR:
                case COMMAND.RST:
                case COMMAND.TRG:
                    base.Command($"{Command}");
                    break;
                case COMMAND.DLY:
                case COMMAND.IMAX:
                case COMMAND.ISET:
                case COMMAND.OVSET:
                case COMMAND.VMAX:
                case COMMAND.VSET:
                    base.Command($"{Command} {Double.Parse(arg)}");
                    break;
                case COMMAND.FOLD:
                    base.Command($"{Command} {Enum.Parse(typeof(FOLD), arg)}");
                    break;
                case COMMAND.MASK:
                case COMMAND.UNMASK:
                    base.Command($"{Command} {arg}");
                    break;
                default: throw new NotImplementedException(NotImplementedMessageEnum<COMMAND>(Enum.GetName(typeof(COMMAND), Command)));
            }
        }

        public static String ASTS_FlagsToMnemonics(Int32 ASTS_Flags) {
            if ((ASTS_Flags & ~(Int32)ASTS.ALL) != 0) throw new ArgumentOutOfRangeException(nameof(ASTS_Flags), $"Value {ASTS_Flags} contains bits not defined in {nameof(ASTS)}.");

            ASTS astsFlags = (ASTS)ASTS_Flags;
            if (astsFlags == ASTS.NONE || astsFlags == ASTS.ALL) return astsFlags.ToString(); // Special cases.
            if ((astsFlags & ASTS.unused) == ASTS.unused) throw new ArgumentException($"Value {ASTS_Flags} cannot have the {nameof(ASTS.unused)} flag set.", nameof(ASTS_Flags));
            if ((astsFlags & ASTS.ALL) == ASTS.ALL) throw new ArgumentException($"Value {ASTS_Flags} cannot have the {nameof(ASTS.ALL)} flag set.", nameof(ASTS_Flags));

            List<String> astsMnemonics = new List<String>();
            foreach (ASTS astsFlag in Enum.GetValues(typeof(ASTS))) {
                if (astsFlag == ASTS.NONE || astsFlag == ASTS.unused || astsFlag == ASTS.ALL) continue; // Skip special cases.
                if ((astsFlags & astsFlag) == astsFlag) astsMnemonics.Add(astsFlag.ToString());
            }
            return astsMnemonics.Count > 0 ? String.Join(", ", astsMnemonics) : nameof(ASTS.NONE);
        }

        public static String ASTS_MnemonicsToFlags(String ASTS_Mnemonics) {
            if (String.IsNullOrWhiteSpace(ASTS_Mnemonics)) throw new ArgumentException("Value cannot be null, empty, or whitespace.", nameof(ASTS_Mnemonics));
            ASTS_Mnemonics = ASTS_Mnemonics.ToUpper().Replace(" ", ","); // Allow either spaces or commas as separators, but convert spaces to commas for consistent splitting.
            if (ASTS_Mnemonics.Contains(nameof(ASTS.unused).ToUpper())) throw new ArgumentException($"Value {ASTS_Mnemonics} cannot contain the {nameof(ASTS.unused)} flag.", nameof(ASTS_Mnemonics));
            if (ASTS_Mnemonics.Contains(nameof(ASTS.NONE)) && !ASTS_Mnemonics.Equals(nameof(ASTS.NONE))) throw new ArgumentException($"Value {ASTS_Mnemonics} cannot contain the {nameof(ASTS.NONE)} flag.", nameof(ASTS_Mnemonics));
            if (ASTS_Mnemonics.Contains(nameof(ASTS.ALL)) && !ASTS_Mnemonics.Equals(nameof(ASTS.ALL))) throw new ArgumentException($"Value {ASTS_Mnemonics} cannot contain the {nameof(ASTS.ALL)} flag.", nameof(ASTS_Mnemonics));

            String[] astsMnemonics = ASTS_Mnemonics.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (astsMnemonics.Length == 1 && Enum.TryParse(astsMnemonics[0], out ASTS singleASTSFlag) && (singleASTSFlag == ASTS.NONE || singleASTSFlag == ASTS.ALL)) return singleASTSFlag.ToString(); // Special cases.
            Int32 astsFlags = 0;
            foreach (String astsMnemonic in astsMnemonics) {
                if (!Enum.TryParse(astsMnemonic, out ASTS astsFlag)) throw new ArgumentException($"Value {astsMnemonic} is not a valid {nameof(ASTS)} mnemonic.", nameof(ASTS_Mnemonics));
                astsFlags |= (Int32)astsFlag;
            }
            return astsFlags.ToString();
        }

        public T Query<T>(QUERY Query) {
            String response = base.Query($"{Query}?").Substring($"{Query} ".Length); // Response is in the format "QUERY value", so remove the "QUERY " part to get just the value.
            switch (Query) {
                case QUERY.ASTS:
                case QUERY.FAULT:
                case QUERY.STS:
                case QUERY.UNMASK:
                    if (typeof(T) == typeof(Int32)) return (T)(Object)Convert<Int32>(response);
                    if (typeof(T) == typeof(String)) return (T)(Object)ASTS_FlagsToMnemonics(Convert<Int32>(response));
                    throw new NotImplementedException($"Query {Query} cannot be converted to type {typeof(T).Name}.");
                case QUERY.AUXA:
                case QUERY.AUXB:
                case QUERY.HOLD:
                case QUERY.OUT:
                case QUERY.SRQ:
                    return (T)(Object)Convert<STATE>(response);
                case QUERY.DLY:
                case QUERY.IMAX:
                case QUERY.IOUT:
                case QUERY.ISET:
                case QUERY.OVSET:
                case QUERY.VMAX:
                case QUERY.VOUT:
                case QUERY.VSET:
                    return (T)(Object)Convert<Double>(response);
                case QUERY.ERR:
                    return (T)(Object)Convert<Byte>(response);
                case QUERY.FOLD:
                    return (T)(Object)Convert<FOLD>(response);
                case QUERY.ID:
                case QUERY.ROM:
                    return (T)(Object)Convert<String>(response);
                default: throw new NotImplementedException(NotImplementedMessageEnum<QUERY>(Enum.GetName(typeof(QUERY), Query)));
            }
        }

        public void SetOff(Double VoltsDC, Double AmpsDC, Double OVP) {
            StateSet(STATE.off, MillisecondsDelay: 0);
            Command(COMMAND.OVSET, OVP.ToString());
            Command(COMMAND.VSET, VoltsDC.ToString());
            Command(COMMAND.ISET, AmpsDC.ToString());
        }

        public void OutputsOff() { Command(COMMAND.OUT, STATE.off.ToString()); }

        public (Double AmpsDC, Double VoltsDC) Get() { return (Query<Double>(QUERY.ISET), Query<Double>(QUERY.VSET)); }

        public void SetOffOn(Double VoltsDC, Double AmpsDC, Double OVP, Int32 MillisecondsDelay = 500) {
            SetOff(VoltsDC, AmpsDC, OVP);
            StateSet(STATE.ON, MillisecondsDelay);
        }

        public STATE StateGet() { return Query<STATE>(QUERY.OUT); }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            Command(COMMAND.OUT, State.ToString());
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.
        }

        public new void ResetCommand() { Command(COMMAND.RST); }

        public Sorensen_XFR_XHR_GPIB(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY_DC) {
            Command(COMMAND.CLR);
            ResetCommand();
            SetOff(VoltsDC: 0, AmpsDC: 0, OVP: Query<Double>(QUERY.VMAX));
        }
    }
}