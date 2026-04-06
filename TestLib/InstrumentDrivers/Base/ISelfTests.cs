using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public enum SELF_TEST_RESULT { PASS = 0, FAIL = 1, EXCEPTION = 2 }

    public interface ISelfTests {
        (SELF_TEST_RESULT Result, String Message) SelfTests();  // NOTE: provide default implementation if ever upgrade to .Net from .Net Framework.
    }
}