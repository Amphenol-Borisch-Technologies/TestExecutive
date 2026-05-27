using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base;
using System;
using System.Threading;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.PowerSupplies {
    public class Chroma_61602 : InstrumentDriver, IPowerSupplyAC {
        public enum RangeVoltsAC { Minimum = 0, Maximum = 300 }
        public enum RangeHertz { Minimum = 15, Maximum = 1000 }

        public void OutputsOff() { StateSet(STATE.off, MillisecondsDelay: 0); }

        public (Double AmpsAC, Double VoltsAC, Double Hertz) GetAC() { return (Double.Parse(Query(":MEASure:SCALar:CURRent:AC?")), Double.Parse(Query(":MEASure:SCALar:VOLTage:ACDC?")), Double.Parse(Query(":MEASure:SCALar:FREQuency?"))); }

        public void SetOffOn(Double VoltsAC, Double Hertz, Int32 MillisecondsDelay = 500) {
            if (VoltsAC < (Double)RangeVoltsAC.Minimum || VoltsAC > (Double)RangeVoltsAC.Maximum) throw new ArgumentOutOfRangeException($"{VoltsAC} must be ≥ {(Int32)RangeVoltsAC.Minimum} and ≤ {(Int32)RangeVoltsAC.Maximum} VAC.");
            if (Hertz < (Double)RangeHertz.Minimum || Hertz > (Double)RangeHertz.Maximum) throw new ArgumentOutOfRangeException($"{Hertz} must be ≥ {(Int32)RangeHertz.Minimum} and ≤ {(Int32)RangeHertz.Maximum} Hertz.");
            OutputsOff();
            Command(":OUTPut:PROTection:CLEar");
            Command(":OUTPut:COUPling:AC");
            Command($":SOURce:FREQuency:IMMediate {Hertz}");
            Command($":SOURce:VOLTage:LEVel:IMMediate:AMPLitude:AC {VoltsAC}");
            StateSet(STATE.ON, MillisecondsDelay);
        }

        public STATE StateGet() { return Query(":OUTPut:STATe?") == "ON" ? STATE.ON : STATE.off; }

        public void StateSet(STATE State, Int32 MillisecondsDelay = 500) {
            Command($":OUTPut:STATe {State}");
            Thread.Sleep(MillisecondsDelay); // Allow some time for voltage to stabilize.        
        }

        public Chroma_61602(String Address, String Detail) : base(Address, Detail, INSTRUMENT_TYPE.POWER_SUPPLY_AC) { }

        public void ClearStatus() => Command("*CLS");
        public void Reset() => Command("*RST");
        public void Save(Int32 n) => Command($"*SAV {n}");
        public void Recall(Int32 n) => Command($"*RCL {n}");
        public String Identify() => Query("*IDN?");
        public String SelfTest() => Query("*TST?");
        public String GetStatusByte() => Query("*STB?");
        public void SetSRE(Int32 value) => Command($"*SRE {value}");
        public String GetSRE() => Query("*SRE?");
        public void SetESE(Int32 value) => Command($"*ESE {value}");
        public String GetESE() => Query("*ESE?");
        public String SystemError() => Query("SYSTem:ERRor?");
        public String SystemVersion() => Query("SYSTem:VERSion?");

        // ----------------------------------------------------
        // OUTPUT SUBSYSTEM
        // ----------------------------------------------------

        public void OutputOn() => Command("OUTPut ON");
        public void OutputOff() => Command("OUTPut OFF");
        public String OutputState() => Query("OUTPut?");

        public void RelayOn() => Command("OUTPut:RELay ON");
        public void RelayOff() => Command("OUTPut:RELay OFF");
        public String RelayState() => Query("OUTPut:RELay?");
        public void SetACSlew(Double vpm) => Command($"OUTPut:SLEW:VOLTage:AC {vpm}");
        public Double GetACSlew() => Query<Double>("OUTPut:SLEW:VOLTage:AC?");

        public void SetDCSlew(Double vpm) => Command($"OUTPut:SLEW:VOLTage:DC {vpm}");
        public Double GetDCSlew() => Query<Double>("OUTPut:SLEW:VOLTage:DC?");
        public void SetFreqSlew(Double hzMs) => Command($"OUTPut:SLEW:FREQuency {hzMs}");
        public Double GetFreqSlew() => Query<Double>("OUTPut:SLEW:FREQuency?");

        public void EnableOutputSlew(Boolean on) => Command($"OUTPut:SLEW:OUT {(on ? "ON" : "OFF")}");
        public String GetOutputSlewState() => Query("OUTPut:SLEW:OUT?");

        public void SetCoupling(String mode) => Command($"OUTPut:COUPling {mode}");
        public String GetCoupling() => Query("OUTPut:COUPling?");

        public void ClearProtection() => Command("OUTPut:PROTection:CLEar");

        public void SetHVOption(String state) => Command($"OUTPut:OPTion:HV {state}");
        public String GetHVOption() => Query("OUTPut:OPTion:HV?");

        // ----------------------------------------------------
        // SOURCE SUBSYSTEM
        // ----------------------------------------------------

        public void SetFrequency(Double Hertz) => Command($"SOURce:FREQuency {Hertz}");
        public Double GetFrequency() => Query<Double>("SOURce:FREQuency?");

        public void SetACVoltage(Double VAC) => Command($"SOURce:VOLTage:AC {VAC}");
        public Double GetACVoltage() => Query<Double>("SOURce:VOLTage:AC?");
        public void SetDCVoltage(Double VDC) => Command($"SOURce:VOLTage:DC {VDC}");
        public Double GetDCVoltage() => Query<Double>("SOURce:VOLTage:DC?");

        public void SetCurrentLimit(Double A) => Command($"SOURce:CURRent:LIMit {A}");
        public Double GetCurrentLimit() => Query<Double>("SOURce:CURRent:LIMit?");

        public void SetCurrentDelay(Double Seconds) => Command($"SOURce:CURRent:DELay {Seconds}");
        public Double GetCurrentDelay() => Query<Double>("SOURce:CURRent:DELay?");
        public void SetInrushStart(Double MilliSeconds) => Command($"SOURce:CURRent:INRush:STARt {MilliSeconds}");
        public Double GetInrushStart() => Query<Double>("SOURce:CURRent:INRush:STARt?");

        public void SetInrushInterval(Double MilliSeconds) => Command($"SOURce:CURRent:INRush:INTerval {MilliSeconds}");
        public Double GetInrushInterval() => Query<Double>("SOURce:CURRent:INRush:INTerval?");
        public void SetVoltageLimitAC(Double VAC) => Command($"SOURce:VOLTage:LIMit:AC {VAC}");
        public Double GetVoltageLimitAC() => Query<Double>("SOURce:VOLTage:LIMit:AC?");

        public void SetVoltageLimitDCPlus(Double VDC) => Command($"SOURce:VOLTage:LIMit:DC:PLUS {VDC}");
        public Double GetVoltageLimitDCPlus() => Query<Double>("SOURce:VOLTage:LIMit:DC:PLUS?");

        public void SetVoltageLimitDCMinus(Double VDC) => Command($"SOURce:VOLTage:LIMit:DC:MINus {VDC}");
        public Double GetVoltageLimitDCMinus() => Query<Double>("SOURce:VOLTage:LIMit:DC:MINus?");

        public void SetRange(String range) => Command($"SOURce:VOLTage:RANGe {range}");
        public String GetRange() => Query("SOURce:VOLTage:RANGe?");

        // ----------------------------------------------------
        // CONFIG SUBSYSTEM
        // ----------------------------------------------------

        public void SetInhibit(String mode) => Command($"SOURce:CONFigure:INHibit {mode}");
        public String GetInhibit() => Query("SOURce:CONFigure:INHibit?");

        public void SetExternal(Boolean on) => Command($"SOURce:CONFigure:EXTernal {(on ? "ON" : "OFF")}");
        public String GetExternal() => Query("SOURce:CONFigure:EXTernal?");
        public void SetCouplingMode(String mode) => Command($"SOURce:CONFigure:COUPling {mode}");
        public String GetCouplingMode() => Query("SOURce:CONFigure:COUPling?");

        // ----------------------------------------------------
        // PHASE SUBSYSTEM
        // ----------------------------------------------------

        public void SetPhaseOn(Double Degree) => Command($"SOURce:PHASe:ON {Degree}");
        public Double GetPhaseOn() => Query<Double>("SOURce:PHASe:ON?");

        public void SetPhaseOff(Double Degree) => Command($"SOURce:PHASe:OFF {Degree}");
        public Double GetPhaseOff() => Query<Double>("SOURce:PHASe:OFF?");
        // ----------------------------------------------------
        // MEASURE / FETCH SUBSYSTEM
        // ----------------------------------------------------

        public Double MeasVoltageAC() => Query<Double>("MEASure:VOLTage:AC?");
        public Double MeasVoltageDC() => Query<Double>("MEASure:VOLTage:DC?");
        public Double MeasCurrentAC() => Query<Double>("MEASure:CURRent:AC?");
        public Double MeasCurrentDC() => Query<Double>("MEASure:CURRent:DC?");
        public Double MeasCurrentPeak() => Query<Double>("MEASure:CURRent:AMPLitude:MAXimum?");
        public Double MeasCurrentCrest() => Query<Double>("MEASure:CURRent:CREST?");
        public Double MeasFrequency() => Query<Double>("MEASure:FREQuency?");
        public Double MeasPowerTrue() => Query<Double>("MEASure:POWer:AC?");
        public Double MeasPowerApparent() => Query<Double>("MEASure:POWer:AC:APP?");
        public Double MeasPowerReactive() => Query<Double>("MEASure:POWer:AC:REAC?");
        public Double MeasPowerFactor() => Query<Double>("MEASure:POWer:AC:PFAC?");

        // Fetch versions
        public Double FetchVoltageAC() => Query<Double>("FETCh:VOLTage:AC?");
        public Double FetchVoltageDC() => Query<Double>("FETCh:VOLTage:DC?");
        public Double FetchCurrentAC() => Query<Double>("FETCh:CURRent:AC?");
        public Double FetchCurrentPeak() => Query<Double>("FETCh:CURRent:AMPLitude:MAXimum?");
        public Double FetchCurrentCrest() => Query<Double>("FETCh:CURRent:CREST?");
        public Double FetchFrequency() => Query<Double>("FETCh:FREQuency?");
        public Double FetchPowerTrue() => Query<Double>("FETCh:POWer:AC?");
        public Double FetchPowerApparent() => Query<Double>("FETCh:POWer:AC:APP?");
        public Double FetchPowerReactive() => Query<Double>("FETCh:POWer:AC:REAC?");
        public Double FetchPowerFactor() => Query<Double>("FETCh:POWer:AC:PFAC?");

        // ----------------------------------------------------
        // STATUS SUBSYSTEM
        // ----------------------------------------------------

        public String StatusOperation() => Query("STATus:OPERation?");
        public String StatusOperationEvent() => Query("STATus:OPERation:EVENt?");
        public void SetStatusOperationEnable(Int32 mask) => Command($"STATus:OPERation:ENABle {mask}");
        public String GetStatusOperationEnable() => Query("STATus:OPERation:ENABle?");

        public String StatusQuestionableCondition() => Query("STATus:QUESionable:CONDition?");
        public String StatusQuestionableEvent() => Query("STATus:QUESionable:EVENt?");
        public void SetStatusQuestionableEnable(Int32 mask) => Command($"STATus:QUESionable:ENABle {mask}");
        public String GetStatusQuestionableEnable() => Query("STATus:QUESionable:ENABle?");

        public void SetStatusQuestionableNTR(Int32 mask) => Command($"STATus:QUESionable:NTRansition {mask}");
        public String GetStatusQuestionableNTR() => Query("STATus:QUESionable:NTRransition?");

        public void SetStatusQuestionablePTR(Int32 mask) => Command($"STATus:QUESionable:PTRansition {mask}");
        public String GetStatusQuestionablePTR() => Query("STATus:QUESionable:PTRansition?");

        // ----------------------------------------------------
        // SERIES / INSTR
        // ----------------------------------------------------

        public void SetSeriesState(String mode) => Command($"SERies:STATE {mode}");
        public String GetSeriesState() => Query("SERies:STATE?");

        public void SetInstrumentDegree(Double Degree) => Command($"INSTrument:DEGRee {Degree}");
        public Double GetInstrumentDegree() => Query<Double>("INSTrument:DEGRee?");
    }
}