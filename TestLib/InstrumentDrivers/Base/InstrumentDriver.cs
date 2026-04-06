using Ivi.Visa;
using Keysight.Visa;
using System;
using System.Globalization;
using System.Linq;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public class InstrumentDriver : IDisposable, IInstrument {
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

        public void Command(String ScpiCommand) {
            ThrowIfDisposed();
            Command(_iMessageBasedSession.FormattedIO.WriteLine, ScpiCommand);
        }

        public void Command(Byte[] Bytes) {
            ThrowIfDisposed();
            Command(_iMessageBasedSession.RawIO.Write, Bytes);
        }

        private void Command<TParam>(Action<TParam> WriteMethod, TParam ScpiCommand) {
            ThrowIfDisposed();
            lock (_lock) {
                _terminationCharacterEnabled = _iMessageBasedSession.TerminationCharacterEnabled;
                try {
                    _iMessageBasedSession.TerminationCharacterEnabled = true;
                    WriteMethod(ScpiCommand);
                } finally {
                    _iMessageBasedSession.TerminationCharacterEnabled = _terminationCharacterEnabled;
                }
            }
        }

        public String Query(String ScpiQuery) {
            ThrowIfDisposed();
            lock (_lock) {
                _terminationCharacterEnabled = _iMessageBasedSession.TerminationCharacterEnabled;
                String s;
                try {
                    _iMessageBasedSession.TerminationCharacterEnabled = true;
                    _iMessageBasedSession.FormattedIO.WriteLine(ScpiQuery);
                    s = _iMessageBasedSession.FormattedIO.ReadLine().Trim();
                } finally {
                    _iMessageBasedSession.TerminationCharacterEnabled = _terminationCharacterEnabled;
                }
                return s.Trim();
            }
        }

        public T Query<T>(String ScpiQuery) {
            ThrowIfDisposed();
            String raw = null;
            String Raw() => raw ?? (raw = Query(ScpiQuery));
            switch (typeof(T)) {
                case Type t when t.IsEnum: return (T)Enum.Parse(typeof(T), Raw(), ignoreCase: true);
                case Type t when t == typeof(Boolean): return (T)(Object)ParseBoolean(Raw(), ScpiQuery);
                case Type t when t == typeof(Byte): return (T)(Object)Byte.Parse(Raw(), CultureInfo.InvariantCulture);
                case Type t when t == typeof(Double): return (T)(Object)(Double.Parse(Raw(), NumberStyles.Any, CultureInfo.InvariantCulture));
                case Type t when t == typeof(Int32): return (T)(Object)Int32.Parse(Raw(), CultureInfo.InvariantCulture);
                case Type t when t == typeof(String): return (T)(Object)Raw();
                case Type t when t == typeof(Boolean[]): return (T)(Object)ParseBooleans(Raw(), ScpiQuery);
                case Type t when t == typeof(Byte[]): return (T)(Object)Raw().Split(',').Select(s => Byte.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                case Type t when t == typeof(Double[]): return (T)(Object)Raw().Split(',').Select(s => Double.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture)).ToArray();
                case Type t when t == typeof(Int32[]): return (T)(Object)Raw().Split(',').Select(s => Int32.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                case Type t when t == typeof(String[]): return (T)(Object)Raw().Split(',').Select(s => s.Trim()).ToArray();
                default: throw new InstrumentException($"Type '{typeof(T)}' is not yet supported for SCPI queries.", Address, Detail, ScpiQuery);
            }
        }

        private Boolean ParseBoolean(String ScpiResponse, String ScpiQuery) {
            ScpiResponse = ScpiResponse.ToUpperInvariant();
            if (ScpiResponse == "0" || ScpiResponse == "OFF" || ScpiResponse == "FALSE") return false;
            if (ScpiResponse == "1" || ScpiResponse == "ON" || ScpiResponse == "TRUE") return true;
            throw new InstrumentException($"Cannot parse '{ScpiResponse}' as Boolean in response from query {ScpiQuery}.", Address, Detail, ScpiQuery);
        }

        private Boolean[] ParseBooleans(String ScpiResponse, String ScpiQuery) {
            String[] parts = ScpiResponse.Split(',').Select(ba => ba.Trim().ToUpperInvariant()).ToArray();
            Boolean[] booleans = new Boolean[parts.Length];
            for (Int32 i = 0; i < parts.Length; i++) booleans[i] = ParseBoolean(parts[i], ScpiQuery);
            return booleans;
        }

        public Byte[] QueryBinaryBlockOfByte(String ScpiQuery) {
            ThrowIfDisposed();
            return QueryBinary(ScpiQuery, () => _iMessageBasedSession.FormattedIO.ReadBinaryBlockOfByte());
        }

        public Byte[] QueryRawIO(String ScpiQuery) {
            ThrowIfDisposed();
            return QueryBinary(ScpiQuery, () => _iMessageBasedSession.RawIO.Read());
        }

        private Byte[] QueryBinary(String ScpiQuery, Func<Byte[]> ReadFunction) {
            ThrowIfDisposed();
            lock (_lock) {
                Command(ScpiQuery);
                _terminationCharacterEnabled = _iMessageBasedSession.TerminationCharacterEnabled;
                Byte[] response;
                try {
                    _iMessageBasedSession.TerminationCharacterEnabled = false;
                    response = ReadFunction();
                } finally {
                    _iMessageBasedSession.TerminationCharacterEnabled = _terminationCharacterEnabled;
                }
                return response;
            }
        }

        public void Clear() {
            ThrowIfDisposed();
            lock (_lock) { _iMessageBasedSession.Clear(); }
        }

        ~InstrumentDriver() { Dispose(false); }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(Boolean disposing) {
            lock (_lock) {
                if (_disposed) return;
                if (disposing) _iMessageBasedSession?.Dispose();
                _disposed = true;
            }
        }

        public void ThrowIfDisposed() { if (_disposed) throw new ObjectDisposedException(GetType().Name); }

        public void ResetCommand() {
            ThrowIfDisposed();
            Command("*RST");
        }
    }
}
