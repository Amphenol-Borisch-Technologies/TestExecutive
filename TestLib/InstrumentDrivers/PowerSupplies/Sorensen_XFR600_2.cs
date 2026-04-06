using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Threading;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class Sorensen_XFR600_2 : InstrumentDriver, IPowerSupplyDC_Outputs1 {
        public void OutputsOff() { StateSet(STATE.off, MillisecondsDelay: 0); }

        public (Double AmpsDC, Double VoltsDC) Get(DC DC) { return (Double.Parse(Query("ISET?")), Double.Parse(Query("VSET?"))); }

        public void SetOffOn(Double VoltsDC, Double AmpsDC, Double OVP, Int32 MillisecondsDelay = 500) {
            OutputsOff();
            Command($"OVSET {OVP}");
            Command($"VSET {VoltsDC}");
            Command($"ISET {AmpsDC}");
            StateSet(STATE.ON, MillisecondsDelay);
        }

        public STATE StateGet() { return Query("OUT?") == "1" ? STATE.ON : STATE.off; }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            Command($"OUT {State}");
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.
        }

        public void ClearCommand() { Command("CLR"); }

        public new void ResetCommand() { Command("RST"); }

        public void RemoteEnableCommand(STATE State) { Command($"REN {State == STATE.ON}"); }

        public Sorensen_XFR600_2(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY_DC) {
            RemoteEnableCommand(STATE.ON);
            StateSet(STATE.off);
            ClearCommand();
            ResetCommand();
        }
    }
}