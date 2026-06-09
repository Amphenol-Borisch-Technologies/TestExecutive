using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;

namespace ABT.Test.TestExecutive.MS_Test.InstrumentDrivers.Base {

    [TestClass()]
    public class InstrumentDriverTests {
        private static InstrumentDriver? _instrumentDriver;
        private const String address = "GPIB0::5::INSTR";
        private const String detail = "Sorensen XFR30-40";
        private const INSTRUMENT_TYPE instrumentType = INSTRUMENT_TYPE.POWER_SUPPLY_DC;

        [TestInitialize]
        public void Setup() { _instrumentDriver = new(Address: address, Detail: detail, instrumentType); }

        [TestMethod]
        public void InstrumentDriverTest() {
            Assert.IsNotNull(_instrumentDriver);
            Assert.AreEqual(address, _instrumentDriver.Address);
            Assert.AreEqual(detail, _instrumentDriver.Detail);
            Assert.AreEqual(instrumentType, _instrumentDriver.InstrumentType);
            Assert.IsInstanceOfType(_instrumentDriver, typeof(InstrumentDriver));
            Assert.IsInstanceOfType(_instrumentDriver, typeof(IDisposable));
            Assert.IsInstanceOfType(_instrumentDriver, typeof(Object));
        }

        [TestMethod()]
        public void ThrowIfDisposedTest() {
            InstrumentDriver instrumentDriver = new(Address: address, Detail: detail, instrumentType);
            instrumentDriver.ThrowIfDisposed();
            instrumentDriver.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => instrumentDriver.ThrowIfDisposed());
        }
    }
}