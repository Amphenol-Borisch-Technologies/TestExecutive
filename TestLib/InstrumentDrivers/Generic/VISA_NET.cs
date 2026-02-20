using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Keysight.Visa;
using System;
using System.Collections.Generic;
using System.Linq;
using static ABT.Test.TestExecutive.TestLib.InstrumentDrivers.WaveformGenerator.WS_3162_VISA_NET;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Generic {
    public class VISA_NET : IInstrument, IDiagnostics, IDisposable, IVISA_NET {
        public enum IDN_FIELD { Manufacturer, Model, SerialNumber, FirmwareRevision } // Example: "Keysight Technologies,E36103B,MY61001983,1.0.2-1.02".  
        public String Address { get; }
        public String Detail { get; }
        public UsbSession UsbSession;
        public INSTRUMENT_TYPE InstrumentType { get; set; }
        private Boolean _disposed = false;
        private Object _lock = new Object();

        public SELF_TEST_RESULT SelfTests() {
            try {
                UsbSession.FormattedIO.WriteLine("*TST?");
                if (TestQuery().Equals("0")) return SELF_TEST_RESULT.PASS;
                return SELF_TEST_RESULT.FAIL;
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULT.FAIL;
            }
        }
        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULT.PASS;
            return (SelfTests() is SELF_TEST_RESULT.PASS, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
        }

        public VISA_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPE.UNKNOWN;

            UsbSession = new UsbSession(Address) {
                TerminationCharacter = 0x0A,
                TerminationCharacterEnabled = true
            };
            ResetClear();
        }

        public String QueryLine(String scpiCommand) {
            lock (_lock) {
                UsbSession.TerminationCharacterEnabled = true;
                UsbSession.FormattedIO.WriteLine(scpiCommand);
                return UsbSession.FormattedIO.ReadLine().Trim();
            }
        }

        public Byte[] QueryBinaryBlockOfByte(String scpiCommand) {
            lock (_lock) {
                UsbSession.TerminationCharacterEnabled = false;
                UsbSession.FormattedIO.WriteLine(scpiCommand);
                return UsbSession.FormattedIO.ReadBinaryBlockOfByte();
            }
        }

        public Byte[] QueryRawIO(String scpiCommand) {
            lock (_lock) {
                UsbSession.TerminationCharacterEnabled = false;
                UsbSession.FormattedIO.WriteLine(scpiCommand);
                return UsbSession.RawIO.Read();
            }
        }

        public String Identity(IDN_FIELD Property) {
            String Identity = IdentityQuery();
            return Identity.Split(',')[(Int32)Property];
        }

        public static String Identity(Object Instrument, IDN_FIELD Property) {
            String Address = ((IInstrument)Instrument).Address;
            return Identity(Address, Property);
        }
        
        ~VISA_NET() { Dispose(false); }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing) {
            if (!_disposed) {
                if (disposing) UsbSession?.Dispose();
                _disposed = true;
            }
        }

        public void ClearStatusCommand() { UsbSession.FormattedIO.WriteLine("*CLS"); }
        public void EventStatusEnableCommand(Byte RegisterMask) { UsbSession.FormattedIO.WriteLine($"*ESE {RegisterMask}"); }
        public Byte EventStatusEnableQuery() { return Byte.Parse(QueryLine("*ESE?").Substring(5)); }
        public Byte EventStatusRegisterQuery() { return Byte.Parse(QueryLine("*ESR?").Substring(5)); }
        public String IdentityQuery() { return QueryLine("*IDN?"); }
        public void OperationCompleteCommand() { UsbSession.FormattedIO.WriteLine($"*OPC"); }
        public Byte OperationCompleteQuery() { return Byte.Parse(QueryLine("*OPC?").Substring(5)); }
        public void OperationCompleteQuery(String scpiCommand) { if (!QueryLine("*OPC?").Equals("1")) throw new InvalidOperationException($"{Detail}, Address '{Address}' didn't complete SCPI command '{scpiCommand}'!"); }
        public void ServiceRequestEnableCommand(Byte RegisterMask) { UsbSession.FormattedIO.WriteLine($"*SRE {RegisterMask}"); }
        public Byte ServiceRequestEnableQuery() { return Byte.Parse(QueryLine("*SRE?").Substring(5)); }
        public Byte StatusRegisterQuery() { return Byte.Parse(QueryLine("*STB?").Substring(5)); }
        public String TestQuery() { return QueryLine("*TST?").Substring(5); }
        public void ResetCommand() { UsbSession.FormattedIO.WriteLine("*RST"); }
        public void WaitCommand() { UsbSession.FormattedIO.WriteLine("*WAI"); }
        public void ResetClear() {
            ResetCommand();
            ClearStatusCommand();
        }
    }
}