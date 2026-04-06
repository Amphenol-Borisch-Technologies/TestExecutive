using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.WaveformGenerators {
    #region TL;DR
    // NOTE: Keysight 33120A Function Generator User's Guide https://www.keysight.com/us/en/assets/9018-04436/user-manuals/9018-04436.pdf.
    #endregion TL;DR
    public class Keysight_33120A : InstrumentDriver {
        public Keysight_33120A(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.WAVEFORM_GENERATOR) { ResetCommand(); }

        ~Keysight_33120A() { Dispose(); }
    }
}
