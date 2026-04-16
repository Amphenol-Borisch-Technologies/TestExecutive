using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using static ABT.Test.TestExecutive.TestLib.TestLib;
using System;
using System.Threading;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class Sorensen_XFR_GPIB : InstrumentDriver, IPowerSupplyDC_Outputs1 {
        [Flags] public enum ASTS : UInt16 { NONE = 0, CV = 1, CC = 2, unused = 4, OV = 8, OT = 16, SD = 32, FOLD = 64, ERR = 128, PON = 256, REM = 512, ACF = 1024, OPF = 2048, SNSP = 4096, ALL = 8191 }

        public enum FOLD { OFF=0, CV=1, CC=2 }

        public enum COMMAND { AUXA, AUXB, CLR, DLY, FOLD, HOLD, IMAX, ISET, MASK, OUT, OVSET, RST, SRQ, TRG, UNMASK, VMAX, VSET }

        public enum QUERY { ASTS, AUXA, AUXB, DLY, ERR, FAULT, FOLD, HOLD, ID, IMAX, IOUT, ISET, OUT, OVSET, ROM, SRQ, STS, UNMASK, VMAX, VOUT, VSET }

        public void OutputsOff() { StateSet(STATE.off, MillisecondsDelay: 0); }

        public (Double AmpsDC, Double VoltsDC) Get(DC DC) { return (Double.Parse(Query("ISET?")), Double.Parse(Query("VSET?"))); }

        public void Command(COMMAND Command, String arg="") {
            switch (Command) {
                case COMMAND.AUXA:
                case COMMAND.AUXB:
                case COMMAND.HOLD:
                case COMMAND.OUT:
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
                    break;
                default: throw new NotImplementedException(NotImplementedMessageEnum<COMMAND>(Enum.GetName(typeof(COMMAND), Command)));
            }
        }

        public T Query<T>(QUERY Query) {
            String response = base.Query($"{Query}?").Substring($"{Query} ".Length); // Response is in the format "QUERY value", so remove the "QUERY " part to get just the value.
            switch (Query) {
                case QUERY.ASTS:   return (T)(Object)Convert<ASTS>(response);
                case QUERY.AUXA:   return (T)(Object)Convert<STATE>(response);
                case QUERY.AUXB:   return (T)(Object)Convert<STATE>(response);
                case QUERY.DLY:    return (T)(Object)Convert<Double>(response);
                case QUERY.ERR:    return (T)(Object)Convert<Byte>(response);
                case QUERY.FAULT:  return (T)(Object)Convert<ASTS>(response);
                case QUERY.FOLD:   return (T)(Object)Convert<FOLD>(response);
                case QUERY.HOLD:   return (T)(Object)Convert<STATE>(response);
                case QUERY.ID:     return (T)(Object)Convert<String>(response);
                case QUERY.IMAX:   return (T)(Object)Convert<Double>(response);
                case QUERY.IOUT:   return (T)(Object)Convert<Double>(response);
                case QUERY.ISET:   return (T)(Object)Convert<Double>(response);
                case QUERY.OUT:    return (T)(Object)Convert<STATE>(response);
                case QUERY.OVSET:  return (T)(Object)Convert<Double>(response);
                case QUERY.ROM:    return (T)(Object)Convert<String>(response);
                case QUERY.SRQ:    return (T)(Object)Convert<STATE>(response);
                case QUERY.STS:    return (T)(Object)Convert<ASTS>(response);
                case QUERY.UNMASK: return (T)(Object)Convert<ASTS>(response);
                case QUERY.VMAX:   return (T)(Object)Convert<Double>(response);
                case QUERY.VOUT:   return (T)(Object)Convert<Double>(response);
                case QUERY.VSET:   return (T)(Object)Convert<Double>(response);
                default: throw new NotImplementedException(NotImplementedMessageEnum<QUERY>(Enum.GetName(typeof(QUERY), Query)));
            }
        }

        public void SetOff(Double VoltsDC, Double AmpsDC, Double OVP) {
            OutputsOff();
            Command($"OVSET {OVP}");
            Command($"VSET {VoltsDC}");
            Command($"ISET {AmpsDC}");
        }

        public void SetOffOn(Double VoltsDC, Double AmpsDC, Double OVP, Int32 MillisecondsDelay = 500) {
            SetOff(VoltsDC, AmpsDC, OVP);
            StateSet(STATE.ON, MillisecondsDelay);
        }

        public STATE StateGet() { return Query("OUT?") == "1" ? STATE.ON : STATE.off; }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            Command($"OUT {State}");
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.
        }

        public String ASTS_Query() { return ((ASTS)Query<UInt16>("ASTS?")).ToString(); }

        public void ClearCommand() { Command("CLR"); }

        public new void ResetCommand() { Command("RST"); }

        public Sorensen_XFR_GPIB(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY_DC) {
            ClearCommand();
            ResetCommand();
            SetOff(VoltsDC: 0, AmpsDC: 0, OVP: Query<Double>("VMAX?"));
        }
    }
}