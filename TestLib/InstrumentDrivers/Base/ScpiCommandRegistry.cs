using System;
using System.Collections.Generic;
using System.Linq;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public sealed class ScpiCommandRegistry<TEnum>
        where TEnum : Enum {
        private readonly ScpiInstrument _instrument;
        private readonly Dictionary<TEnum, Action<String>> _handlers;

        public ScpiCommandRegistry(ScpiInstrument instrument) {
            _instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            _handlers = new Dictionary<TEnum, Action<String>>();
        }

        // ------------------------------------------------------------
        // PUBLIC API: MAP COMMAND TO HANDLER
        // ------------------------------------------------------------

        public ScpiCommandRegistry<TEnum> Map(TEnum command, Action handler) {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers[command] = _ => handler();
            return this;
        }

        public ScpiCommandRegistry<TEnum> Map(TEnum command, Action<String> handler) {
            _handlers[command] = handler ?? throw new ArgumentNullException(nameof(handler));
            return this;
        }

        // ------------------------------------------------------------
        // INVOCATION
        // ------------------------------------------------------------

        public void Invoke(TEnum command, String arg = "") {
            if (!_handlers.TryGetValue(command, out var handler)) throw new NotImplementedException($"No SCPI command handler registered for '{command}'. Instrument: {_instrument.Address} ({_instrument.Detail})");
            handler(arg ?? String.Empty);
        }

        // ------------------------------------------------------------
        // VALIDATION AGAINST ENUM
        // ------------------------------------------------------------

        public ScpiCommandRegistry<TEnum> ValidateAll() {
            var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            var missing = enumValues.Where(e => !_handlers.ContainsKey(e)).ToList();

            if (missing.Any()) throw new InvalidOperationException($"SCPI command handlers are missing for the following enum values: {String.Join(", ", missing)}");
            return this;
        }

        // ------------------------------------------------------------
        // OPTIONAL: CHECK IF A COMMAND IS REGISTERED
        // ------------------------------------------------------------

        public Boolean IsRegistered(TEnum command) {
            return _handlers.ContainsKey(command);
        }
    }
}