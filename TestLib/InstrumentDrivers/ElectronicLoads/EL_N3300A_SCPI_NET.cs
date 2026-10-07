using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using Agilent.CommandExpert.ScpiNet.AgN33xx_A_00_08;
using System;
using System.Threading;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.ElectronicLoads {
    public class EL_N3300A_SCPI_NET : IInstrument, ISelfTests {

        public String Address { get; }
        public String Detail { get; }
        public AgN33xx AgN33xx { get; }
        public INSTRUMENT_TYPE InstrumentType { get; }

        public void ResetCommand() {
            AgN33xx.SCPI.RST.Command();
            AgN33xx.SCPI.CLS.Command();
        }

        public (SELF_TEST_RESULT Result, String Message) SelfTests() {
            Int32 result;
            try {
                AgN33xx.SCPI.TST.Query(out result);
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return (SELF_TEST_RESULT.FAIL, String.Empty);
            }
            return ((SELF_TEST_RESULT)result, String.Empty); // AgN33xx returns 0 for passed, 1 for fail.
        }

        public STATE StateGet() {
            AgN33xx.SCPI.SOURce.OUTPut.STATe.Query(out Boolean state);
            return state ? STATE.ON : STATE.off;
        }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            AgN33xx.SCPI.SOURce.OUTPut.STATe.Command(State == STATE.ON);
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.        
        }

        public EL_N3300A_SCPI_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            AgN33xx = new AgN33xx(Address);
            InstrumentType = INSTRUMENT_TYPE.ELECTRONIC_LOAD;
        }
    }
}