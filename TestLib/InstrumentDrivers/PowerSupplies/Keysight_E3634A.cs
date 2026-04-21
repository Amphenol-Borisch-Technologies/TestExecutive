using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class Keysight_E3634A : InstrumentDriver, IPowerSupplyDC_Outputs1, ISelfTests {
        public enum RANGE { P25V, P50V }

        public void OutputsOff() { StateSet(STATE.off, MillisecondsDelay: 0); }

        public RANGE RangeGet() { return (RANGE)Enum.Parse(typeof(RANGE), Query(":SOURce:VOLTage:RANGe?")); }
        public void RangeSet(RANGE Range) { Command($":SOURce:VOLTage:RANGe {Range}"); }

        public (Double AmpsDC, Double VoltsDC) Get() { return (Double.Parse(Query(":MEASure:CURRent:DC?")), Double.Parse(Query(":MEASure:VOLTage:DC?"))); }

        public void SetOffOn(Double VoltsDC, Double AmpsDC, Double OVP, Int32 MillisecondsDelay = 500) {
            OutputsOff();
            Command(":SOURce: VOLTage: PROTection: CLEar");
            Command($":SOURce: VOLTage: PROTection: LEVel MAXimum");
            Command($":SOURce: VOLTage: LEVel: IMMediate: AMPLitude {VoltsDC}");
            Command($":SOURce: CURRent: LEVel: IMMediate: AMPLitude {AmpsDC}");
            Command($":SOURce: VOLTage: PROTection: LEVel {OVP}");
            StateSet(STATE.ON, MillisecondsDelay);
        }

        public STATE StateGet() { return Query(":OUTPut:STATe?") == "1" ? STATE.ON : STATE.off; }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            Command($":OUTPut:STATe {State == STATE.ON}");
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.        
        }

        public Keysight_E3634A(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY_DC) { }

        public (SELF_TEST_RESULT Result, String Message) SelfTests() {
            ThrowIfDisposed();
            const String Test = "*TST?";
            Int32 PR = 15;
            StringBuilder Message = new StringBuilder();
            Message.AppendLine($"{nameof(SelfTests)}".PadRight(PR) + $": SCPI {Test}");
            Message.AppendLine($"{nameof(InstrumentDriver)}".PadRight(PR) + $": {GetType().Name}");
            Message.AppendLine($"{nameof(InstrumentType)}".PadRight(PR) + $": {InstrumentType}");
            Message.AppendLine($"{nameof(Detail)}".PadRight(PR) + $": {Detail}");
            Message.AppendLine($"{nameof(Address)}".PadRight(PR) + $": {Address}");
            Message.Append($"{nameof(SELF_TEST_RESULT)}".PadRight(PR));
            SELF_TEST_RESULT Result;
            try {
                Result = (SELF_TEST_RESULT)Int32.Parse(Query(Test));
            } catch (Exception exception) {
                Result = SELF_TEST_RESULT.EXCEPTION;
                Message.Insert(0, $"{exception.Message}{Environment.NewLine}");
                _ = MessageBox.Show($"{exception.Message}{Environment.NewLine}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            return (Result, Message.Append($": {Result}").ToString());
        }
    }
}