using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Multifunction;
using Agilent.CommandExpert.ScpiNet.AgE363x_1_7;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class PS_E3632A_SCPI_NET : IInstrument, IPowerSupplyDC_Outputs1, ISelfTests {
        public enum RANGE { P15V, P30V }

        public String Address { get; }
        public String Detail { get; }
        public AgE363x AgE363x { get; }
        public INSTRUMENT_TYPE InstrumentType { get; }

        public void ResetCommand() {
            AgE363x.SCPI.RST.Command();
            AgE363x.SCPI.CLS.Command();
        }

        public (SELF_TEST_RESULT Result, String Message) SelfTests() {
            Int32 result;
            try {
                AgE363x.SCPI.TST.Query(out result);
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return (SELF_TEST_RESULT.FAIL, String.Empty);
            }
            return ((SELF_TEST_RESULT)result, String.Empty); // AgE363x returns 0 for passed, 1 for fail.
        }

        public void OutputsOff() { StateSet(STATE.off, MillisecondsDelay: 0); }

        public RANGE RangeGet() {
            AgE363x.SCPI.SOURce.VOLTage.RANGe.Query(out String range);
            return (RANGE)Enum.Parse(typeof(RANGE), range);
        }
        public void RangeSet(RANGE Range) { AgE363x.SCPI.SOURce.VOLTage.RANGe.Command($"{Range}"); }

        public (Double VoltsDC, Double AmperesDC) Get() {
            AgE363x.SCPI.MEASure.VOLTage.DC.Query(out Double VoltsDC);
            AgE363x.SCPI.MEASure.CURRent.DC.Query(out Double AmperesDC);
            return (VoltsDC, AmperesDC);
        }

        public void SetOffOn(Double VoltsDC, Double AmperesDC, Double OVP, Int32 MillisecondsDelay = 500) {
            OutputsOff();
            AgE363x.SCPI.SOURce.VOLTage.PROTection.CLEar.Command();
            AgE363x.SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{MMD.MAXimum}");
            AgE363x.SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command($"{VoltsDC}");
            AgE363x.SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command($"{AmperesDC}");
            AgE363x.SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{OVP}");
            StateSet(STATE.ON, MillisecondsDelay);
        }

        public STATE StateGet() {
            AgE363x.SCPI.OUTPut.STATe.Query(out Boolean state);
            return state ? STATE.ON : STATE.off;
        }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            AgE363x.SCPI.OUTPut.STATe.Command(State == STATE.ON);
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.        
        }

        public PS_E3632A_SCPI_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            AgE363x = new AgE363x(Address);
            InstrumentType = INSTRUMENT_TYPE.POWER_SUPPLY_DC;
        }
    }
}