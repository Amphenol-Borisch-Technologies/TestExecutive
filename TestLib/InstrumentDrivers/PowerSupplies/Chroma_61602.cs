using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Threading;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class Chroma_61602 : InstrumentDriver, IPowerSupplyAC {
        public enum RangeVoltsAC { Minimum = 0, Maximum = 300 }
        public enum RangeHertz { Minimum = 15, Maximum = 1000 }

        public void OutputsOff() { StateSet(STATE.off, MillisecondsDelay: 0); }

        public (Double AmpsAC, Double VoltsAC, Double Hertz) GetAC() { return (Double.Parse(Query(":MEASure:SCALar:CURRent:AC?")), Double.Parse(Query(":MEASure:SCALar:VOLTage:ACDC?")), Double.Parse(Query(":MEASure:SCALar:FREQuency?"))); }

        public void SetOffOn(Double VoltsAC, Double OVP, Double Hertz) {
            ValidateRangeVoltsAC(VoltsAC, nameof(VoltsAC));
            ValidateRangeVoltsAC(OVP, nameof(OVP));
            if (OVP < VoltsAC) throw new ArgumentException($"{nameof(OVP)} must be ≥ {nameof(VoltsAC)}.");
            if (Hertz < (Double)RangeHertz.Minimum || Hertz > (Double)RangeHertz.Maximum) throw new ArgumentOutOfRangeException($"{Hertz} must be ≥ {(Int32)RangeHertz.Minimum} and ≤ {(Int32)RangeHertz.Maximum} Hertz.");
            StateSet(STATE.off, MillisecondsDelay: 0);
            Command(":OUTPut:PROTection:CLEar");
            Command(":OUTPut:COUPling:AC");
            Command($":SOURce:VOLTage:LIMit {OVP}");
            Command($":SOURce:FREQuency:IMMediate: {Hertz}");
            Command($":SOURce:VOLTage:LEVel:IMMediate:AMPLitude: AC {VoltsAC}");
            StateSet(STATE.ON, MillisecondsDelay: 500);
        }

        private void ValidateRangeVoltsAC(Double VoltsAC, String NameVAC) { if (VoltsAC < (Double)RangeVoltsAC.Minimum || VoltsAC > (Double)RangeVoltsAC.Maximum) throw new ArgumentOutOfRangeException($"{NameVAC} must be ≥ {(Int32)RangeVoltsAC.Minimum} and ≤ {(Int32)RangeVoltsAC.Maximum} VAC."); }

        public STATE StateGet() { return Query(":OUTPut:STATe?") == "ON" ? STATE.ON : STATE.off; }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            Command($":OUTPut:STATe {State}");
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.        
        }

        public Chroma_61602(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY_AC) { }
    }
}