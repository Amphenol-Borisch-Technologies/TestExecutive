using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public enum AC { Amperes, Volts }

    public interface IPowerSupplyAC { void OutputsOff(); }

    public interface IPowerSupplyAC_Outputs1 : IPowerSupplyAC {
        STATE StateGet();
        void StateSet(STATE State, Int32 MillisecondsDelay = 500);
        (Double VoltsAC, Double AmperesAC, Double Hertz) Get();
        void SetOffOn(Double VoltsAC, Double Hertz, Int32 MillisecondsDelay = 500);
    }
}
