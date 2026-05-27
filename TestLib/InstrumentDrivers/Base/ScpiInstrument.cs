using System;
using System.Globalization;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public abstract class ScpiInstrument : InstrumentDriver {
        protected ScpiInstrument(String address, String detail, INSTRUMENT_TYPE type)
            : base(address, detail, type) {
        }

        // ------------------------------------------------------------
        // SCPI WRITE HELPERS
        // ------------------------------------------------------------

        protected void Write(String command) {
            Command(command);
        }

        protected void Write(String command, object arg) {
            if (arg == null)
                throw new ArgumentNullException(nameof(arg));

            Command($"{command} {FormatArg(arg)}");
        }

        // ------------------------------------------------------------
        // SCPI READ HELPERS
        // ------------------------------------------------------------

        protected T Read<T>(String query) {
            if (String.IsNullOrWhiteSpace(query)) throw new ArgumentException("Query cannot be null or whitespace.", nameof(query));

            return Query<T>($"{query}?");
        }

        // ------------------------------------------------------------
        // SCPI FORMATTING + PARSING
        // ------------------------------------------------------------

        protected String FormatArg(Object arg) {
            switch (arg) {
                case Enum e:    return FormatEnum(e);
                case Boolean b: return b ? "1" : "0";
                case Double d:  return d.ToString("G", CultureInfo.InvariantCulture);
                case Single f:  return f.ToString("G", CultureInfo.InvariantCulture);
                case Int32 i:   return i.ToString(CultureInfo.InvariantCulture);
                case String s:  return s;
                default:        return System.Convert.ToString(arg, CultureInfo.InvariantCulture);
            }
        }

        protected String FormatEnum(Enum e) {
            // SCPI generally uppercase identifiers
            return e.ToString().ToUpperInvariant();
        }

        protected String StripQuotes(String s) {
            if (String.IsNullOrWhiteSpace(s)) return s;
            s = s.Trim();
            if (s.StartsWith("\"") && s.EndsWith("\"") && s.Length >= 2) return s.Substring(1, s.Length - 2);
            return s;
        }

        // ------------------------------------------------------------
        // BINARY TRANSFERS (pass‑through to InstrumentDriver)
        // ------------------------------------------------------------

        protected Byte[] ReadBinary(String query) {
            return QueryBinaryBlockOfByte(query);
        }
    }
}