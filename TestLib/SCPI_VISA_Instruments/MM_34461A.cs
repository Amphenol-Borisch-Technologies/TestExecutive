using System;
using System.Windows.Forms;
using Agilent.CommandExpert.ScpiNet.Ag3466x_2_08;
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

public enum PROPERTY { AmperageAC, AmperageDC, Capacitance, Continuity, Frequency, Fresistance, Period, Resistance, Temperature, VoltageAC, VoltageDC, VoltageDiodic }

    public static class MM_34461A {
        public const String MODEL = "34461A";

        public const Boolean LoadOrStimulus = false;

        public static Boolean IsMM_34461A(SCPI_VISA_Instrument SVI) { return (SVI.Instrument.GetType() == typeof(Ag3466x)); }

        public static void DelayAutoSet(SCPI_VISA_Instrument SVI, Boolean state) {
            ((Ag3466x)SVI.Instrument).SCPI.TRIGger.DELay.AUTO.Command(state);
        }
                
        public static Boolean DelayAutoIs(SCPI_VISA_Instrument SVI) {
            ((Ag3466x)SVI.Instrument).SCPI.TRIGger.DELay.AUTO.Query(out Boolean state);
            return state;
        }

        public static void DelaySet(SCPI_VISA_Instrument SVI, MMD mmd) {
            ((Ag3466x)SVI.Instrument).SCPI.TRIGger.DELay.Command(Enum.GetName(typeof(MMD), mmd));
        }

        public static void DelaySet(SCPI_VISA_Instrument SVI, Double Seconds) {
            ((Ag3466x)SVI.Instrument).SCPI.TRIGger.DELay.Command(Seconds);
        }

        public static Double DelayGet(SCPI_VISA_Instrument SVI) {
            ((Ag3466x)SVI.Instrument).SCPI.TRIGger.DELay.Query(MINimum, out Double seconds);
            return seconds;
        }

        public static Double Get(SCPI_VISA_Instrument SVI, PROPERTY property) {
            // SCPI FORMAT:DATA(ASCii/REAL) command unavailable on KS 34461A.
            switch (property) {
                case PROPERTY.AmperageAC:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.CURRent.AC.QueryAsciiReal(AUTO, DEFault, out Double acCurrent);
                    return acCurrent;
                case PROPERTY.AmperageDC:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.CURRent.DC.QueryAsciiReal(AUTO, DEFault, out Double dcCurrent);
                    return dcCurrent;
                case PROPERTY.Capacitance:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.CAPacitance.QueryAsciiReal(AUTO, DEFault, out Double capacitance);
                    return capacitance;
                case PROPERTY.Continuity:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.CONTinuity.QueryAsciiReal(out Double continuity);
                    return continuity;
                case PROPERTY.Frequency:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.FREQuency.QueryAsciiReal(AUTO, DEFault, out Double frequency);
                    return frequency;
                case PROPERTY.Fresistance:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.FRESistance.QueryAsciiReal(AUTO, DEFault, out Double fresistance);
                    return fresistance;
                case PROPERTY .Period:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.PERiod.QueryAsciiReal(AUTO, DEFault, out Double period);
                    return period;
                case PROPERTY.Resistance:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.RESistance.QueryAsciiReal(AUTO, DEFault, out Double resistance);
                    return resistance;
                case PROPERTY.Temperature:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.TEMPerature.QueryAsciiReal(AUTO, DEFault, null, null, out Double temperature);
                    return temperature;
                case PROPERTY.VoltageAC:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.VOLTage.AC.QueryAsciiReal(AUTO, DEFault, out Double acVoltage);
                    return acVoltage;
                case PROPERTY.VoltageDC:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.VOLTage.DC.QueryAsciiRealClone(AUTO, DEFault, out Double dcVoltage);
                    return dcVoltage;
                case PROPERTY .VoltageDiodic:
                    ((Ag3466x)SVI.Instrument).SCPI.MEASure.DIODe.QueryAsciiReal(out Double diodeVoltage);
                    return diodeVoltage;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void Initialize(SCPI_VISA_Instrument SVI) {
            // NOTE:  Mustn't invoke TestExecutive.CT_EmergencyStop.ThrowIfCancellationRequested(); on Initialize() or it's invoked methods Reset() & Clear().
            SCPI99.Initialize(SVI);
        }

        public static void TerminalsSetRear(SCPI_VISA_Instrument SVI) {
            if (TerminalsGet(SVI) == TERMINAL.Front) _ = MessageBox.Show("Please depress Keysight 34461A Front/Rear button.", "Paused, click OK to continue.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ((Ag3466x)SVI.Instrument).SCPI.TRIGger.DELay.Command(Enum.GetName(typeof(MMD), MMD.DEFault));            
            ((Ag3466x)SVI.Instrument).SCPI.TRIGger.DELay.AUTO.Command(true);
        }

        public static TERMINAL TerminalsGet(SCPI_VISA_Instrument SVI) {
            ((Ag3466x)SVI.Instrument).SCPI.ROUTe.TERMinals.Query(out String terminals);
            return String.Equals(terminals, "REAR") ? TERMINAL.Rear : TERMINAL.Front;
        }
    }
}