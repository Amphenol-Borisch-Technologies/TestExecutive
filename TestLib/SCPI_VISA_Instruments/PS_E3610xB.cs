using System;
using System.Threading;
using Agilent.CommandExpert.ScpiNet.AgE3610XB_1_0_0_1_00;
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
    public static class PS_E36103B { public const String MODEL = "E36103B"; } // PS_E36103B needed only in class TestExecutive.AppConfig.SCPI_VISA_Instrument.

    public static class PS_E36105B { public const String MODEL = "E36105B"; } // PS_E36105B needed only in class TestExecutive.AppConfig.SCPI_VISA_Instrument.

    public static class PS_E3610xB {
        public static Boolean IsPS_E3610xB(SCPI_VISA_Instrument SVI) { return (SVI.Instrument.GetType() == typeof(AgE3610XB)); }

        public const Boolean LoadOrStimulus = true;

        public static Boolean CurrentAmplitudeIs(SCPI_VISA_Instrument SVI, Double AmpsDC, Double Delta) {
            return SCPI99.IsCloseEnough(CurrentAmplitudeGet(SVI), AmpsDC, Delta);
        }

        public static Double CurrentAmplitudeGet(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Query(null, out Double ampsDC);
            return ampsDC;
        }

        public static void CurrentAmplitudeSet(SCPI_VISA_Instrument SVI, Double AmpsDC) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command(AmpsDC);
        }

        public static Double CurrentProtectionDelayGet(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.DELay.TIME.Query(null, out Double seconds);
            return seconds;
        }

        public static void CurrentProtectionDelaySet(SCPI_VISA_Instrument SVI, Double DelaySeconds) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.DELay.TIME.Command(DelaySeconds);
            CurrentProtectionStateSet(SVI, STATE.ON);
        }

        public static Boolean CurrentProtectionStateGet(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.STATe.Query(out Boolean state);
            return state;
        }

        public static void CurrentProtectionStateSet(SCPI_VISA_Instrument SVI, STATE State) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.STATe.Command((State is STATE.ON));
        }
     
        public static void CurrentProtectionTrippedClear(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.CLEar.Command();
        }

        public static Boolean CurrentProtectionTrippedGet(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.TRIPped.Query(out Boolean tripped);
            return tripped;
        }

        public static Double Get(SCPI_VISA_Instrument SVI, PS_DC DC) {
            switch (DC) {
                case PS_DC.Amps:
                    ((AgE3610XB)SVI.Instrument).SCPI.MEASure.CURRent.DC.Query(out Double ampsDC);
                    return ampsDC;
                case PS_DC.Volts:
                    ((AgE3610XB)SVI.Instrument).SCPI.MEASure.VOLTage.DC.Query(out Double voltsDC);
                    return voltsDC;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Initialize(SCPI_VISA_Instrument SVI) {
            // NOTE:  Mustn't invoke TestExecutive.CT_EmergencyStop.ThrowIfCancellationRequested(); on Initialize() or it's invoked methods Reset() & Clear().
            SCPI99.Initialize(SVI);
            ((AgE3610XB)SVI.Instrument).SCPI.OUTPut.PROTection.CLEar.Command();
            ((AgE3610XB)SVI.Instrument).SCPI.DISPlay.WINDow.TEXT.CLEar.Command();
        }

        public static void Local(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SYSTem.LOCal.Command();
        }

        public static void Remote(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SYSTem.REMote.Command();
        }

        public static void RemoteLock(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SYSTem.RWLock.Command();
        }

        public static void Set(SCPI_VISA_Instrument SVI, STATE State, Double VoltsDC, Double AmpsDC, SENSE_MODE KelvinSense = SENSE_MODE.INTernal, Double DelaySecondsCurrentProtection = 0, Double DelaySecondsSettling = 0) {
            VoltageProtectionStateSet(SVI, STATE.off);
            CurrentProtectionStateSet(SVI, STATE.off);
            VoltageProtectionTrippedClear(SVI);
            CurrentProtectionTrippedClear(SVI);

            VoltageSenseModeSet(SVI, KelvinSense);
            VoltageAmplitudeSet(SVI, VoltsDC);
            CurrentAmplitudeSet(SVI, AmpsDC);

            VoltageProtectionSet(SVI, VoltsDC * 1.10);
            CurrentProtectionDelaySet(SVI, DelaySecondsCurrentProtection);

            VoltageProtectionStateSet(SVI, STATE.ON);
            CurrentProtectionStateSet(SVI, STATE.ON);
            SCPI99.Set(SVI, State);

            Thread.Sleep(millisecondsTimeout: (Int32)(DelaySecondsSettling * 1000));
        }

        public static Boolean VoltageAmplitudeIs(SCPI_VISA_Instrument SVI, Double VoltsDC, Double Delta) {
            return SCPI99.IsCloseEnough(VoltageAmplitudeGet(SVI), VoltsDC, Delta);
        }

        public static Double VoltageAmplitudeGet(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Query(null, out Double voltsDC);
            return voltsDC;
        }

        public static void VoltageAmplitudeSet(SCPI_VISA_Instrument SVI, Double VoltsDC) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command(VoltsDC);
        }

        public static Double VoltageProtectionGet(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.LEVel.Query(null, out Double amplitude);
            return amplitude;
        }

        public static void VoltageProtectionSet(SCPI_VISA_Instrument SVI, Double VoltsDC) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.LEVel.Command(VoltsDC);
            VoltageProtectionStateSet(SVI, STATE.ON);
        }

        public static void VoltageProtectionTrippedClear(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.CLEar.Command();
        }

        public static Boolean VoltageProtectionTrippedGet(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.TRIPped.Query(out Boolean tripped);
            return tripped;
        }

        public static Boolean VoltageProtectionStateGet(SCPI_VISA_Instrument SVI) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.STATe.Query(out Boolean state);
            return state;
        }

        public static void VoltageProtectionStateSet(SCPI_VISA_Instrument SVI, STATE State) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.STATe.Command(State is STATE.ON);
        }
 
        public static void VoltageSenseModeSet(SCPI_VISA_Instrument SVI, SENSE_MODE KelvinSense) {
            ((AgE3610XB)SVI.Instrument).SCPI.SOURce.VOLTage.SENSe.SOURce.Command(Enum.GetName(typeof(SENSE_MODE), KelvinSense));
        }
    }
}