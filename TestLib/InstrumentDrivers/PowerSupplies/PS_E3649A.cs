using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Threading;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class PS_E3649A : Instrument, IPowerSupplyE3649A {

        public void OutputsOff() { Command(":OUTPut:STATe 0"); }
        // NOTE: Some multi-output supplies like the E3649A permit individual control of outputs,
        // but the E3649A does not; all supplies are set to the same STATE, off or ON.

        public OUTPUT2 Selected() { return Query(":INSTrument:SELect?") == "OUTP1" ? OUTPUT2.OUTput1 : OUTPUT2.OUTput2; }

        public void Select(OUTPUT2 Output) { Command($":INSTrument:SELect? {Output}"); }

        public (Double AmpsDC, Double VoltsDC) Get(OUTPUT2 Output, DC DC) {
            Select(Output);
            return (Double.Parse(Query(":MEASure:SCALar:CURRent:DC?")), Double.Parse(Query(":MEASure:SCALar:VOLTage:DC?")));
        }

        public void SetOffOn(OUTPUT2 Output, Double VoltsDC, Double AmpsDC, Double OVP) {
            Select(Output);
            Command(":OUTPut:STATe 0");
            Command(":SOURce: VOLTage: PROTection: CLEar");
            Command($":SOURce: VOLTage: PROTection: LEVel MAXimum");
            Command($":SOURce: VOLTage: LEVel: IMMediate: AMPLitude {VoltsDC}");
            Command($":SOURce: CURRent: LEVel: IMMediate: AMPLitude {AmpsDC}");
            Command($":SOURce: VOLTage: PROTection: LEVel {OVP}");
            Command(":OUTPut:STATe 1");
            Thread.Sleep(500); // Allow some time for voltage to stabilize.
        }

        public STATE StateGet(OUTPUT2 Output) {
            Select(Output);
            return Query(":OUTPut:STATe?") == "1" ? STATE.ON : STATE.off;
        }

        public void StateSet(STATE State) {
            // NOTE: Some multi-output supplies like the E3649A permit individual control of outputs,
            // but the E3649A does not; all supplies are set to the same STATE, off or ON.
            Command($":OUTPut:STATe {State == STATE.ON}");
            Thread.Sleep(500); // Allow some time for voltage to stabilize.  
        }

        public PS_E3649A(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY) { }
    }
}