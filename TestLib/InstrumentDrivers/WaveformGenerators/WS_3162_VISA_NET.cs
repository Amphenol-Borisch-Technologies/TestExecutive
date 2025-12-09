using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Keysight.Visa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.WaveformGenerator {
    // NOTE: WaveStation 2000/3000 SCPI Reference Manual https://cdn.teledynelecroy.com/files/manuals/wsta_scpi_manual_reva.pdf.
    // NOTE: Operator's Manual: WaveStation 3000 Function & Arbitrary Waveform Generator https://cdn.teledynelecroy.com/files/manuals/wavestation_3000_om.pdf.
    // TODO: Test below WaveStation 3162 commands & queries:
    //
    //  Short       Long            Subsystem   What It Does
    //  --------------------------------------------------------------
    //  *IDN?       *IDN            SYSTEM      Retrieves device identification information.
    //  *OPC        *OPC            SYSTEM      Sets the Event Status Register(ESR) OPC bit to TRUE(1).
    //  *CLS        *CLS            SYSTEM      Clears all status data registers
    //  *ESE        *ESE            SYSTEM      Sets the Standard Event Status Enable register(ESE)
    //  *ESR?       *ESR?           SYSTEM      Reads and clears the contents of the Event Status Register(ESR)
    //  *RST        *RST            SYSTEM      Initiates a device reset.
    //  *SRE        *SRE            SYSTEM      Sets the Service Request Enable register(SRE)
    //  *STB?       *STB?           SYSTEM      Reads the contents of the 488.1 defined status register(STB), and the Master Summary Status(MSS)
    //  *TST        *TST            SYSTEM      Performs an internal self-test.
    //  *WAI        *WAI            SYSTEM      Wait to continue command.
    //  BSWV        BASIC_WAVE      SIGNAL      Sets or retrieves basic wave parameters.
    //  BUZZ        BUZZER          SYSTEM      Sets or retrieves buzzer status.
    //  CHDR        COMM_HEADER                 Sets or retrieves the query return format.
    //  INVT        INVERT          SIGNAL      Sets or retrieves the phase of the output signal.
    //  OUTP        OUTPUT          SIGNAL      Sets or retrieves output state.
    //  PACP        CHANNEL_COPY    SIGNAL      Copies parameters from one channel to the other
    //  ROSC        ROSCILLATOR     SIGNAL      Sets or retrieves the clock source.
    //  SCFG        SYSTEM_CONFIG   SYSTEM      Sets or retrieves the state used (last or default) when powering on the WaveStation.
    //  SCSV        SCREEN_SAVE     SYSTEM      Sets screen saver on/off or retrieves screen saver status.
    //  VKEY        VIRTUAL_KEY     SYSTEM      Sends equivalent keyboard function to device.

    // TODO: Code & Test below WaveStation 3162 commands & queries:
    //
    //  Short       Long            Subsystem   What It Does
    //  --------------------------------------------------------------
    //  ARWV        ARBWAVE         SYSTEM      Sets the instrument to an arbitrary waveform or retrieves Arbitrary Waveform settings.
    //  BTWV        BURSTWAVE       SIGNAL      Sets instrument to a burst waveform or retrieves current Burst Wave settings.
    //  MDWV        MODULATEWAVE    SIGNAL      Sets instrument to a modulated waveform or retrieves current Modulate Wave settings.
    //  STL         STORE_LIST      SIGNAL      Retrieves all waveform names stored in WaveStation’s device memory.
    //  SWWV        SWEEP           SIGNAL      Sets instrument to sweep a waveform or retrieves Sweep Wave settings.
    //  SYNC        SYNC            SIGNAL      Sends a Sync pulse upon occurrence of the specified function.
    //  WVCSV       WAVE_CSV                    Saves.CSV file to user-defined memory location.

    public class WS_3162_VISA_NET : IInstrument, IDiagnostics, IDisposable {
        public enum CHANNELS { C1, C2 }
        public enum CLOCK_SOURCE { INT, EXT }
        public enum COMMAND_HEADERS { OFF, SHORT, LONG }
        public enum CONFIGURATIONS { DEFAULT, LAST }
        public enum MINUTES { OFF = 0, M1 = 1, M5 = 5, M15 = 15, M30 = 30, M60 = 60, M120 = 120, M300 = 300 }
        public enum OUTP { ON, OFF, LOAD_50, LOAD_HZ, PLRT_NOR, PLRT_INVT }
        public enum STATUSES { OFF, ON }
        public enum VIRTUAL_KEYS {
            KB_BURST = 17, KB_CHANNEL = 33, KB_FUNC1 = 28, KB_FUNC2 = 23, KB_FUNC3 = 18, KB_FUNC4 = 13,
            KB_FUNC5 = 8, KB_FUNC6 = 3, KB_HELP = 12, KB_KNOB_DOWN = 176, KB_KNOB_LEFT = 177, KB_KNOB_RIGHT = 175, KB_LEFT = 44,
            KB_MOD = 15, KB_NEGATIVE = 43, KB_NUMBER_0 = 48, KB_NUMBER_1 = 49, KB_NUMBER_2 = 50, KB_NUMBER_3 = 51, KB_NUMBER_4 = 52,
            KB_NUMBER_5 = 53, KB_NUMBER_6 = 54, KB_NUMBER_7 = 55, KB_NUMBER_8 = 56, KB_NUMBER_9 = 57, KB_OUTPUT1 = 153, KB_OUTPUT2 = 152,
            KB_PARAMETER = 5, KB_POINT = 46, KB_RIGHT = 40, KB_SWEEP = 16, KB_UTILITY = 11, KB_WAVES = 4
        }
        public class BasicWave {
            public enum COMMANDS { WVTP, FRQ, AMP, OFST, SYM, DUTY, PHSE, STDEV, MEAN, WIDTH, RISE, FALL, DLY }
            public enum QUERIES { WVTP, FRQ, PERI, AMP, OFST, HLEV, LLEV, PHSE, DUTY }
            public enum WVTP { SINE, SQUARE, RAMP, PULSE, NOISE, ARB, DC }
        }

        public UsbSession UsbSession;
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }
        public SELF_TEST_RESULTS SelfTests() {
            try {
                UsbSession.FormattedIO.WriteLine("*TST?");
                if (TestQuery().Equals("0")) return SELF_TEST_RESULTS.PASS;
                return SELF_TEST_RESULTS.FAIL;
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULTS.FAIL;
            }
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_3162 = (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                // TODO: Eventually; add verification measurements of the WS-3162 waveform generator using external instrumentation.
            }
            return result_3162;
        }

        public WS_3162_VISA_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.WAVEFORM_GENERATOR;
            UsbSession = new UsbSession(Address) {
                TerminationCharacter = 0x0a,
                TerminationCharacterEnabled = true
            };
            ResetCommand();
            ClearStatusCommand();
            CommandHeaderCommand(COMMAND_HEADERS.LONG);
            ScreenSaveCommand(MINUTES.M5);
            BuzzerCommand(STATUSES.ON);
        }

        public String QueryLine(String scpiCommand) {
            UsbSession.TerminationCharacterEnabled = true;
            UsbSession.FormattedIO.WriteLine(scpiCommand);
            return UsbSession.FormattedIO.ReadLine().Trim();
        }

        public Byte[] QueryBinaryBlockOfByte(String scpiCommand) {
            UsbSession.TerminationCharacterEnabled = false;
            UsbSession.FormattedIO.WriteLine(scpiCommand);
            return UsbSession.FormattedIO.ReadBinaryBlockOfByte();
        }

        public Byte[] QueryRawIO() {
            UsbSession.TerminationCharacterEnabled = false;
            return UsbSession.RawIO.Read();
        }

        public void BasicWaveCommand(CHANNELS Channel, BasicWave.COMMANDS Command, Object Parameter) {
            BasicWave.WVTP wvtp = (BasicWave.WVTP)Enum.Parse(typeof(BasicWave.WVTP), BasicWaveQuery(Channel, BasicWave.QUERIES.WVTP));
            switch (Command) {
                case BasicWave.COMMANDS.WVTP: {
                        if (Enum.IsDefined(typeof(BasicWave.WVTP), Parameter.ToString())) UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe WVTP,{Parameter}");
                        else {
                            BasicWave.WVTP[] waveEnum = (BasicWave.WVTP[])Enum.GetValues(typeof(BasicWave.WVTP));
                            String waveTypes = "{ " + String.Join(", ", waveEnum.Select(wt => wt.ToString())) + " }";
                            throw new ArgumentException($"Wavetype '{Parameter}' must be in set '{waveTypes}'.");
                        }
                        break;
                    }
                case BasicWave.COMMANDS.FRQ: {
                        if (wvtp == BasicWave.WVTP.NOISE) throw new ArgumentException("Frequency invalid for WVTP = NOISE.");
                        if (Double.TryParse(Parameter.ToString(), out Double hertzFRQ)) {
                            if (hertzFRQ < 1E-6 || hertzFRQ > 160E6) throw new ArgumentOutOfRangeException($"Frequency '{hertzFRQ}' must be between 1E-6 and 160E6 Hertz.");
                            UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe FRQ,{hertzFRQ}HZ");
                        } else throw new ArgumentException(nameof(Parameter), $"Frequency '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.AMP: {
                        if (wvtp == BasicWave.WVTP.NOISE) throw new ArgumentException("Amplifier invalid for WVTP = NOISE.");
                        if (Double.TryParse(Parameter.ToString(), out Double voltsAMP)) {
                            if (voltsAMP < 2E-3 || voltsAMP > 2E1) throw new ArgumentOutOfRangeException($"Amplifier voltage '{voltsAMP}' must be between 2E-3 and 2E1 Volts.");
                            UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe AMP,{voltsAMP}V");
                        } else throw new ArgumentException(nameof(Parameter), $"Amplifier voltage '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.OFST: {
                        if (wvtp == BasicWave.WVTP.NOISE) throw new ArgumentException("Offset invalid for WVTP = NOISE.");
                        if (Double.TryParse(Parameter.ToString(), out Double voltsOFST)) {
                            Double voltsAMP = Double.Parse(BasicWaveQuery(Channel, BasicWave.QUERIES.AMP).Replace("V", ""));
                            if (voltsOFST > (voltsAMP / 2)) throw new ArgumentOutOfRangeException($"Offset voltage '{voltsOFST}' must be ≤ amplitude/2 '{voltsAMP / 2}'.");
                            if (Math.Abs(voltsOFST) + (voltsAMP / 2) > 10) throw new ArgumentOutOfRangeException($"Offset '{voltsOFST}' and amplitude '{voltsAMP}' combination must be within ± 10 Volts.");
                            UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe OFST,{voltsOFST}V");
                        } else throw new ArgumentException(nameof(Parameter), $"Offset voltage '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.SYM: {
                        if (wvtp != BasicWave.WVTP.RAMP) throw new ArgumentException("Symmetry invalid for WVTP ≠ RAMP.");
                        if (Double.TryParse(Parameter.ToString(), out Double symmetry)) {
                            if (symmetry < 0 || symmetry > 100) throw new ArgumentOutOfRangeException($"Symmetry '{symmetry}' must be between 0 and 100.");
                            UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe SYM,{symmetry}");
                        } else throw new ArgumentException(nameof(Parameter), $"Symmetry '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.DUTY: {
                        if (Double.TryParse(Parameter.ToString(), out Double duty)) {
                            switch (wvtp) {
                                case BasicWave.WVTP.PULSE:
                                    if (duty < 0.001 || duty > 0.999) throw new ArgumentOutOfRangeException($"Duty cycle '{duty}' must be between 0.001 and 0.999 for WVTP = PULSE.");
                                    UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe DUTY,{duty * 100}%");
                                    break;
                                case BasicWave.WVTP.SQUARE:
                                    if (duty < 0.2 || duty > 0.8) throw new ArgumentOutOfRangeException($"Duty cycle '{duty}' must be between 0.2 and 0.8 for WVTP = SQUARE.");
                                    UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe DUTY,{duty * 100}%");
                                    break;
                                default:
                                    throw new ArgumentException($"Duty cycle invalid for WVTP '{wvtp}'; must be PULSE or SQUARE.");
                            }
                        } else throw new ArgumentException(nameof(Parameter), $"Duty cycle '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.PHSE: {
                        if (wvtp == BasicWave.WVTP.NOISE) throw new ArgumentException("Phase invalid for WVTP = NOISE.");
                        if (Double.TryParse(Parameter.ToString(), out Double phaseDegrees)) {
                            if (phaseDegrees < 0 || phaseDegrees > 360) throw new ArgumentOutOfRangeException($"Phase '{phaseDegrees}' must be between 0 and 360°.");
                            UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe PHSE,{phaseDegrees}");
                        } else throw new ArgumentException(nameof(Parameter), $"Phase '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.STDEV: {
                        if (wvtp != BasicWave.WVTP.NOISE) throw new ArgumentException("Standard deviation invalid for WVTP ≠ NOISE.");
                        if (Double.TryParse(Parameter.ToString(), out Double stdevVolts)) {
                            if (stdevVolts < 0.0005 || stdevVolts > 1.599) throw new ArgumentOutOfRangeException($"Standard deviation voltage '{stdevVolts}' must be between 0.0005 and 1.599 volts.");
                            UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe STDEV,{stdevVolts}V");
                        } else throw new ArgumentException(nameof(Parameter), $"Standard deviation '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.MEAN: {
                        if (wvtp != BasicWave.WVTP.NOISE) throw new ArgumentException("Mean invalid for WVTP ≠ NOISE.");
                        if (Double.TryParse(Parameter.ToString(), out Double meanVolts)) {
                            // TODO: if (meanVolts < 0.0005 || meanVolts > 1.599) throw new ArgumentOutOfRangeException($"Mean voltage '{meanVolts}' must be between 0.0005 and 1.599 volts.");
                            UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe MEAN,{meanVolts}V");
                        } else throw new ArgumentException(nameof(Parameter), $"Mean '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.WIDTH: {
                        if (wvtp != BasicWave.WVTP.PULSE) throw new ArgumentException("Width invalid for WVTP ≠ PULSE.");
                        if (Double.TryParse(Parameter.ToString(), out Double width)) UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe WIDTH,{width}");
                        else throw new ArgumentException(nameof(Parameter), $"Width '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.RISE: {
                        if (wvtp != BasicWave.WVTP.PULSE) throw new ArgumentException("Rise invalid for WVTP ≠ PULSE.");
                        if (Double.TryParse(Parameter.ToString(), out Double rise)) UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe RISE,{rise}");
                        else throw new ArgumentException(nameof(Parameter), $"Rise '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.FALL: {
                        if (wvtp != BasicWave.WVTP.PULSE) throw new ArgumentException("Fall invalid for WVTP ≠ PULSE.");
                        if (Double.TryParse(Parameter.ToString(), out Double fall)) UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe FALL,{fall}");
                        else throw new ArgumentException(nameof(Parameter), $"Fall '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                case BasicWave.COMMANDS.DLY: {
                        if (wvtp != BasicWave.WVTP.PULSE) throw new ArgumentException("Delay invalid for WVTP ≠ PULSE.");
                        if (Double.TryParse(Parameter.ToString(), out Double delaySeconds)) {
                            Double periodSeconds = Double.Parse(BasicWaveQuery(Channel, BasicWave.QUERIES.PERI).Replace("S", ""));
                            if (delaySeconds < 0 || delaySeconds > periodSeconds) throw new ArgumentOutOfRangeException($"Delay '{delaySeconds}' must be between 0 and '{periodSeconds}' seconds.");
                            UsbSession.FormattedIO.WriteLine($"{Channel}:BaSic_WaVe DLY,{delaySeconds}S");
                        } else throw new ArgumentException(nameof(Parameter), $"Delay '{Parameter}' must be of type '{typeof(Double)}'.");
                        break;
                    }
                default: throw new ArgumentException($"BasicWaveCommand '{Command}' not coded yet.");
            }
        }
        public String BasicWaveQuery(CHANNELS Channel) { return QueryLine($"{Channel}:BaSic_WaVe?"); }
        public String BasicWaveQuery(CHANNELS Channel, BasicWave.QUERIES Query) {
            String response = BasicWaveQuery(Channel);                  // C1:BASIC_WAVE WVTP,SQUARE,FRQ,1e+07HZ,PERI,1e-07S,AMP,1V,OFST,0.5V,HLEV,1V,LLEV,0V,PHSE,0,DUTY,50
            response = response.Substring(response.IndexOf(' ') + 1);   // WVTP,SQUARE,FRQ,1e+07HZ,PERI,1e-07S,AMP,1V,OFST,0.5V,HLEV,1V,LLEV,0V,PHSE,0,DUTY,50
            List<String> responses = response.Split(',').ToList();
            return responses[responses.IndexOf(Query.ToString()) + 1];
        }
        public void BuzzerCommand(STATUSES Status) { UsbSession.FormattedIO.WriteLine($"BUZZer {Status}"); }
        public STATUSES BuzzerQuery() {
            String response = QueryLine("BUZZer?");
            return (STATUSES)Enum.Parse(typeof(STATUSES), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void ChannelParameterCopyCommand(CHANNELS ChannelSource) { UsbSession.FormattedIO.WriteLine($"PAraCoPy {(ChannelSource == CHANNELS.C1 ? CHANNELS.C2 : CHANNELS.C1)},{ChannelSource}"); }
        public void ClearStatusCommand() { UsbSession.FormattedIO.WriteLine("*CLS"); }
        public void ClockSourceCommand(CLOCK_SOURCE ClockSource) { UsbSession.FormattedIO.WriteLine($"ROSCillator {ClockSource}"); }
        public CLOCK_SOURCE ClockSourceQuery() {
            String response = QueryLine("ROSCillator?");
            return (CLOCK_SOURCE)Enum.Parse(typeof(CLOCK_SOURCE), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void CommandHeaderCommand(COMMAND_HEADERS CommandHeader) { UsbSession.FormattedIO.WriteLine($"*Comm_HeaDeR {CommandHeader}"); }
        public COMMAND_HEADERS CommandHeaderQuery() {
            String response = QueryLine("*Comm_HeaDeR?");
            return (COMMAND_HEADERS)Enum.Parse(typeof(COMMAND_HEADERS), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void ConfigurationCommand(CONFIGURATIONS Configuration) { UsbSession.FormattedIO.WriteLine($"Sys_CFG {Configuration}"); }
        public CONFIGURATIONS ConfigurationQuery() {
            String response = QueryLine("Sys_CFG?");
            return (CONFIGURATIONS)Enum.Parse(typeof(CONFIGURATIONS), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void EventStatusEnableCommand(Byte RegisterMask) { UsbSession.FormattedIO.WriteLine($"*ESE {RegisterMask}"); }
        public Byte EventStatusEnableQuery() { return Byte.Parse(QueryLine("*ESE?").Substring(5)); }
        public Byte EventStatusRegisterQuery() { return Byte.Parse(QueryLine("*ESR?").Substring(5)); }
        public String IdentityQuery() { return QueryLine("*IDN?"); }
        public void InvertCommand(STATUSES Status) { UsbSession.FormattedIO.WriteLine($"INVerT {Status}"); }
        public STATUSES InvertQuery() {
            String response = QueryLine("INVerT?");
            return (STATUSES)Enum.Parse(typeof(STATUSES), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public void OperationCompleteCommand() { UsbSession.FormattedIO.WriteLine($"*OPC"); }
        public Byte OperationCompleteQuery() { return Byte.Parse(QueryLine("*OPC?").Substring(5)); }
        public void OperationCompleteQuery(String scpiCommand) { if (!QueryLine("*OPC?").Equals("1")) throw new InvalidOperationException($"{Detail}, Address '{Address}' didn't complete SCPI command '{scpiCommand}'!"); }
        public void OutputCommand(CHANNELS Channel, OUTP Output) { UsbSession.FormattedIO.WriteLine($"{Channel}:OUTPut {Output.ToString().Replace('_', ',')}"); }
        public String OutputQuery(CHANNELS Channel) { return QueryLine($"{Channel}:OUTPut?"); }
        public void ScreenSaveCommand(MINUTES Minutes) {
            if (Minutes == MINUTES.OFF) UsbSession.FormattedIO.WriteLine($"SCreen_SaVe {Minutes}");
            else UsbSession.FormattedIO.WriteLine($"SCreen_SaVe {(Int32)Minutes}");
        }
        public String ScreenSaveQuery() {
            String response = QueryLine("SCreen_SaVe?");
            return response.Substring(response.IndexOf(" ") + 1);
        }
        public void ServiceRequestEnableCommand(Byte RegisterMask) { UsbSession.FormattedIO.WriteLine($"*SRE {RegisterMask}"); }
        public Byte ServiceRequestEnableQuery() { return Byte.Parse(QueryLine("*SRE?").Substring(5)); }
        public Byte StatusRegisterQuery() { return Byte.Parse(QueryLine("*STB?").Substring(5)); }
        public void SynchronizeCommand(CHANNELS Channel, STATUSES Status) { UsbSession.FormattedIO.WriteLine($"{Channel}:SYNC {Status}"); }
        public CHANNELS SynchronizeQuery() {
            String response = QueryLine("SYNC?");
            return (CHANNELS)Enum.Parse(typeof(CHANNELS), response.Substring(response.IndexOf(" ") + 1), true);
        }
        public String TestQuery() { return QueryLine("*TST?").Substring(5); }
        public void VirtualKeyCommand(VIRTUAL_KEYS VirtualKey) { UsbSession.FormattedIO.WriteLine($"VKEY VALUE,{VirtualKey},STATE,1"); }

        public void ResetCommand() { UsbSession.FormattedIO.WriteLine("*RST"); }
        public void WaitCommand() { UsbSession.FormattedIO.WriteLine("*WAI"); }
        public void ResetClear() {
            ResetCommand();
            ClearStatusCommand();
        }

        ~WS_3162_VISA_NET() { Dispose(); }

        public void Dispose() { UsbSession.Dispose(); }
    }
}
