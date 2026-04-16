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
                case QUERY.ASTS:
                case QUERY.FAULT:
                case QUERY.STS:
                case QUERY.UNMASK:
                    return (T)(Object)Convert<ASTS>(response);
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

        public (Double AmpsDC, Double VoltsDC) Get(DC DC) { return (Query<Double>(QUERY.ISET), Query<Double>(QUERY.VSET)); }

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

        public Sorensen_XFR_GPIB(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY_DC) {
            Command(COMMAND.CLR);
            ResetCommand();
            SetOff(VoltsDC: 0, AmpsDC: 0, OVP: Query<Double>(QUERY.VMAX));
        }
    }
}