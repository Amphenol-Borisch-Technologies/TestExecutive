using Ivi.Visa;
using Keysight.Visa;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public abstract class Instrument : IDisposable {
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPE InstrumentType { get; }
        public enum IDN_FIELD { Manufacturer, Model, SerialNumber, FirmwareRevision } // Example: "Keysight Technologies,E36103B,MY61001983,1.0.2-1.02".  
        private readonly IMessageBasedSession _iMessageBasedSession;
        private readonly Object _lock = new Object();
        private Boolean _disposed = false;

        public Instrument(String Address, String Detail, INSTRUMENT_TYPE InstrumentType) {
            _iMessageBasedSession = new ResourceManager().Open(Address) as IMessageBasedSession;
            _iMessageBasedSession.TimeoutMilliseconds = 5000;
            this.Address = Address;
            this.Detail = Detail;
            this.InstrumentType = InstrumentType;
        }

        public void Command(String ScpiCommand) { lock (_lock) { _iMessageBasedSession.FormattedIO.WriteLine(ScpiCommand); } }

        public void Command(Byte[] Bytes) { lock (_lock) { _iMessageBasedSession.RawIO.Write(Bytes); } }

        public String Query(String SCPI_Command) {
            lock (_lock) {
                _iMessageBasedSession.TerminationCharacterEnabled = true;
                _iMessageBasedSession.FormattedIO.WriteLine(SCPI_Command);
                return _iMessageBasedSession.FormattedIO.ReadLine().Trim();
            }
        }

        public Byte[] QueryBinaryBlockOfByte(String SCPI_Command) {
            lock (_lock) {
                _iMessageBasedSession.TerminationCharacterEnabled = false;
                _iMessageBasedSession.FormattedIO.WriteLine(SCPI_Command);
                return _iMessageBasedSession.FormattedIO.ReadBinaryBlockOfByte();
            }
        }

        public Byte[] QueryRawIO(String SCPI_Command) {
            lock (_lock) {
                _iMessageBasedSession.TerminationCharacterEnabled = false;
                _iMessageBasedSession.FormattedIO.WriteLine(SCPI_Command);
                return _iMessageBasedSession.RawIO.Read();
            }
        }

        public void Clear() { lock (_lock) { _iMessageBasedSession.Clear(); } }

        public (SELF_TEST_RESULT Result, String Message) SelfTests() {
            const String Test = "*TST?";
            Int32 PR = 15;
            StringBuilder Message = new StringBuilder();
            Message.AppendLine($"{nameof(SelfTests)}".PadRight(PR) + $": SCPI {Test}");
            Message.AppendLine($"{nameof(Instrument)}".PadRight(PR) + $": {GetType().Name}");
            Message.AppendLine($"{nameof(InstrumentType)}".PadRight(PR) + $": {InstrumentType}");
            Message.AppendLine($"{nameof(Detail)}".PadRight(PR) + $": {Detail}");
            Message.AppendLine($"{nameof(Address)}".PadRight(PR) + $": {Address}");
            Message.Append($"{nameof(SELF_TEST_RESULT)}".PadRight(PR));
            SELF_TEST_RESULT Result;
            try {
                Result = (SELF_TEST_RESULT)Int32.Parse(Query(Test));
            } catch (Exception exception) {
                Result = SELF_TEST_RESULT.EXCEPTION;
                Message.Insert(0, $"{exception.Message}{Environment.NewLine}");
                _ = MessageBox.Show($"{exception.Message}{Environment.NewLine}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            return (Result, Message.Append($": {Result}").ToString());
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            return (SelfTests().Result is SELF_TEST_RESULT.PASS, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: SelfTests().Message, Event: SelfTests().Result is SELF_TEST_RESULT.PASS ? EVENTS.PASS : EVENTS.FAIL) });
        }

        public String Identity(IDN_FIELD Property) {
            String Identity = IdentityQuery();
            return Identity.Split(',')[(Int32)Property];
        }

        ~Instrument() { Dispose(false); }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(Boolean disposing) {
            if (!_disposed) {
                if (disposing) _iMessageBasedSession?.Dispose();
                _disposed = true;
            }
        }

        #region SCPI99
        public void ClearStatusCommand() { Command("*CLS"); }

        public void EventStatusEnableCommand(Byte RegisterMask) { Command($"*ESE {RegisterMask}"); }

        public Byte EventStatusEnableQuery() { return Byte.Parse(Query("*ESE?").Substring(5)); }

        public Byte EventStatusRegisterQuery() { return Byte.Parse(Query("*ESR?").Substring(5)); }

        public String IdentityQuery() { return Query("*IDN?"); }

        public void OperationCompleteCommand() { Command($"*OPC"); }

        public Byte OperationCompleteQuery() { return Byte.Parse(Query("*OPC?").Substring(5)); }

        public void OperationCompleteQuery(String scpiCommand) { if (!Query("*OPC?").Equals("1")) throw new InvalidOperationException($"{Detail}, Address '{Address}' didn't complete SCPI command '{scpiCommand}'!"); }

        public void ServiceRequestEnableCommand(Byte RegisterMask) { Command($"*SRE {RegisterMask}"); }

        public Byte ServiceRequestEnableQuery() { return Byte.Parse(Query("*SRE?").Substring(5)); }

        public Byte StatusRegisterQuery() { return Byte.Parse(Query("*STB?").Substring(5)); }

        public String TestQuery() { return Query("*TST?").Substring(5); }

        public void ResetClear() {
            ResetCommand();
            ClearStatusCommand();
        }

        public void ResetCommand() { _iMessageBasedSession.FormattedIO.WriteLine("*RST"); }

        public void WaitCommand() { _iMessageBasedSession.FormattedIO.WriteLine("*WAI"); }
        #endregion
    }
}
