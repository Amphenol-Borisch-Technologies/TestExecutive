using Agilent.CommandExpert.ScpiNet.Ag33500B_33600A_2_09;
using System;

// All Agilent.CommandExpert.ScpiNet drivers are procured by adding new SCPI VISA Instruments in Keysight's Command Expert app software.
//  - Command Expert literally downloads & installs Agilent.CommandExpert.ScpiNet drivers when new SVIs are added.
//  - The Agilent.CommandExpert.ScpiNet drivers are installed into folder C:\ProgramData\Keysight\Command Expert\ScpiNetDrivers.
// https://www.keysight.com/us/en/lib/software-detail/computer-software/command-expert-downloads-2151326.html
//
// Enthusiastically recommend using Command Expert to generate SCPI commands, which are directly exportable as .Net statements.
// https://www.keysight.com/us/en/search.html/command+expert
//
namespace ABT.TestSpace.TestExec.SCPI_VISA_Instruments {
    public static class WG_33509B {
        public const String MODEL = "33509B";

        public const Boolean LoadOrStimulus = true;

        public static void FunctionSquare(SCPI_VISA_Instrument SVI, Double DutyCyclePercent, Double Hz) {
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.FUNCtion.SQUare.DCYCle.Command(null, DutyCyclePercent, "PCT");
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.FUNCtion.SQUare.PERiod.Command(null, 1 / Hz);
        }

        public static Boolean IsWG_33509(SCPI_VISA_Instrument SVI) { return (SVI.Instrument.GetType() == typeof(Ag33500B_33600A)); }

        public static void Initialize(SCPI_VISA_Instrument SVI) {
            // NOTE:  Mustn't invoke TestExecutive.CT_EmergencyStop.ThrowIfCancellationRequested(); on Initialize() or it's invoked methods Reset() & Clear().
            SCPI99.Initialize(SVI);
            ((Ag33500B_33600A)SVI.Instrument).SCPI.DISPlay.TEXT.CLEar.Command();
        }

        public static STATE Get(SCPI_VISA_Instrument SVI) {
            ((Ag33500B_33600A)SVI.Instrument).SCPI.OUTPut.Query(null, out Boolean State);
            return State ? STATE.ON : STATE.off;
        }

        public static Boolean Is(SCPI_VISA_Instrument SVI, STATE State) {
            return (State == Get(SVI));
        }

        public static void Set(SCPI_VISA_Instrument SVI, STATE State) {
            ((Ag33500B_33600A)SVI.Instrument).SCPI.OUTPut.Command(null, (State is STATE.ON));
        }

        public static void SetVoltage(SCPI_VISA_Instrument SVI, Double V_Low, Double V_High, Double V_Offset) {
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.VOLTage.LOW.Command(null, V_Low);
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.VOLTage.HIGH.Command(null, V_High);
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.VOLTage.OFFSet.Command(null, V_Offset);
        }

        public static void ModulateFM_SquareWave(SCPI_VISA_Instrument SVI, Double HzDeviation, Double HzFM, STATE State) {
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.FM.INTernal.FUNCtion.Command(null, "SQUare");
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.FM.INTernal.FREQuency.Command(null, HzFM, "HZ");
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.FM.DEViation.Command(null, HzDeviation, "HZ");
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.FM.SOURce.Command(null, "INTernal");
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.FM.STATe.Command(null, (State is STATE.ON));
        }

        public static String WaveformGet(SCPI_VISA_Instrument SVI) {
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.APPLy.Query(null, out String Waveform);
            return Waveform;
        }

        public static void WaveformSquareApply(SCPI_VISA_Instrument SVI, Double Hz, Double V_High, Double V_Offset) {
            ((Ag33500B_33600A)SVI.Instrument).SCPI.OUTPut.LOAD.Command(null, "INFinity");
            ((Ag33500B_33600A)SVI.Instrument).SCPI.SOURce.APPLy.SQUare.Command(null, Hz, "HZ", V_High, "V", V_Offset, "V");
        }
    }
}