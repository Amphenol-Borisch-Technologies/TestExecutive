using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Threading;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class PS_E3634A : Instrument, IPowerSupplyOutputs1 {
        public enum RANGE { P25V, P50V }

        public void OutputsOff() { Command(":OUTPut:STATe 0"); }

        public RANGE RangeGet() { return (RANGE)Enum.Parse(typeof(RANGE), Query(":SOURce:VOLTage:RANGe?")); }
        public void RangeSet(RANGE Range) { Command($":SOURce:VOLTage:RANGe {Range}"); }

        public (Double AmpsDC, Double VoltsDC) Get(DC DC) { return (Double.Parse(Query(":MEASure:CURRent:DC?")), Double.Parse(Query(":MEASure:VOLTage:DC?"))); }

        public void SetOffOn(Double VoltsDC, Double AmpsDC, Double OVP) {
            Command(":OUTPut:STATe 0");
            Command(":SOURce: VOLTage: PROTection: CLEar");
            Command($":SOURce: VOLTage: PROTection: LEVel MAXimum");
            Command($":SOURce: VOLTage: LEVel: IMMediate: AMPLitude {VoltsDC}");
            Command($":SOURce: CURRent: LEVel: IMMediate: AMPLitude {AmpsDC}");
            Command($":SOURce: VOLTage: PROTection: LEVel {OVP}");
            Command(":OUTPut:STATe 1");
            Thread.Sleep(500); // Allow some time for voltage to stabilize.
        }

        public STATE StateGet() { return Query(":OUTPut:STATe?") == "1" ? STATE.ON : STATE.off; }

        public void StateSet(STATE State) {
            Command($":OUTPut:STATe {State == STATE.ON}");
            Thread.Sleep(500); // Allow some time for voltage to stabilize.        
        }

        public PS_E3634A(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY) { }
    }
}