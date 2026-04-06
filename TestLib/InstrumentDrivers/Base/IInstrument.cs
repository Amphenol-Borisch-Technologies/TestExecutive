using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public interface IInstrument {
        String Address { get; }                                 // NOTE: Store in instrument objects for easy error reporting of addresses.  Not easily gotten otherwise.
        String Detail { get; }                                  // NOTE: Store in instrument objects for easy error reporting of detailed descriptions, similar but more useful than SCPI's *IDN query.
        INSTRUMENT_TYPE InstrumentType { get; }
        void ResetCommand();                                    // NOTE: After each test run perform SCPI's *RST & *CLS commands or IVI's Initialize command or intstrument manufacturer's proprietary equivalent to reset/clear the instrument to a known state.
    }
}