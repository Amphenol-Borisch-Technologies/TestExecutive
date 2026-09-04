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

        public Sorensen_XFR_XHR_GPIB_new(String address, String detail)
            : base(address, detail, INSTRUMENT_TYPE.POWER_SUPPLY_DC) {
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
        }

        // --------------------------------------------------------------------
        // PUBLIC API (same as your original, but simplified)
        // --------------------------------------------------------------------

        public void Command(COMMAND cmd, String arg = "") => _commands.Invoke(cmd, arg);
        public T Query<T>(QUERY q) => _queries.Invoke<T>(q);

        public void OutputsOff() => _commands.Invoke(COMMAND.OUT, STATE.off.ToString());

        public (Double AmpsDC, Double VoltsDC) Get() => (Query<Double>(QUERY.ISET), _queries.Invoke<Double>(QUERY.VSET));

        public void SetOff(Double volts, Double amps, Double ovp) {
            StateSet(STATE.off, 0);
            _commands.Invoke(COMMAND.OVSET, ovp.ToString());
            _commands.Invoke(COMMAND.VSET, volts.ToString());
            _commands.Invoke(COMMAND.ISET, amps.ToString());
        }

        public void SetOffOn(Double volts, Double amps, Double ovp, Int32 delayMs = 500) {
            SetOff(volts, amps, ovp);
            StateSet(STATE.ON, delayMs);
        }

        public STATE StateGet() => _queries.Invoke<STATE>(QUERY.OUT);

        public void StateSet(STATE state, Int32 delayMs = 500) {
            _commands.Invoke(COMMAND.OUT, state.ToString());
            Thread.Sleep(delayMs);
        }

        public new void ResetCommand() {
            _commands.Invoke(COMMAND.CLR);
            _commands.Invoke(COMMAND.RST);
            SetOff(0, 0, _queries.Invoke<Double>(QUERY.VMAX));
        }
    }
}