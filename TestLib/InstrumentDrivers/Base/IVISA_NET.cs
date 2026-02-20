using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public interface IVISA_NET {
        String QueryLine(String scpiCommand);

        Byte[] QueryBinaryBlockOfByte(String scpiCommand);

        Byte[] QueryRawIO(String scpiCommand);
    }
}
