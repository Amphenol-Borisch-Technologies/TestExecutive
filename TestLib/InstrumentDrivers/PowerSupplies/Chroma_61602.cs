using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Threading;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class Chroma_61602 : InstrumentDriver, IPowerSupplyAC {
        public enum RangeVoltsAC { Minimum = 0, Maximum = 300 }
        public enum RangeHertz { Minimum = 15, Maximum = 1000 }

        public void OutputsOff() { StateSet(STATE.off, MillisecondsDelay: 0); }

        public (Double AmpsAC, Double VoltsAC, Double Hertz) GetAC() { return (Double.Parse(Query(":MEASure:SCALar:CURRent:AC?")), Double.Parse(Query(":MEASure:SCALar:VOLTage:ACDC?")), Double.Parse(Query(":MEASure:SCALar:FREQuency?"))); }

        public void SetOffOn(Double VoltsAC, Double Hertz, Int32 MillisecondsDelay = 500) {
            if (VoltsAC < (Double)RangeVoltsAC.Minimum || VoltsAC > (Double)RangeVoltsAC.Maximum) throw new ArgumentOutOfRangeException($"{VoltsAC} must be ≥ {(Int32)RangeVoltsAC.Minimum} and ≤ {(Int32)RangeVoltsAC.Maximum} VAC.");
            if (Hertz < (Double)RangeHertz.Minimum || Hertz > (Double)RangeHertz.Maximum) throw new ArgumentOutOfRangeException($"{Hertz} must be ≥ {(Int32)RangeHertz.Minimum} and ≤ {(Int32)RangeHertz.Maximum} Hertz.");
            OutputsOff();
            Command(":OUTPut:PROTection:CLEar");
            Command(":OUTPut:COUPling:AC");
            Command($":SOURce:FREQuency:IMMediate {Hertz}");
            Command($":SOURce:VOLTage:LEVel:IMMediate:AMPLitude:AC {VoltsAC}");
            StateSet(STATE.ON, MillisecondsDelay);
        }

        public STATE StateGet() { return Query(":OUTPut:STATe?") == "ON" ? STATE.ON : STATE.off; }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            Command($":OUTPut:STATe {State}");
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.        
        }

        public Chroma_61602(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY_AC) { }
    }
}