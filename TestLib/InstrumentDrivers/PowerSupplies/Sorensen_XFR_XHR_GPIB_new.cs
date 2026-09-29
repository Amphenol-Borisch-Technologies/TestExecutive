using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using Microsoft.VisualBasic.Devices;
using System;
using System.Threading;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class Sorensen_XFR_XHR_GPIB_new : ScpiInstrument, IPowerSupplyDC_Outputs1 {
        [Flags]
        public enum ASTS { NONE = 0, CV = 1, CC = 2, unused = 4, OV = 8, OT = 16, SD = 32, FOLD = 64, ERR = 128, PON = 256, REM = 512, ACF = 1024, OPF = 2048, SNSP = 4096, ALL = 8191 }
        public enum FOLD { OFF = 0, CV = 1, CC = 2 }
        public enum COMMAND { AUXA, AUXB, CLR, DLY, FOLD, HOLD, IMAX, ISET, MASK, OUT, OVSET, RST, SRQ, TRG, UNMASK, VMAX, VSET }
        public enum QUERY { ASTS, AUXA, AUXB, DLY, ERR, FAULT, FOLD, HOLD, ID, IMAX, IOUT, ISET, OUT, OVSET, ROM, SRQ, STS, UNMASK, VMAX, VOUT, VSET }

        private readonly ScpiCommandRegistry<COMMAND> _commands;
        private readonly ScpiQueryRegistry<QUERY> _queries;

        public Sorensen_XFR_XHR_GPIB_new(String address, String detail) : base(address, detail, INSTRUMENT_TYPE.POWER_SUPPLY_DC) {
            _commands = new ScpiCommandRegistry<COMMAND>(this)
                .Map(COMMAND.CLR, () => Write("CLR"))
                .Map(COMMAND.RST, () => Write("RST"))
                .Map(COMMAND.TRG, () => Write("TRG"))
                .Map(COMMAND.AUXA, arg => Write("AUXA", arg))
                .Map(COMMAND.AUXB, arg => Write("AUXB", arg))
                .Map(COMMAND.OUT, arg => Write("OUT", arg))
                .Map(COMMAND.SRQ, arg => Write("SRQ", arg))
                .Map(COMMAND.HOLD, arg => Write("HOLD", arg))
                .Map(COMMAND.IMAX, arg => Write("IMAX", arg))
                .Map(COMMAND.ISET, arg => Write("ISET", arg))
                .Map(COMMAND.VMAX, arg => Write("VMAX", arg))
                .Map(COMMAND.VSET, arg => Write("VSET", arg))
                .Map(COMMAND.OVSET, arg => Write("OVSET", arg))
                .Map(COMMAND.DLY, arg => Write("DLY", arg))
                .Map(COMMAND.FOLD, arg => Write("FOLD", arg))
                .Map(COMMAND.MASK, arg => Write("MASK", arg))
                .Map(COMMAND.UNMASK, arg => Write("UNMASK", arg))
                .ValidateAll();

            _queries = new ScpiQueryRegistry<QUERY>(this)
                .Map<Int32>(QUERY.ASTS, () => Read<Int32>("ASTS"))
                .Map<Int32>(QUERY.FAULT, () => Read<Int32>("FAULT"))
                .Map<Int32>(QUERY.STS, () => Read<Int32>("STS"))
                .Map<Int32>(QUERY.UNMASK, () => Read<Int32>("UNMASK"))
                .Map<Int32>(QUERY.ERR, () => Read<Byte>("ERR"))
                .Map<Double>(QUERY.DLY, () => Read<Double>("DLY"))
                .Map<Double>(QUERY.IMAX, () => Read<Double>("IMAX"))
                .Map<Double>(QUERY.IOUT, () => Read<Double>("IOUT"))
                .Map<Double>(QUERY.ISET, () => Read<Double>("ISET"))
                .Map<Double>(QUERY.OVSET, () => Read<Double>("OVSET"))
                .Map<Double>(QUERY.VMAX, () => Read<Double>("VMAX"))
                .Map<Double>(QUERY.VOUT, () => Read<Double>("VOUT"))
                .Map<Double>(QUERY.VSET, () => Read<Double>("VSET"))
                .Map<STATE>(QUERY.AUXA, () => Read<STATE>("AUXA"))
                .Map<STATE>(QUERY.AUXB, () => Read<STATE>("AUXB"))
                .Map<STATE>(QUERY.HOLD, () => Read<STATE>("HOLD"))
                .Map<STATE>(QUERY.OUT, () => Read<STATE>("OUT"))
                .Map<STATE>(QUERY.SRQ, () => Read<STATE>("SRQ"))
                .Map<FOLD>(QUERY.FOLD, () => Read<FOLD>("FOLD"))
                .Map<String>(QUERY.ID, () => Read<String>("ID"))
                .Map<String>(QUERY.ROM, () => Read<String>("ROM"))
                .ValidateAll();

            ResetCommand();
        }

        public void Command(COMMAND Command, String Arguments = "") => _commands.Invoke(Command, Arguments);
        public T Query<T>(QUERY Query) => _queries.Invoke<T>(Query);

        public void OutputsOff() => _commands.Invoke(COMMAND.OUT, ((Int32)STATE.off).ToString());

        public (Double VoltsDC, Double AmperesDC) Get() => (Query<Double>(QUERY.VSET), _queries.Invoke<Double>(QUERY.ISET));

        public void SetOff(Double VoltsDC, Double AmperesDC, Double OVP) {
            StateSet(STATE.off, 0);
            _commands.Invoke(COMMAND.OVSET, OVP.ToString());
            _commands.Invoke(COMMAND.VSET, VoltsDC.ToString());
            _commands.Invoke(COMMAND.ISET, AmperesDC.ToString());
        }

        public void SetOffOn(Double VoltsDC, Double AmperesDC, Double OVP, Int32 MillisecondsDelay = 500) {
            SetOff(VoltsDC, AmperesDC, OVP);
            StateSet(STATE.ON, MillisecondsDelay);
        }

        public STATE StateGet() => _queries.Invoke<STATE>(QUERY.OUT);

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            _commands.Invoke(COMMAND.OUT, ((Int32)State).ToString());
            Thread.Sleep(MillisecondsDelay);
        }

        public override void ResetCommand() {
            ThrowIfDisposed();
            _ = _queries.Invoke<Byte>(QUERY.ERR); // Clear any existing errors in the error queue.  Don't care what the error is, just want to clear it.
            _commands.Invoke(COMMAND.CLR);
            _commands.Invoke(COMMAND.RST);
            SetOff(VoltsDC: 0, AmperesDC: 0, OVP: _queries.Invoke<Double>(QUERY.VMAX));
        }
    }
}