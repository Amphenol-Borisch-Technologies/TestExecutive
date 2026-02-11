using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces {
    public enum AC { Amps, Volts }
    public enum DC { Amps, Volts }
    public enum OUTPUT2 { OUTput1, OUTput2 };
    public enum OUTPUT3 { OUTput1, OUTput2, OUTput3 };
    public enum MMD { MINimum, MAXimum, DEFault }

    public interface IPowerSupply { void OutputsOff(); }

    public interface IPowerSupplyOutputs1 : IPowerSupply {
        STATE StateGet();
        void StateSet(STATE State);
        (Double AmpsDC, Double VoltsDC) Get(DC DC);
        void SetOffOn(Double Volts, Double Amps, Double OVP);
    }

    public interface IPowerSupplyOutputs2 : IPowerSupply {
        STATE StateGet(OUTPUT2 Output);
        void StateSet(OUTPUT2 Output, STATE State);
        (Double AmpsDC, Double VoltsDC) Get(OUTPUT2 Output, DC DC);
        void SetOffOn(OUTPUT2 Output, Double Volts, Double Amps, Double OVP);
    }

    public interface IPowerSupplyOutputs3 : IPowerSupply {
        STATE StateGet(OUTPUT3 Output);
        void StateSet(OUTPUT3 Output, STATE State);
        (Double AmpsDC, Double VoltsDC) Get(OUTPUT3 Output, DC DC);
        void SetOffOn(OUTPUT3 Output, Double Volts, Double Amps, Double OVP);
    }

    public interface IPowerSupplyE3649A : IPowerSupply {
        STATE StateGet(OUTPUT2 Output);
        void StateSet(STATE State);
        // NOTE: Some multi-output supplies like the E3649A permit individual control of outputs,
        // but the E3649A does not; all supplies are set to the same STATE, off or ON.
        (Double AmpsDC, Double VoltsDC) Get(OUTPUT2 Output, DC DC);
        void SetOffOn(OUTPUT2 Output, Double Volts, Double Amps, Double OVP);
    }
}
