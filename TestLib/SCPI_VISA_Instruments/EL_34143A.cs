using Agilent.CommandExpert.ScpiNet.AgEL30000_1_2_5_1_0_6_17_114;
using System;
using System.Runtime.CompilerServices;
using static ABT.TestSpace.TestExec.SCPI_VISA_Instruments.Keysight;
// All Agilent.CommandExpert.ScpiNet drivers are procured by adding new SCPI VISA Instruments in Keysight's Command Expert app software.
//  - Command Expert literally downloads & installs Agilent.CommandExpert.ScpiNet drivers when new SVIs are added.
//  - The Agilent.CommandExpert.ScpiNet drivers are installed into folder C:\ProgramData\Keysight\Command Expert\ScpiNetDrivers.
// https://www.keysight.com/us/en/lib/software-detail/computer-software/command-expert-downloads-2151326.html
//
// Enthusiastically recommend using Command Expert to generate SCPI commands, which are directly exportable as .Net statements.
// https://www.keysight.com/us/en/search.html/command+expert
//
namespace ABT.TestSpace.TestExec.SCPI_VISA_Instruments {
    public enum LOAD_MODE { CURR = 0, POW = 1, RES = 2, VOLT = 3 }
    public enum LOAD_MEASURE { CURR = 0, POW = 1, VOLT = 3 }

    public static class EL_34143A {
        public const String MODEL = "EL34143A";

        public const Boolean LoadOrStimulus = true;

        public static Boolean IsEL_34143A(SCPI_VISA_Instrument SVI) { return (SVI.Instrument.GetType() == typeof(AgEL30000)); }

        public static void Initialize(SCPI_VISA_Instrument SVI) {
            // NOTE:  Mustn't invoke TestExecutive.CT_EmergencyStop.ThrowIfCancellationRequested(); on Initialize() or it's invoked methods Reset() & Clear().
            SCPI99.Initialize(SVI);
            ((AgEL30000)SVI.Instrument).SCPI.OUTPut.PROTection.CLEar.Command();
            ((AgEL30000)SVI.Instrument).SCPI.DISPlay.WINDow.TEXT.CLEar.Command();
        }

        public static void Local(SCPI_VISA_Instrument SVI) {
            ((AgEL30000)SVI.Instrument).SCPI.SYSTem.LOCal.Command();
        }

        public static LOAD_MODE Get(SCPI_VISA_Instrument SVI) {
            ((AgEL30000)SVI.Instrument).SCPI.SOURce.MODE.Query(null, out String LoadMode);
            return (LOAD_MODE)Enum.Parse(typeof(LOAD_MODE), LoadMode);
        }

        public static Double Get(SCPI_VISA_Instrument SVI, LOAD_MEASURE LoadMeasure, SENSE_MODE KelvinSense) {
            VoltageSenseModeSet(SVI, KelvinSense);
            switch (LoadMeasure) {
                case LOAD_MEASURE.CURR:
                    ((AgEL30000)SVI.Instrument).SCPI.MEASure.SCALar.CURRent.DC.Query(null, out Double[] ampsDC);
                    return ampsDC[0];
                case LOAD_MEASURE.POW:
                    ((AgEL30000)SVI.Instrument).SCPI.MEASure.SCALar.POWer.DC.Query(null, out Double[] watts);
                    return watts[0];
                case LOAD_MEASURE.VOLT:
                    ((AgEL30000)SVI.Instrument).SCPI.MEASure.SCALar.VOLTage.DC.Query(null, out Double[] voltsDC);
                    return voltsDC[0];
                default:
                    throw new NotImplementedException();
            }
        }

        public static Double Get(SCPI_VISA_Instrument SVI, PS_DC DC, CHANNEL Channel, SENSE_MODE KelvinSense) {
            VoltageSenseModeSet(SVI, KelvinSense);
            switch (DC) {
                case PS_DC.Amps:
                    ((AgEL30000)SVI.Instrument).SCPI.MEASure.SCALar.CURRent.DC.Query(Channels[Channel], out Double[] ampsDC);
                    return ampsDC[(Int32)(Channel)];
                case PS_DC.Volts:
                    ((AgEL30000)SVI.Instrument).SCPI.MEASure.SCALar.VOLTage.DC.Query(Channels[Channel], out Double[] voltsDC);
                    return voltsDC[(Int32)(Channel)];
                default:
                    throw new NotImplementedException();
            }
        }

        public static Boolean Is(SCPI_VISA_Instrument SVI, STATE State) {
            ((AgEL30000)SVI.Instrument).SCPI.OUTPut.STATe.Query(null, out Boolean state);
            return ((state ? STATE.ON : STATE.off) == State);
        }

        public static Boolean Is(SCPI_VISA_Instrument SVI, Double LoadValue, LOAD_MODE LoadMode) {
            if (!Is(SVI, LoadMode)) return false;
            Double delta = 0.01;
            switch (LoadMode) {
                case LOAD_MODE.CURR:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Query(null, null, out Double ampsDC);
                    return SCPI99.IsCloseEnough(LoadValue, ampsDC, delta);
                case LOAD_MODE.POW:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.LEVel.IMMediate.AMPLitude.Query(null, null, out Double[] watts);
                    return SCPI99.IsCloseEnough(LoadValue, watts[0], delta);
                case LOAD_MODE.RES:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.LEVel.IMMediate.AMPLitude.Query(null, null, out Double[] ohms);
                    return SCPI99.IsCloseEnough(LoadValue, ohms[0], delta);
                case LOAD_MODE.VOLT:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Query(null, null, out Double[] voltsDC);
                    return SCPI99.IsCloseEnough(LoadValue, voltsDC[0], delta);
                default:
                    throw new NotImplementedException();
            }
        }

        public static Boolean Is(SCPI_VISA_Instrument SVI, LOAD_MODE LoadMode) {
            return LoadMode == Get(SVI);
        }

        public static void Remote(SCPI_VISA_Instrument SVI) {
            ((AgEL30000)SVI.Instrument).SCPI.SYSTem.REMote.Command();
        }

        public static void RemoteLock(SCPI_VISA_Instrument SVI) {
            ((AgEL30000)SVI.Instrument).SCPI.SYSTem.RWLock.Command();
        }

        public static void Set(SCPI_VISA_Instrument SVI, STATE State) {
            if (!Is(SVI, State)) ((AgEL30000)SVI.Instrument).SCPI.OUTPut.STATe.Command(State is STATE.ON, null);
        }

        public static void Set(SCPI_VISA_Instrument SVI, STATE State, Double LoadValue, LOAD_MODE LoadMode, SENSE_MODE KelvinSense) {
            Set(SVI, LoadValue, LoadMode, KelvinSense);
            Set(SVI, State);
        }

        public static void Set(SCPI_VISA_Instrument SVI, Double LoadValue, LOAD_MODE LoadMode, SENSE_MODE KelvinSense) {
            ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SENSe.SOURce.Command(Enum.GetName(typeof(SENSE_MODE), KelvinSense));
            VoltageSenseModeSet(SVI, KelvinSense);
            switch (LoadMode) {
                case LOAD_MODE.CURR:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.MODE.Command("CURRent", null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.RANGe.Command(MAXimum, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command(LoadValue, null);
                    break;
                case LOAD_MODE.POW:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.MODE.Command("POWer", null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.RANGe.Command(MAXimum, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.LEVel.IMMediate.AMPLitude.Command(LoadValue, null);
                    // TODO:  Eventually; ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.PROTection.STATe.Command(false, null);
                    break;
                case LOAD_MODE.RES:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.MODE.Command("RESistance", null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.RANGe.Command(MAXimum, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.LEVel.IMMediate.AMPLitude.Command(LoadValue, null);
                    // TODO:  Eventually; ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.PROTection.STATe.Command(false, null);
                    break;
                case LOAD_MODE.VOLT:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.MODE.Command("VOLTage", null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.RANGe.Command(MAXimum, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command(LoadValue, null);
                    // TODO:  Eventually; ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.PROTection.STATe.Command(false, null);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static Boolean SlewRatesAre(SCPI_VISA_Instrument SVI, Double SlewRateRising, Double SlewRateFalling, LOAD_MODE LoadMode) {
            return ((SlewRateRising, SlewRateFalling) == SlewRatesGet(SVI, LoadMode));
        }

        public static (Double SlewRateRising, Double SlewRateFalling) SlewRatesGet(SCPI_VISA_Instrument SVI, LOAD_MODE LoadMode) {
            Double[] slewRateRising; Double[] slewRateFalling;
            switch (LoadMode) {
                case LOAD_MODE.CURR:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.SLEW.POSitive.IMMediate.Query(null, null, out slewRateRising);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.SLEW.NEGative.IMMediate.Query(null, null, out slewRateFalling);
                    break;
                case LOAD_MODE.POW:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.SLEW.POSitive.IMMediate.Query(null, null, out slewRateRising);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.SLEW.NEGative.IMMediate.Query(null, null, out slewRateFalling);
                    break;
                case LOAD_MODE.RES:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.SLEW.POSitive.IMMediate.Query(null, null, out slewRateRising);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.SLEW.NEGative.IMMediate.Query(null, null, out slewRateFalling);
                    break;
                case LOAD_MODE.VOLT:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.POSitive.IMMediate.Query(null, null, out slewRateRising);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.NEGative.IMMediate.Query(null, null, out slewRateFalling);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return (slewRateRising[0], slewRateFalling[0]);
        }

        public static void SlewRatesSet(SCPI_VISA_Instrument SVI, Double SlewRateRising, Double SlewRateFalling, LOAD_MODE LoadMode) {
            switch (LoadMode) {
                case LOAD_MODE.CURR:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.SLEW.COUP.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.SLEW.POSitive.MAXimum.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.SLEW.POSitive.IMMediate.Command(SlewRateRising, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.SLEW.NEGative.MAXimum.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.CURRent.SLEW.NEGative.IMMediate.Command(SlewRateFalling, null);
                    break;
                case LOAD_MODE.POW:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.SLEW.COUPle.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.SLEW.POSitive.MAXimum.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.SLEW.POSitive.IMMediate.Command(SlewRateRising, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.SLEW.NEGative.MAXimum.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.POWer.SLEW.NEGative.IMMediate.Command(SlewRateFalling, null);
                    break;
                case LOAD_MODE.RES:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.SLEW.COUPle.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.SLEW.POSitive.MAXimum.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.SLEW.POSitive.IMMediate.Command(SlewRateRising, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.SLEW.NEGative.MAXimum.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.RESistance.SLEW.NEGative.IMMediate.Command(SlewRateFalling, null);
                    break;
                case LOAD_MODE.VOLT:
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.COUPle.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.POSitive.MAXimum.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.POSitive.IMMediate.Command(SlewRateRising, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.NEGative.MAXimum.Command(false, null);
                    ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.NEGative.IMMediate.Command(SlewRateFalling, null);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static SENSE_MODE VoltageSenseModeGet(SCPI_VISA_Instrument SVI) {
            ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SENSe.SOURce.Query(null, out String SenseMode);
            return (SENSE_MODE)Enum.Parse(typeof(SENSE_MODE), SenseMode);
        }

        public static Boolean VoltageSenseModeIs(SCPI_VISA_Instrument SVI, SENSE_MODE SenseMode) {
            return SenseMode == VoltageSenseModeGet(SVI);
        }

        public static void VoltageSenseModeSet(SCPI_VISA_Instrument SVI, SENSE_MODE KelvinSense) {
            ((AgEL30000)SVI.Instrument).SCPI.SOURce.VOLTage.SENSe.SOURce.Command(Enum.GetName(typeof(SENSE_MODE), KelvinSense));
            // Despite being part of VOLTage sub-system, SCPI.SOURce.VOLTage.SENSe.SOURce.Command("EXTernal"/"INTernal") enables/disables 4-wire Kelvin sensing for all Kelvin capable loads.
        }
    }
}