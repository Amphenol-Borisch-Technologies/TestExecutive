using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces {
    public enum AC { Amps, Volts }
    public enum DC { Amps, Volts }
    public enum OUTPUTS2 { OUTput1, OUTput2 };
    public enum OUTPUTS3 { OUTput1, OUTput2, OUTput3 };
    public enum MMD { MINimum, MAXimum, DEFault }

    public interface IPowerSupply { void OutputsOff(); }

    public interface IPowerSupplyOutputs1 : IPowerSupply {
        STATES StateGet();
        void StateSet(STATES State);
        (Double AmpsDC, Double VoltsDC) Get(DC DC);
        void SetOffOn(Double Volts, Double Amps, Double OVP, STATES State);
    }

    public interface IPowerSupplyOutputs2 : IPowerSupply {
        STATES StateGet(OUTPUTS2 Output);
        void StateSet(OUTPUTS2 Output, STATES State);
        (Double AmpsDC, Double VoltsDC) Get(OUTPUTS2 Output, DC DC);
        void SetOffOn(OUTPUTS2 Output, Double Volts, Double Amps, Double OVP, STATES State);
    }

    public interface IPowerSupplyOutputs3 : IPowerSupply {
        STATES StateGet(OUTPUTS3 Output);
        void StateSet(OUTPUTS3 Output, STATES State);
        (Double AmpsDC, Double VoltsDC) Get(OUTPUTS3 Output, DC DC);
        void SetOffOn(OUTPUTS3 Output, Double Volts, Double Amps, Double OVP, STATES State);
    }

    public interface IPowerSupplyE3649A : IPowerSupply {
        STATES StateGet(OUTPUTS2 Output);
        void StateSet(STATES State);
        // NOTE: Some multi-output supplies like the E3649A permit individual control of outputs,
        // but the E3649A does not; all supplies are set to the same STATE, off or ON.
        (Double AmpsDC, Double VoltsDC) Get(OUTPUTS2 Output, DC DC);
        void SetOffOn(OUTPUTS2 Output, Double Volts, Double Amps, Double OVP, STATES State);
    }
}
