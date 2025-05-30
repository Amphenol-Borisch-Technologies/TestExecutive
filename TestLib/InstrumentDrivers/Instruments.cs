﻿using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using System;
using System.Collections.Generic;
//using System.Diagnostics.Metrics;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers {
    public enum INSTRUMENT_TYPES { DIGITAL_IO, ELECTRONIC_LOAD, LOGIC_ANALYZER, MULTI_FUNCTION, MULTI_METER, OSCILLOSCOPE_ANALOG, OSCILLOSCOPE_MIXED_SIGNAL, POWER_ANALYZER, POWER_SUPPLY, SWITCHING, UNKNOWN, WAVEFORM_GENERATOR }
    [Flags] public enum INSTRUMENT_CATEGORIES { DIGITAL_INPUT = 1, DIGITAL_OUTPUT = 2, ANALOG_MEASURE = 4, ANALOG_STIMULUS = 8, SWITCHING = 16, UNKNOWN = 32 }
    public enum STATES { off = 0, ON = 1 } // NOTE: To Command an instrument off or ON, and Query it's STATE, again off or ON.
    public enum SENSE_MODE { EXTernal, INTernal }
    // Consistent convention for lower-cased inactive states off/low/zero as 1st states in enums, UPPER-CASED active ON/HIGH/ONE as 2nd states.

    public static class Instruments {
        public static Dictionary<INSTRUMENT_TYPES, INSTRUMENT_CATEGORIES> InstrumentClassification = new Dictionary<INSTRUMENT_TYPES, INSTRUMENT_CATEGORIES>() {
            { INSTRUMENT_TYPES.ELECTRONIC_LOAD, INSTRUMENT_CATEGORIES.ANALOG_MEASURE | INSTRUMENT_CATEGORIES.ANALOG_STIMULUS },
            { INSTRUMENT_TYPES.DIGITAL_IO, INSTRUMENT_CATEGORIES.DIGITAL_INPUT | INSTRUMENT_CATEGORIES.DIGITAL_OUTPUT },
            { INSTRUMENT_TYPES.LOGIC_ANALYZER, INSTRUMENT_CATEGORIES.DIGITAL_INPUT },
            { INSTRUMENT_TYPES.MULTI_FUNCTION, INSTRUMENT_CATEGORIES.SWITCHING | INSTRUMENT_CATEGORIES.ANALOG_MEASURE | INSTRUMENT_CATEGORIES.ANALOG_STIMULUS | INSTRUMENT_CATEGORIES.DIGITAL_INPUT | INSTRUMENT_CATEGORIES.DIGITAL_OUTPUT },
            { INSTRUMENT_TYPES.MULTI_METER, INSTRUMENT_CATEGORIES.ANALOG_MEASURE },
            { INSTRUMENT_TYPES.OSCILLOSCOPE_ANALOG, INSTRUMENT_CATEGORIES.ANALOG_MEASURE },
            { INSTRUMENT_TYPES.OSCILLOSCOPE_MIXED_SIGNAL, INSTRUMENT_CATEGORIES.ANALOG_MEASURE | INSTRUMENT_CATEGORIES.DIGITAL_INPUT },
            { INSTRUMENT_TYPES.POWER_ANALYZER, INSTRUMENT_CATEGORIES.ANALOG_MEASURE },
            { INSTRUMENT_TYPES.POWER_SUPPLY, INSTRUMENT_CATEGORIES.ANALOG_STIMULUS },
            { INSTRUMENT_TYPES.SWITCHING, INSTRUMENT_CATEGORIES.SWITCHING },
            { INSTRUMENT_TYPES.UNKNOWN, INSTRUMENT_CATEGORIES.UNKNOWN },
            { INSTRUMENT_TYPES.WAVEFORM_GENERATOR, INSTRUMENT_CATEGORIES.ANALOG_STIMULUS | INSTRUMENT_CATEGORIES.DIGITAL_OUTPUT }
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