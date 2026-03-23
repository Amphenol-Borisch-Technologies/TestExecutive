using System;
using System.Runtime.Serialization;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    [Serializable]
    public class InstrumentException : Exception {
        public String Address { get; }
        public String Detail { get; }
        public String ScpiCommand { get; }

        public InstrumentException() { }

        public InstrumentException(String message) : base(message) { }

        public InstrumentException(String message, Exception inner) : base(message, inner) { }

        public InstrumentException(String message, String address, String detail, String scpiCommand) : base(message) {
            Address = address;
            Detail = detail;
            ScpiCommand = scpiCommand;
        }

        public InstrumentException(String message, String address, String detail, String scpiCommand, Exception inner) : base(message, inner) {
            Address = address;
            Detail = detail;
            ScpiCommand = scpiCommand;
        }

        // Serialization constructor
        protected InstrumentException(SerializationInfo info, StreamingContext context) : base(info, context) {
            Address = info.GetString(nameof(Address));
            Detail = info.GetString(nameof(Detail));
            ScpiCommand = info.GetString(nameof(ScpiCommand));
        }

        // Serialization support
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Address), Address);
            info.AddValue(nameof(Detail), Detail);
            info.AddValue(nameof(ScpiCommand), ScpiCommand);
        }

        public override String ToString() {
            return $"{base.ToString()}" +
                   $"{Environment.NewLine}Address: {Address}" +
                   $"{Environment.NewLine}Detail: {Detail}" +
                   $"{Environment.NewLine}SCPI: {ScpiCommand}";
        }
    }
}