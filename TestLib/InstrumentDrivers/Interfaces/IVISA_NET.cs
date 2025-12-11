using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces {
    public interface IVISA_NET {
        String QueryLine(String scpiCommand);

        Byte[] QueryBinaryBlockOfByte(String scpiCommand);

        Byte[] QueryRawIO(String scpiCommand);
    }
}
