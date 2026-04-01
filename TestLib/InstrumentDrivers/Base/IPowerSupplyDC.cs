using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public enum DC { Amps, Volts }
    public enum OUTPUT2 { OUTput1, OUTput2 };
    public enum OUTPUT3 { OUTput1, OUTput2, OUTput3 };
    public enum MMD { MINimum, MAXimum, DEFault }

    public interface IPowerSupplyDC { void OutputsOff(); }

    public interface IPowerSupplyDC_Outputs1 : IPowerSupplyDC {
        STATE StateGet();
        void StateSet(STATE State);
        (Double AmpsDC, Double VoltsDC) Get(DC DC);
        void SetOffOn(Double VoltsDC, Double AmpsDC, Double OVP);
    }

    public interface IPowerSupplyDC_Outputs2 : IPowerSupplyDC {
        STATE StateGet(OUTPUT2 Output);
        void StateSet(OUTPUT2 Output, STATE State);
        (Double AmpsDC, Double VoltsDC) Get(OUTPUT2 Output, DC DC);
        void SetOffOn(OUTPUT2 Output, Double VoltsDC, Double AmpsDC, Double OVP);
    }

    public interface IPowerSupplyDC_Outputs3 : IPowerSupplyDC {
        STATE StateGet(OUTPUT3 Output);
        void StateSet(OUTPUT3 Output, STATE State);
        (Double AmpsDC, Double VoltsDC) Get(OUTPUT3 Output, DC DC);
        void SetOffOn(OUTPUT3 Output, Double VoltsDC, Double AmpsDC, Double OVP);
    }

    public interface IPowerSupplyDC_E3649A : IPowerSupplyDC {
        STATE StateGet(OUTPUT2 Output);
        void StateSet(STATE State);
        // NOTE: Some multi-output supplies like the E3649A permit individual control of outputs,
        // but the E3649A does not; all supplies are set to the same STATE, off or ON.
        (Double AmpsDC, Double VoltsDC) Get(OUTPUT2 Output, DC DC);
        void SetOffOn(OUTPUT2 Output, Double VoltsDC, Double AmpsDC, Double OVP);
    }
}
