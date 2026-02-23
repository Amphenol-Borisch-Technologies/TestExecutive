using System;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public interface IVISA_NET {
        String QueryLine(String SCPI_Command);

        Byte[] QueryBinaryBlockOfByte(String SCPI_Command);

        Byte[] QueryRawIO(String SCPI_Command);
    }
}
