using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies;
using static ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies.Sorensen_XFR_GPIB;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.MS_Test.InstrumentDrivers.Base;

[TestClass()]
public class Sorensen_XFR_GPIBTests {
    public TestContext TestContext { get; set; }
    private static Sorensen_XFR_GPIB? _XFR_GPIB;
    private const String address = "GPIB0::5::INSTR";
    private const String detail = "Sorensen XFR30-40";
    private const INSTRUMENT_TYPE instrumentType = INSTRUMENT_TYPE.POWER_SUPPLY_DC;

    [ClassInitialize]
    public static void Setup(TestContext testcontext) { _XFR_GPIB = new Sorensen_XFR_GPIB(Address: address, Detail: detail); }

    [TestMethod()]
    public void CommandTest() {
        Assert.Fail();
    }

    [DataTestMethod()]
    //[DataRow(QUERY.ASTS)]
    //[DataRow(QUERY.FAULT)]
    //[DataRow(QUERY.STS)]
    //[DataRow(QUERY.UNMASK)]
    [DataRow(QUERY.AUXA)]
    [DataRow(QUERY.AUXB)]
    [DataRow(QUERY.HOLD)]
    [DataRow(QUERY.OUT)]
    [DataRow(QUERY.SRQ)]
    [DataRow(QUERY.DLY)]
    [DataRow(QUERY.IMAX)]
    [DataRow(QUERY.IOUT)]
    [DataRow(QUERY.ISET)]
    [DataRow(QUERY.OVSET)]
    [DataRow(QUERY.VMAX)]
    [DataRow(QUERY.VOUT)]
    [DataRow(QUERY.VSET)]
    //[DataRow(QUERY.ERR)]
    //[DataRow(QUERY.FOLD)]
    //[DataRow(QUERY.ID)]
    //[DataRow(QUERY.ROM)]
    public void QueryTest(QUERY Query) {
        Assert.IsNotNull(_XFR_GPIB);
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_GPIB.Command(COMMAND.VSET, 0.ToString());
        _XFR_GPIB.Command(COMMAND.ISET, 0.ToString());
        _XFR_GPIB.Command(COMMAND.OVSET, _XFR_GPIB.Query<Double>(QUERY.VMAX).ToString());
        switch (Query) {
            case QUERY.ASTS:
            case QUERY.FAULT:
            case QUERY.STS:
            case QUERY.UNMASK:
                break;
            case QUERY.AUXA:
            case QUERY.AUXB:
            case QUERY.HOLD:
            case QUERY.OUT:
            case QUERY.SRQ:
                _XFR_GPIB.Command($"{Query} {STATE.ON}");
                Assert.AreEqual(STATE.ON, _XFR_GPIB.Query<STATE>(Query));
                _XFR_GPIB.Command($"{Query} {STATE.off}");
                Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(Query));
                break;
            case QUERY.DLY:
                _XFR_GPIB.Command($"{Query} 20");
                Assert.AreEqual(20D, _XFR_GPIB.Query<Double>(Query));
                _XFR_GPIB.Command($"{Query} 0.5");
                Assert.AreEqual(0.5D, _XFR_GPIB.Query<Double>(Query));
                break;
            case QUERY.IOUT:
            case QUERY.VOUT:
                Assert.IsTrue(_XFR_GPIB.Query<Double>(Query) < 0.1);
                break;
            case QUERY.OVSET:
            case QUERY.IMAX:
            case QUERY.VMAX:
                Double originalValue = _XFR_GPIB.Query<Double>(Query);
                _XFR_GPIB.Command($"{Query} {originalValue - 1}");
                Assert.AreEqual(originalValue - 1, _XFR_GPIB.Query<Double>(Query));
                _XFR_GPIB.Command($"{Query} {originalValue}");
                Assert.AreEqual(originalValue, _XFR_GPIB.Query<Double>(Query));
                break;
            case QUERY.ISET:
            case QUERY.VSET:
                _XFR_GPIB.Command($"{Query} 1");
                Assert.AreEqual(1D, _XFR_GPIB.Query<Double>(Query));
                _XFR_GPIB.Command($"{Query} 0");
                Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(Query));
                break;
            case QUERY.ERR:
                break;
            case QUERY.FOLD:
                break;
            case QUERY.ID:
            case QUERY.ROM:
                break;
            default: throw new NotImplementedException(NotImplementedMessageEnum<QUERY>(Enum.GetName(typeof(QUERY), Query)));
        }
    }

    [TestMethod()]
    public void SetOffTest() {
        Assert.IsNotNull(_XFR_GPIB);
        _XFR_GPIB.SetOff(VoltsDC: 5D, AmpsDC: 0.2D, OVP: 10D);
        Assert.AreEqual(5D, _XFR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0.2D, _XFR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(10D, _XFR_GPIB.Query<Double>(QUERY.OVSET));
        Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_GPIB.Command(COMMAND.OUT, STATE.ON.ToString());
        Assert.AreEqual(STATE.ON, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_GPIB.SetOff(VoltsDC: 0D, AmpsDC: 0D, OVP: _XFR_GPIB.Query<Double>(QUERY.VMAX));
        Assert.AreEqual(0, _XFR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(_XFR_GPIB.Query<Double>(QUERY.VMAX), _XFR_GPIB.Query<Double>(QUERY.OVSET));
        Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(QUERY.OUT));
    }

    [TestMethod()]
    public void OutputsOffTest() {
        Assert.IsNotNull(_XFR_GPIB);
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_GPIB.Command(COMMAND.VSET, 0.ToString());
        _XFR_GPIB.Command(COMMAND.ISET, 0.ToString());
        Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.ISET));
        _XFR_GPIB.Command(COMMAND.OUT, STATE.ON.ToString());
        Assert.AreEqual(STATE.ON, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_GPIB.OutputsOff();
        Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(QUERY.OUT));
    }

    [TestMethod()]
    public void GetTest() {
        Assert.IsNotNull(_XFR_GPIB);
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_GPIB.Command(COMMAND.VSET, 1.ToString());
        _XFR_GPIB.Command(COMMAND.ISET, 1.ToString());
        Assert.AreEqual((1D, 1D), _XFR_GPIB.Get());
        _XFR_GPIB.Command(COMMAND.VSET, 0.ToString());
        _XFR_GPIB.Command(COMMAND.ISET, 0.ToString());
        Assert.AreEqual((0D, 0D), _XFR_GPIB.Get());
    }

    [TestMethod()]
    public void SetOffOnTest() {
        Assert.IsNotNull(_XFR_GPIB);
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_GPIB.Command(COMMAND.VSET, 0.ToString());
        _XFR_GPIB.Command(COMMAND.ISET, 0.ToString());
        _XFR_GPIB.Command(COMMAND.OVSET, 10D.ToString());
        Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(10D, _XFR_GPIB.Query<Double>(QUERY.OVSET));
        _XFR_GPIB.SetOffOn(VoltsDC: 5D, AmpsDC: 0.2D, OVP: 15D, MillisecondsDelay: 0);
        Assert.AreEqual(STATE.ON, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        Assert.AreEqual(5D, _XFR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0.2D, _XFR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(15D, _XFR_GPIB.Query<Double>(QUERY.OVSET));
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_GPIB.SetOffOn(VoltsDC: 0D, AmpsDC: 0D, OVP: _XFR_GPIB.Query<Double>(QUERY.VMAX), MillisecondsDelay: 0);
        Assert.AreEqual(STATE.ON, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(_XFR_GPIB.Query<Double>(QUERY.VMAX), _XFR_GPIB.Query<Double>(QUERY.OVSET));
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
    }

    [TestMethod()]
    public void StateGetTest() {
        Assert.IsNotNull(_XFR_GPIB);
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        Assert.AreEqual(STATE.off, _XFR_GPIB.StateGet());
        _XFR_GPIB.Command(COMMAND.OUT, STATE.ON.ToString());
        Assert.AreEqual(STATE.ON, _XFR_GPIB.StateGet());
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        Assert.AreEqual(STATE.off, _XFR_GPIB.StateGet());
    }

    [TestMethod()]
    public void StateSetTest() {
        Assert.IsNotNull(_XFR_GPIB);
        _XFR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_GPIB.StateSet(STATE.ON);
        Assert.AreEqual(STATE.ON, _XFR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_GPIB.StateSet(STATE.off);
        Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(QUERY.OUT));
    }

    [TestMethod()]
    public void Sorensen_XFR_GPIBTest() {
        Assert.IsNotNull(_XFR_GPIB);
        Assert.AreEqual(address, _XFR_GPIB.Address);
        Assert.AreEqual(detail, _XFR_GPIB.Detail);
        Assert.AreEqual(instrumentType, _XFR_GPIB.InstrumentType);
        Assert.IsInstanceOfType(_XFR_GPIB, typeof(Sorensen_XFR_GPIB));
        Assert.IsInstanceOfType(_XFR_GPIB, typeof(InstrumentDriver));
        Assert.IsInstanceOfType(_XFR_GPIB, typeof(IDisposable));
        Assert.IsInstanceOfType(_XFR_GPIB, typeof(Object));
        Assert.AreEqual((Byte)0, _XFR_GPIB.Query<Byte>(QUERY.ERR));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(0D, _XFR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(_XFR_GPIB.Query<Double>(QUERY.VMAX), _XFR_GPIB.Query<Double>(QUERY.OVSET));
        Assert.AreEqual(STATE.off, _XFR_GPIB.Query<STATE>(QUERY.OUT));
    }
}