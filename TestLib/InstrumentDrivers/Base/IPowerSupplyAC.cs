using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public enum AC { Amps, Volts }

    public interface IPowerSupplyAC { void OutputsOff(); }

    public interface IPowerSupplyAC_Outputs1 : IPowerSupplyAC {
        STATE StateGet();
        void StateSet(STATE State);
        (Double AmpsAC, Double VoltsAC, Double Hertz) Get();
        void SetOffOn(Double VoltsAC, Double Hertz);
    }
}
