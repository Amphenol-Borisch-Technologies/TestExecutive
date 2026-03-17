using ABT.Test.TestExecutive.TestLib.Configuration;
using Ivi.Visa;
using Keysight.Visa;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public abstract class InstrumentDriver : IDisposable {
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPE InstrumentType { get; }
        public enum IDN_FIELD { Manufacturer, Model, SerialNumber, FirmwareRevision } // Example: "Keysight Technologies,E36103B,MY61001983,1.0.2-1.02".
        private readonly IMessageBasedSession _iMessageBasedSession;
        private readonly Object _lock = new Object();
        private Boolean _disposed = false;
        private Boolean _terminationCharacterEnabled;

        public InstrumentDriver(String Address, String Detail, INSTRUMENT_TYPE InstrumentType) {
            // TODO: TestExecutive to create a single instance of ResourceManager and pass it to each InstrumentDriver, instead of each InstrumentDriver creating its own temporary instance of ResourceManager.
            _iMessageBasedSession = new ResourceManager().Open(Address) as IMessageBasedSession;
            _iMessageBasedSession.TimeoutMilliseconds = 5000;
            _terminationCharacterEnabled = _iMessageBasedSession.TerminationCharacterEnabled;
            this.Address = Address;
            this.Detail = Detail;
            this.InstrumentType = InstrumentType;
        }

        public void Command(String ScpiCommand) { Command(_iMessageBasedSession.FormattedIO.WriteLine, ScpiCommand); }

        public void Command(Byte[] Bytes) { Command(_iMessageBasedSession.RawIO.Write, Bytes); }

        private void Command<TParam>(Action<TParam> WriteMethod, TParam ScpiCommand) {
            lock (_lock) {
                _terminationCharacterEnabled = _iMessageBasedSession.TerminationCharacterEnabled;
                _iMessageBasedSession.TerminationCharacterEnabled = true;
                WriteMethod(ScpiCommand);
                _iMessageBasedSession.TerminationCharacterEnabled = _terminationCharacterEnabled;
            }
        }

        public String Query(String ScpiQuery) {
            lock (_lock) {
                _terminationCharacterEnabled = _iMessageBasedSession.TerminationCharacterEnabled;
                _iMessageBasedSession.TerminationCharacterEnabled = true;
                _iMessageBasedSession.FormattedIO.WriteLine(ScpiQuery);
                String s = _iMessageBasedSession.FormattedIO.ReadLine().Trim();
                _iMessageBasedSession.TerminationCharacterEnabled = _terminationCharacterEnabled;
                return s;
            }
        }

        public T Query<T>(String ScpiQuery) {
            String raw = null;
            String Raw() => raw ?? (raw = Query(ScpiQuery));
            Double d;
            switch (typeof(T)) {
                case Type t when t.IsEnum: return (T)Enum.Parse(typeof(T), Raw(), ignoreCase: true);
                case Type t when t == typeof(Boolean): return (T)(Object)ParseBoolean(Raw(), ScpiQuery);
                case Type t when t == typeof(Byte): return (T)(Object)Byte.Parse(Raw(), CultureInfo.InvariantCulture);
                case Type t when t == typeof(Double?): return (T)(Object)(Double.TryParse(Raw(), NumberStyles.Any, CultureInfo.InvariantCulture, out d) ? d : (Double?)null);
                case Type t when t == typeof(Int32): return (T)(Object)Int32.Parse(Raw(), CultureInfo.InvariantCulture);
                case Type t when t == typeof(String): return (T)(Object)Raw();
                case Type t when t == typeof(Boolean[]): return (T)(Object)ParseBooleans(Raw(), ScpiQuery);
                case Type t when t == typeof(Byte[]): return (T)(Object)Raw().Split(',').Select(s => Byte.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                case Type t when t == typeof(Double[]): return (T)(Object)Raw().Split(',').Select(s => Double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d) ? d : (Double?)null).ToArray();
                case Type t when t == typeof(Int32[]): return (T)(Object)Raw().Split(',').Select(s => Int32.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                case Type t when t == typeof(String[]): return (T)(Object)Raw().Split(',').Select(s => s.Trim()).ToArray();
                default: throw new NotSupportedException($"Type '{typeof(T)}' is not yet supported for SCPI queries.");
            }
        }

        private Boolean ParseBoolean(String ScpiResponse, String ScpiQuery) {
            ScpiResponse = ScpiResponse.ToUpperInvariant();
            if (ScpiResponse == "0" || ScpiResponse == "OFF" || ScpiResponse == "FALSE") return false;
            if (ScpiResponse == "1" || ScpiResponse == "ON" || ScpiResponse == "TRUE") return true;
            throw new FormatException($"Cannot parse '{ScpiResponse}' as Boolean in response from query {ScpiQuery}.");
        }

        private Boolean[] ParseBooleans(String ScpiResponse, String ScpiQuery) {
            String[] parts = ScpiResponse.Split(',').Select(ba => ba.Trim().ToUpperInvariant()).ToArray();
            Boolean[] booleans = new Boolean[parts.Length];
            for (Int32 i = 0; i < parts.Length; i++) booleans[i] = ParseBoolean(parts[i], ScpiQuery);
            return booleans;
        }

        public Byte[] QueryBinaryBlockOfByte(String ScpiQuery) { return QueryBinary(ScpiQuery, () => _iMessageBasedSession.FormattedIO.ReadBinaryBlockOfByte()); }

        public Byte[] QueryRawIO(String ScpiQuery) { return QueryBinary(ScpiQuery, () =>_iMessageBasedSession.RawIO.Read()); }

        private Byte[] QueryBinary(String ScpiQuery, Func<Byte[]> ReadFunction) {
            lock (_lock) {
                _terminationCharacterEnabled = _iMessageBasedSession.TerminationCharacterEnabled;
                _iMessageBasedSession.TerminationCharacterEnabled = false;
                _iMessageBasedSession.FormattedIO.WriteLine(ScpiQuery);
                Byte[] response = ReadFunction();
                _iMessageBasedSession.TerminationCharacterEnabled = _terminationCharacterEnabled;
                return response;
            }
        }

        public void Clear() { lock (_lock) { _iMessageBasedSession.Clear(); } }

        public (SELF_TEST_RESULT Result, String Message) SelfTests() {
            const String Test = "*TST?";
            Int32 PR = 15;
            StringBuilder Message = new StringBuilder();
            Message.AppendLine($"{nameof(SelfTests)}".PadRight(PR) + $": SCPI {Test}");
            Message.AppendLine($"{nameof(InstrumentDriver)}".PadRight(PR) + $": {GetType().Name}");
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

        public String Identity(IDN_FIELD Property) {
            String Identity = IdentityQuery();
            return Identity.Split(',')[(Int32)Property];
        }

        ~InstrumentDriver() { Dispose(false); }

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
