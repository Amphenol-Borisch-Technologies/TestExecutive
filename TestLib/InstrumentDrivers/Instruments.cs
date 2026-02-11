using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using System;
using System.Collections.Generic;
//using System.Diagnostics.Metrics;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers {
    public enum INSTRUMENT_TYPE { DIGITAL_IO, ELECTRONIC_LOAD, LOGIC_ANALYZER, MULTI_FUNCTION, MULTI_METER, OSCILLOSCOPE_ANALOG, OSCILLOSCOPE_MIXED_SIGNAL, POWER_ANALYZER, POWER_SUPPLY, SWITCHING, UNKNOWN, WAVEFORM_GENERATOR }
    [Flags] public enum INSTRUMENT_CATEGORIES { DIGITAL_INPUT = 1, DIGITAL_OUTPUT = 2, ANALOG_MEASURE = 4, ANALOG_STIMULUS = 8, SWITCHING = 16, UNKNOWN = 32 }
    public enum STATE { off = 0, ON = 1 } // NOTE: To Command an instrument off or ON, and Query it's STATE, again off or ON.
    public enum LOGIC { low = 0, HIGH = 1 } // NOTE: For digital logic.
    public enum BOOLEAN { zero = 0, ONE = 1 } // NOTE: For Boolean algebra if ever needed.  Consistent convention for lower-cased inactive states off/low/zero as 0th states in enums, UPPER-CASED active ON/HIGH/ONE as 1st states.
    public enum SENSE_MODE { EXTernal, INTernal }

    public static class Instruments {
        public static Dictionary<INSTRUMENT_TYPE, INSTRUMENT_CATEGORIES> InstrumentClassification = new Dictionary<INSTRUMENT_TYPE, INSTRUMENT_CATEGORIES>() {
            { INSTRUMENT_TYPE.ELECTRONIC_LOAD, INSTRUMENT_CATEGORIES.ANALOG_MEASURE | INSTRUMENT_CATEGORIES.ANALOG_STIMULUS },
            { INSTRUMENT_TYPE.DIGITAL_IO, INSTRUMENT_CATEGORIES.DIGITAL_INPUT | INSTRUMENT_CATEGORIES.DIGITAL_OUTPUT },
            { INSTRUMENT_TYPE.LOGIC_ANALYZER, INSTRUMENT_CATEGORIES.DIGITAL_INPUT },
            { INSTRUMENT_TYPE.MULTI_FUNCTION, INSTRUMENT_CATEGORIES.SWITCHING | INSTRUMENT_CATEGORIES.ANALOG_MEASURE | INSTRUMENT_CATEGORIES.ANALOG_STIMULUS | INSTRUMENT_CATEGORIES.DIGITAL_INPUT | INSTRUMENT_CATEGORIES.DIGITAL_OUTPUT },
            { INSTRUMENT_TYPE.MULTI_METER, INSTRUMENT_CATEGORIES.ANALOG_MEASURE },
            { INSTRUMENT_TYPE.OSCILLOSCOPE_ANALOG, INSTRUMENT_CATEGORIES.ANALOG_MEASURE },
            { INSTRUMENT_TYPE.OSCILLOSCOPE_MIXED_SIGNAL, INSTRUMENT_CATEGORIES.ANALOG_MEASURE | INSTRUMENT_CATEGORIES.DIGITAL_INPUT },
            { INSTRUMENT_TYPE.POWER_ANALYZER, INSTRUMENT_CATEGORIES.ANALOG_MEASURE },
            { INSTRUMENT_TYPE.POWER_SUPPLY, INSTRUMENT_CATEGORIES.ANALOG_STIMULUS },
            { INSTRUMENT_TYPE.SWITCHING, INSTRUMENT_CATEGORIES.SWITCHING },
            { INSTRUMENT_TYPE.UNKNOWN, INSTRUMENT_CATEGORIES.UNKNOWN },
            { INSTRUMENT_TYPE.WAVEFORM_GENERATOR, INSTRUMENT_CATEGORIES.ANALOG_STIMULUS | INSTRUMENT_CATEGORIES.DIGITAL_OUTPUT }
        };

        public static void SelfTestFailure(IInstrument iInstrument, Exception exception) {
            Int32 PR = 15;
            _ = MessageBox.Show($"Instrument with driver '{iInstrument.GetType().Name}' failed its Self-Test:{Environment.NewLine}" +
            $"{nameof(iInstrument.InstrumentType)}".PadRight(PR) + $": {iInstrument.InstrumentType}{Environment.NewLine}" +
            $"{nameof(iInstrument.Detail)}".PadRight(PR) + $": {iInstrument.Detail}{Environment.NewLine}" +
            $"{nameof(iInstrument.Address)}".PadRight(PR) + $": {iInstrument.Address}{Environment.NewLine}" +
            $"{nameof(System.Exception)}".PadRight(PR) + $": {exception}{Environment.NewLine}"
            , "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            // If unpowered or not communicating (comms cable possibly disconnected) SelfTest throws a
            // Keysight.CommandExpert.InstrumentAbstraction.CommunicationException exception,
            // which requires an apparently unavailable Keysight library to explicitly catch.
        }
    }
}