using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies;
using static ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies.Sorensen_XFR_XHR_GPIB;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.MS_Test.InstrumentDrivers.Base;

[TestClass()]
public class Sorensen_XFR_XHR_GPIBTests {
    public TestContext TestContext { get; set; }
    private static Sorensen_XFR_XHR_GPIB? _XFR_XHR_GPIB;
    private const String address = "GPIB0::5::INSTR";
    private const String detail = "Sorensen XFR30-40";
    private const INSTRUMENT_TYPE instrumentType = INSTRUMENT_TYPE.POWER_SUPPLY_DC;

    [ClassInitialize]
    public static void Setup(TestContext testcontext) { _XFR_XHR_GPIB = new Sorensen_XFR_XHR_GPIB(Address: address, Detail: detail); }

    [DataTestMethod()]
    [DataRow(COMMAND.AUXA)]
    [DataRow(COMMAND.AUXB)]
    [DataRow(COMMAND.HOLD)]
    [DataRow(COMMAND.OUT)]
    [DataRow(COMMAND.SRQ)]
    [DataRow(COMMAND.CLR)]
    [DataRow(COMMAND.RST)]
    [DataRow(COMMAND.TRG)]
    [DataRow(COMMAND.DLY)]
    [DataRow(COMMAND.IMAX)]
    [DataRow(COMMAND.ISET)]
    [DataRow(COMMAND.OVSET)]
    [DataRow(COMMAND.VMAX)]
    [DataRow(COMMAND.VSET)]
    [DataRow(COMMAND.FOLD)]
    [DataRow(COMMAND.MASK)]
    [DataRow(COMMAND.UNMASK)]
    public void CommandTest(COMMAND Command) {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        switch (Command) {
            case COMMAND.AUXA:
            case COMMAND.AUXB:
            case COMMAND.HOLD:
            case COMMAND.OUT:
            case COMMAND.SRQ:
                _XFR_XHR_GPIB.Command($"{Command} {STATE.ON}");
                Assert.AreEqual(STATE.ON, _XFR_XHR_GPIB.Query<STATE>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString())));
                _XFR_XHR_GPIB.Command($"{Command} {STATE.off}");
                Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString())));
                break;
            case COMMAND.CLR:
            case COMMAND.RST:
            case COMMAND.TRG:
                _ = _XFR_XHR_GPIB.Query<Byte>(QUERY.ERR); // Clear any existing error.
                _XFR_XHR_GPIB.Command($"{Command}");
                Assert.AreEqual(0, _XFR_XHR_GPIB.Query<Byte>(QUERY.ERR)); // Valid Commands shouldn't cause errors.
                break;
            case COMMAND.DLY:
                _XFR_XHR_GPIB.Command($"{Command} 20");
                Assert.AreEqual(20D, _XFR_XHR_GPIB.Query<Double>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString())));
                _XFR_XHR_GPIB.Command($"{Command} 0.5");
                Assert.AreEqual(0.5D, _XFR_XHR_GPIB.Query<Double>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString())));
                break;
            case COMMAND.OVSET:
            case COMMAND.IMAX:
            case COMMAND.VMAX:
                Double originalValue = _XFR_XHR_GPIB.Query<Double>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString()));
                _XFR_XHR_GPIB.Command($"{Command} {originalValue - 1}");
                Assert.AreEqual(originalValue - 1, _XFR_XHR_GPIB.Query<Double>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString())));
                _XFR_XHR_GPIB.Command($"{Command} {originalValue}");
                Assert.AreEqual(originalValue, _XFR_XHR_GPIB.Query<Double>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString())));
                break;
            case COMMAND.ISET:
            case COMMAND.VSET:
                _XFR_XHR_GPIB.Command($"{Command} 1");
                Assert.AreEqual(1D, _XFR_XHR_GPIB.Query<Double>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString())));
                _XFR_XHR_GPIB.Command($"{Command} 0");
                Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>((QUERY)Enum.Parse(typeof(QUERY), Command.ToString())));
                break;
            case COMMAND.FOLD:
                _XFR_XHR_GPIB.Command(COMMAND.FOLD, FOLD.OFF.ToString());
                Assert.AreEqual(FOLD.OFF, _XFR_XHR_GPIB.Query<FOLD>(QUERY.FOLD));
                _XFR_XHR_GPIB.Command(COMMAND.FOLD, FOLD.CC.ToString());
                Assert.AreEqual(FOLD.CC, _XFR_XHR_GPIB.Query<FOLD>(QUERY.FOLD));
                _XFR_XHR_GPIB.Command(COMMAND.FOLD, FOLD.CV.ToString());
                Assert.AreEqual(FOLD.CV, _XFR_XHR_GPIB.Query<FOLD>(QUERY.FOLD));
                break;
            case COMMAND.MASK:
                _XFR_XHR_GPIB.Command(COMMAND.MASK, $"{ASTS.CV}, {ASTS.CC}, {ASTS.OV}, {ASTS.OT}, {ASTS.SD}, {ASTS.FOLD}, {ASTS.ERR}, {ASTS.ACF}, {ASTS.OPF}, {ASTS.SNSP}");
                Assert.AreEqual($"{ASTS.NONE}", _XFR_XHR_GPIB.Query<String>(QUERY.UNMASK));
                Assert.AreEqual((Int32)ASTS.NONE, _XFR_XHR_GPIB.Query<Int32>(QUERY.UNMASK));
                _XFR_XHR_GPIB.Command(COMMAND.MASK, $"{ASTS.ALL}");
                break;
            case COMMAND.UNMASK:
                _XFR_XHR_GPIB.Command(COMMAND.UNMASK, $"{ASTS.CV}, {ASTS.CC}");
                Assert.AreEqual($"{ASTS.CV}, {ASTS.CC}", _XFR_XHR_GPIB.Query<String>(QUERY.UNMASK));
                Assert.AreEqual((Int32)(ASTS.CV | ASTS.CC), _XFR_XHR_GPIB.Query<Int32>(QUERY.UNMASK));
                _XFR_XHR_GPIB.Command(COMMAND.UNMASK, $"{ASTS.NONE}");
                Assert.AreEqual(ASTS.NONE.ToString(), _XFR_XHR_GPIB.Query<String>(QUERY.UNMASK));
                Assert.AreEqual((Int32)ASTS.NONE, _XFR_XHR_GPIB.Query<Int32>(QUERY.UNMASK));
                _XFR_XHR_GPIB.Command(COMMAND.UNMASK, $"{ASTS.NONE}");
                break;
            default: throw new NotImplementedException(NotImplementedMessageEnum<COMMAND>(Enum.GetName(typeof(COMMAND), Command)));
        }
    }

    [DataTestMethod()]
    [DataRow(QUERY.ASTS)]
    [DataRow(QUERY.FAULT)]
    [DataRow(QUERY.STS)]
    [DataRow(QUERY.UNMASK)]
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
    [DataRow(QUERY.ERR)]
    [DataRow(QUERY.FOLD)]
    [DataRow(QUERY.ID)]
    [DataRow(QUERY.ROM)]
    public void QueryTest(QUERY Query) {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.VSET, 0.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.ISET, 0.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.OVSET, _XFR_XHR_GPIB.Query<Double>(QUERY.VMAX).ToString());
        switch (Query) {
            case QUERY.ASTS:
            case QUERY.FAULT:
            case QUERY.STS:
            case QUERY.UNMASK:
                Assert.IsTrue(_XFR_XHR_GPIB.Query<Int32>(Query) >= 0 && _XFR_XHR_GPIB.Query<Int32>(Query) <= 8191);
                Assert.IsFalse(String.IsNullOrEmpty(_XFR_XHR_GPIB.Query<String>(Query)));
                break;
            case QUERY.AUXA:
            case QUERY.AUXB:
            case QUERY.HOLD:
            case QUERY.OUT:
            case QUERY.SRQ:
                _XFR_XHR_GPIB.Command($"{Query} {STATE.ON}");
                Assert.AreEqual(STATE.ON, _XFR_XHR_GPIB.Query<STATE>(Query));
                _XFR_XHR_GPIB.Command($"{Query} {STATE.off}");
                Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(Query));
                break;
            case QUERY.DLY:
                _XFR_XHR_GPIB.Command($"{Query} 20");
                Assert.AreEqual(20D, _XFR_XHR_GPIB.Query<Double>(Query));
                _XFR_XHR_GPIB.Command($"{Query} 0.5");
                Assert.AreEqual(0.5D, _XFR_XHR_GPIB.Query<Double>(Query));
                break;
            case QUERY.IOUT:
            case QUERY.VOUT:
                Assert.IsTrue(_XFR_XHR_GPIB.Query<Double>(Query) < 0.1);
                break;
            case QUERY.OVSET:
            case QUERY.IMAX:
            case QUERY.VMAX:
                Double originalValue = _XFR_XHR_GPIB.Query<Double>(Query);
                _XFR_XHR_GPIB.Command($"{Query} {originalValue - 1}");
                Assert.AreEqual(originalValue - 1, _XFR_XHR_GPIB.Query<Double>(Query));
                _XFR_XHR_GPIB.Command($"{Query} {originalValue}");
                Assert.AreEqual(originalValue, _XFR_XHR_GPIB.Query<Double>(Query));
                break;
            case QUERY.ISET:
            case QUERY.VSET:
                _XFR_XHR_GPIB.Command($"{Query} 1");
                Assert.AreEqual(1D, _XFR_XHR_GPIB.Query<Double>(Query));
                _XFR_XHR_GPIB.Command($"{Query} 0");
                Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(Query));
                break;
            case QUERY.ERR:
                _XFR_XHR_GPIB.Query<Byte>(Query); // Clear any existing error.
                Assert.AreEqual(0, _XFR_XHR_GPIB.Query<Byte>(Query));
                break;
            case QUERY.FOLD:
                _XFR_XHR_GPIB.Command($"{Query} {FOLD.OFF}");
                Assert.AreEqual(FOLD.OFF, _XFR_XHR_GPIB.Query<FOLD>(Query));
                _XFR_XHR_GPIB.Command($"{Query} {FOLD.CC}");
                Assert.AreEqual(FOLD.CC, _XFR_XHR_GPIB.Query<FOLD>(Query));
                _XFR_XHR_GPIB.Command($"{Query} {FOLD.CV}");
                Assert.AreEqual(FOLD.CV, _XFR_XHR_GPIB.Query<FOLD>(Query));
                break;
            case QUERY.ID:
            case QUERY.ROM:
                Assert.IsFalse(String.IsNullOrEmpty(_XFR_XHR_GPIB.Query<String>(Query)));
                break;
            default: throw new NotImplementedException(NotImplementedMessageEnum<QUERY>(Enum.GetName(typeof(QUERY), Query)));
        }
    }

    [TestMethod()]
    public void SetOffTest() {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        _XFR_XHR_GPIB.SetOff(VoltsDC: 5D, AmpsDC: 0.2D, OVP: 10D);
        Assert.AreEqual(5D, _XFR_XHR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0.2D, _XFR_XHR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(10D, _XFR_XHR_GPIB.Query<Double>(QUERY.OVSET));
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.ON.ToString());
        Assert.AreEqual(STATE.ON, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_XHR_GPIB.SetOff(VoltsDC: 0D, AmpsDC: 0D, OVP: _XFR_XHR_GPIB.Query<Double>(QUERY.VMAX));
        Assert.AreEqual(0, _XFR_XHR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(_XFR_XHR_GPIB.Query<Double>(QUERY.VMAX), _XFR_XHR_GPIB.Query<Double>(QUERY.OVSET));
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
    }

    [TestMethod()]
    public void OutputsOffTest() {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.VSET, 0.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.ISET, 0.ToString());
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.ISET));
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.ON.ToString());
        Assert.AreEqual(STATE.ON, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_XHR_GPIB.OutputsOff();
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
    }

    [TestMethod()]
    public void GetTest() {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.VSET, 1.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.ISET, 1.ToString());
        Assert.AreEqual((1D, 1D), _XFR_XHR_GPIB.Get());
        _XFR_XHR_GPIB.Command(COMMAND.VSET, 0.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.ISET, 0.ToString());
        Assert.AreEqual((0D, 0D), _XFR_XHR_GPIB.Get());
    }

    [TestMethod()]
    public void SetOffOnTest() {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.VSET, 0.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.ISET, 0.ToString());
        _XFR_XHR_GPIB.Command(COMMAND.OVSET, 10D.ToString());
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(10D, _XFR_XHR_GPIB.Query<Double>(QUERY.OVSET));
        _XFR_XHR_GPIB.SetOffOn(VoltsDC: 5D, AmpsDC: 0.2D, OVP: 15D, MillisecondsDelay: 0);
        Assert.AreEqual(STATE.ON, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        Assert.AreEqual(5D, _XFR_XHR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0.2D, _XFR_XHR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(15D, _XFR_XHR_GPIB.Query<Double>(QUERY.OVSET));
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        _XFR_XHR_GPIB.SetOffOn(VoltsDC: 0D, AmpsDC: 0D, OVP: _XFR_XHR_GPIB.Query<Double>(QUERY.VMAX), MillisecondsDelay: 0);
        Assert.AreEqual(STATE.ON, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(_XFR_XHR_GPIB.Query<Double>(QUERY.VMAX), _XFR_XHR_GPIB.Query<Double>(QUERY.OVSET));
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
    }

    [TestMethod()]
    public void StateGetTest() {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.StateGet());
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.ON.ToString());
        Assert.AreEqual(STATE.ON, _XFR_XHR_GPIB.StateGet());
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.StateGet());
    }

    [TestMethod()]
    public void StateSetTest() {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        _XFR_XHR_GPIB.Command(COMMAND.OUT, STATE.off.ToString());
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_XHR_GPIB.StateSet(STATE.ON);
        Assert.AreEqual(STATE.ON, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
        _XFR_XHR_GPIB.StateSet(STATE.off);
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
    }

    [TestMethod()]
    public void Sorensen_XFR_GPIBTest() {
        Assert.IsNotNull(_XFR_XHR_GPIB);
        Assert.AreEqual(address, _XFR_XHR_GPIB.Address);
        Assert.AreEqual(detail, _XFR_XHR_GPIB.Detail);
        Assert.AreEqual(instrumentType, _XFR_XHR_GPIB.InstrumentType);
        Assert.IsInstanceOfType(_XFR_XHR_GPIB, typeof(Sorensen_XFR_XHR_GPIB));
        Assert.IsInstanceOfType(_XFR_XHR_GPIB, typeof(InstrumentDriver));
        Assert.IsInstanceOfType(_XFR_XHR_GPIB, typeof(IDisposable));
        Assert.IsInstanceOfType(_XFR_XHR_GPIB, typeof(Object));
        Assert.AreEqual((Byte)0, _XFR_XHR_GPIB.Query<Byte>(QUERY.ERR));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.ISET));
        Assert.AreEqual(0D, _XFR_XHR_GPIB.Query<Double>(QUERY.VSET));
        Assert.AreEqual(_XFR_XHR_GPIB.Query<Double>(QUERY.VMAX), _XFR_XHR_GPIB.Query<Double>(QUERY.OVSET));
        Assert.AreEqual(STATE.off, _XFR_XHR_GPIB.Query<STATE>(QUERY.OUT));
    }
}