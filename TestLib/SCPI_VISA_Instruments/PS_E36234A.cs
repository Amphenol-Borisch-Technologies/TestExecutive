using System;
using System.Threading;
using Agilent.CommandExpert.ScpiNet.AgE36200_1_0_0_1_0_2_1_00;
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
    public static class PS_E36234A {
        public const String MODEL = "E36234A";

        public const Boolean LoadOrStimulus = true;

        private enum CPDS { CCTRans, SCHange } // For .SCPI.SOURce.CURRent.PROTection.DELay.STARt.Command();
        private static readonly String CCTRans = Enum.GetName(typeof(CPDS), CPDS.CCTRans);
        private static readonly String SCHange = Enum.GetName(typeof(CPDS), CPDS.SCHange);

        public static Boolean IsPS_E36234A(SCPI_VISA_Instrument SVI) { return (SVI.Instrument.GetType() == typeof(AgE36200)); }

        public static Double CurrentAmplitudeGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Query(null, Channels[Channel], out Double[] ampsDC);
            return ampsDC[(Int32)Channel];
        }

        public static Boolean CurrentAmplitudeIs(SCPI_VISA_Instrument SVI, Double AmpsDC, Double Delta, CHANNEL Channel) {
            return SCPI99.IsCloseEnough(CurrentAmplitudeGet(SVI, Channel), AmpsDC, Delta);
        }

        public static void CurrentAmplitudeSet(SCPI_VISA_Instrument SVI, Double AmpsDC, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command(AmpsDC, Channels[Channel]);
        }

        public static Double CurrentProtectionAmplitudeGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.LEVel.AMPLitude.Query(null, Channels[Channel], out Double[] amperes);
            return amperes[(Int32)Channel];
        }

        public static void CurrentProtectionAmplitudeSet(SCPI_VISA_Instrument SVI, Double Amperes, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.LEVel.AMPLitude.Command(Amperes, Channels[Channel]);
        }

        public static Double CurrentProtectionDelayGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.DELay.TIME.Query(null, Channels[Channel], out Double[] seconds);
            return seconds[(Int32)Channel];
        }

        public static void CurrentProtectionDelaySet(SCPI_VISA_Instrument SVI, Double DelaySeconds, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.DELay.TIME.Command(DelaySeconds, Channels[Channel]);
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.DELay.STARt.Command(CCTRans, Channels[Channel]);
        }

        public static Boolean CurrentProtectionStateGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.STATe.Query(Channels[Channel], out Boolean[] state);
            return state[(Int32)Channel];
        }
 
        public static void CurrentProtectionStateSet(SCPI_VISA_Instrument SVI, STATE State, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.STATe.Command((State is STATE.ON), Channels[Channel]);
        }
      
        public static void CurrentProtectionTrippedClear(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.CLEar.Command(Channels[Channel]);
        }

        public static Boolean CurrentProtectionTrippedGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.CURRent.PROTection.TRIPped.Query(Channels[Channel], out Int32[] tripped);
            return (tripped[(Int32)Channel] == 1);
        }
        
        public static Double Get(SCPI_VISA_Instrument SVI, PS_DC DC, CHANNEL Channel, SENSE_MODE KelvinSense) {
            VoltageSenseModeSet(SVI, KelvinSense, Channel);
            switch (DC) {
                case PS_DC.Amps:
                    ((AgE36200)SVI.Instrument).SCPI.MEASure.SCALar.CURRent.DC.Query(Channels[Channel], out Double[] ampsDC);
                    return ampsDC[(Int32)(Channel)];
                case PS_DC.Volts:
                    ((AgE36200)SVI.Instrument).SCPI.MEASure.SCALar.VOLTage.DC.Query(Channels[Channel], out Double[] voltsDC);
                    return voltsDC[(Int32)(Channel)];
                default:
                    throw new NotImplementedException();
            }
        }

        public static STATE Get(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.OUTPut.STATe.Query(Channels[Channel], out Boolean[] States);
            return States[(Int32)(Channel)] ? STATE.ON : STATE.off;
        }

        public static void Initialize(SCPI_VISA_Instrument SVI) {
            // NOTE:  Mustn't invoke TestExecutive.CT_EmergencyStop.ThrowIfCancellationRequested(); on Initialize() or it's invoked methods Reset() & Clear().
            SCPI99.Initialize(SVI);
            ((AgE36200)SVI.Instrument).SCPI.OUTPut.PROTection.CLEar.Command(Channels[CHANNEL.C1]);
            ((AgE36200)SVI.Instrument).SCPI.OUTPut.PROTection.CLEar.Command(Channels[CHANNEL.C2]);
            ((AgE36200)SVI.Instrument).SCPI.DISPlay.WINDow.TEXT.CLEar.Command();
        }

        public static Boolean Is(SCPI_VISA_Instrument SVI, STATE State, CHANNEL Channel) {
            return (State == Get(SVI, Channel));
        }

        public static void Local(SCPI_VISA_Instrument SVI) {
            ((AgE36200)SVI.Instrument).SCPI.SYSTem.LOCal.Command();
        }

        public static void Remote(SCPI_VISA_Instrument SVI) {
            ((AgE36200)SVI.Instrument).SCPI.SYSTem.REMote.Command();
        }

        public static void RemoteLock(SCPI_VISA_Instrument SVI) {
            ((AgE36200)SVI.Instrument).SCPI.SYSTem.RWLock.Command();
        }

        public static Boolean SlewRatesAre(SCPI_VISA_Instrument SVI, Double SlewRateRising, Double SlewRateFalling, CHANNEL Channel) {
            return ((SlewRateRising, SlewRateFalling) == SlewRatesGet(SVI, Channel));
        }

        public static (Double SlewRateRising, Double SlewRateFalling) SlewRatesGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.RISing.IMMediate.Query(null, Channels[Channel], out Double[] slewRateRising);
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.FALLing.IMMediate.Query(null, Channels[Channel], out Double[] slewRateFalling);
            return (slewRateRising[(Int32)Channel], slewRateFalling[(Int32)Channel]);
        }

        public static void SlewRatesSet(SCPI_VISA_Instrument SVI, Double SlewRateRising, Double SlewRateFalling, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.RISing.IMMediate.Command(SlewRateRising, Channels[Channel]);
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.SLEW.FALLing.IMMediate.Command(SlewRateFalling, Channels[Channel]);
        }

        public static void Set(SCPI_VISA_Instrument SVI, STATE State, Double VoltsDC, Double AmpsDC, Double VoltageProtectionAmplitude, CHANNEL Channel, SENSE_MODE KelvinSense, Double DelaySecondsCurrentProtection = 0, Double DelaySecondsSettling = 0) {
            Set(SVI, PS_DC.Amps, AmpsDC, Channel, KelvinSense);
            Set(SVI, PS_DC.Volts, VoltsDC, Channel, KelvinSense);
            
            CurrentProtectionDelaySet(SVI, DelaySecondsCurrentProtection, Channel);
            VoltageProtectionAmplitudeSet(SVI, VoltageProtectionAmplitude, Channel);

            Set(SVI, State, Channel);
            CurrentProtectionStateSet(SVI, State, Channel);
            VoltageProtectionStateSet(SVI, State, Channel);

            Thread.Sleep(millisecondsTimeout: (Int32)(DelaySecondsSettling * 1000));
        }

        public static void Set(SCPI_VISA_Instrument SVI, PS_DC DC, Double Amplitude, CHANNEL Channel, SENSE_MODE KelvinSense) {
            VoltageSenseModeSet(SVI, KelvinSense, Channel);
            switch (DC) {
                case PS_DC.Amps:
                    CurrentProtectionStateSet(SVI, STATE.off, Channel);
                    CurrentProtectionTrippedClear(SVI, Channel);
                    CurrentAmplitudeSet(SVI, Amplitude, Channel);
                    CurrentProtectionAmplitudeSet(SVI, Amplitude, Channel);
                    break;
                case PS_DC.Volts:
                    VoltageProtectionStateSet(SVI, STATE.off, Channel);
                    VoltageProtectionTrippedClear(SVI, Channel);
                    VoltageAmplitudeSet(SVI, Amplitude, Channel);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Set(SCPI_VISA_Instrument SVI, STATE State, CHANNEL Channel) {
            if(!Is(SVI, State, Channel)) ((AgE36200)SVI.Instrument).SCPI.OUTPut.STATe.Command((State is STATE.ON), Channels[Channel]);
        }

        public static Double VoltageAmplitudeGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Query(null, Channels[Channel], out Double[] voltsDC);
            return voltsDC[(Int32)Channel];
        }

        public static Boolean VoltageAmplitudeIs(SCPI_VISA_Instrument SVI, Double VoltsDC, Double Delta, CHANNEL Channel) {
            return SCPI99.IsCloseEnough(VoltageAmplitudeGet(SVI, Channel), VoltsDC, Delta);
        }

        public static void VoltageAmplitudeSet(SCPI_VISA_Instrument SVI, Double VoltsDC, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command(VoltsDC, Channels[Channel]);
        }

        public static Double VoltageProtectionAmplitudeGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.LEVel.AMPLitude.Query(null, Channels[Channel], out Double[] amplitude);
            return amplitude[(Int32)Channel];
        }

        public static void VoltageProtectionAmplitudeSet(SCPI_VISA_Instrument SVI, Double VoltsDC, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.LEVel.AMPLitude.Command(VoltsDC, Channels[Channel]);
        }

        public static Boolean VoltageProtectionStateGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.STATe.Query(Channels[Channel], out Boolean state);
            return state;
        }

        public static void VoltageProtectionStateSet(SCPI_VISA_Instrument SVI, STATE State, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.STATe.Command((State is STATE.ON), Channels[Channel]);
        }

        public static void VoltageProtectionTrippedClear(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.CLEar.Command(Channels[Channel]);
        }

        public static Boolean VoltageProtectionTrippedGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.PROTection.TRIPped.Query(Channels[Channel], out Int32[] tripped);
            return (tripped[(Int32)Channel] == 1);
        }

        public static SENSE_MODE VoltageSenseModeGet(SCPI_VISA_Instrument SVI, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.SENSe.SOURce.Query(Channels[Channel], out String[] SenseMode);
            return (SENSE_MODE)Enum.Parse(typeof(SENSE_MODE), SenseMode[(Int32)Channel]); 
        }

        public static Boolean VoltageSenseModeIs(SCPI_VISA_Instrument SVI, SENSE_MODE SenseMode, CHANNEL Channel) {
            return SenseMode == VoltageSenseModeGet(SVI, Channel);
        }

        public static void VoltageSenseModeSet(SCPI_VISA_Instrument SVI, SENSE_MODE KelvinSense, CHANNEL Channel) {
            ((AgE36200)SVI.Instrument).SCPI.SOURce.VOLTage.SENSe.SOURce.Command(Enum.GetName(typeof(SENSE_MODE), KelvinSense), Channels[Channel]);
        }
    }
}