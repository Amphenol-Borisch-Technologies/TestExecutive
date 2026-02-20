using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public enum SELF_TEST_RESULT { PASS = 0, FAIL = 1 }

    public interface IInstrument {
        String Address { get; }                 // NOTE: Store in instrument objects for easy error reporting of addresses.  Not easily gotten otherwise.
        String Detail { get; }                  // NOTE: Store in instrument objects for easy error reporting of detailed descriptions, similar but more useful than SCPI's *IDN query.
        INSTRUMENT_TYPE InstrumentType { get; }
        void ResetClear();                      // NOTE: After each test run perform SCPI's *RST & *CLS commands or IVI's Initialize command.
        SELF_TEST_RESULT SelfTests();          // NOTE: provide default implementation if ever upgrade to .Net from .Net Framework.
    }
}