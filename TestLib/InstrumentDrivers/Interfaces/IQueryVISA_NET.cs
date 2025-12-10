using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces {
    public interface IQueryVISA_NET {
        String QueryLine(String scpiCommand);

        Byte[] QueryBinaryBlockOfByte(String scpiCommand);

        Byte[] QueryRawIO(String scpiCommand);
    }
}
