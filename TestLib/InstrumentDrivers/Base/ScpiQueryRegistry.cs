using System;
using System.Collections.Generic;
using System.Linq;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public sealed class ScpiQueryRegistry<TEnum>
        where TEnum : Enum {
        private readonly ScpiInstrument _instrument;
        private readonly Dictionary<TEnum, Func<Object>> _handlers;

        public ScpiQueryRegistry(ScpiInstrument instrument) {
            _instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            _handlers = new Dictionary<TEnum, Func<Object>>();
        }

        // ------------------------------------------------------------
        // PUBLIC API: MAP QUERY TO HANDLER
        // ------------------------------------------------------------

        public ScpiQueryRegistry<TEnum> Map<T>(TEnum query, Func<T> handler) {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            // Wrap handler<T> into Func<object>
            _handlers[query] = () => handler();
            return this;
        }

        // ------------------------------------------------------------
        // INVOCATION
        // ------------------------------------------------------------

        public T Invoke<T>(TEnum query) {
            if (!_handlers.TryGetValue(query, out var handler)) throw new NotImplementedException($"No SCPI query handler registered for '{query}'. Instrument: {_instrument.Address} ({_instrument.Detail})");

            Object value = handler();

            try {
                return (T)value;
            } catch (InvalidCastException) {
                throw new InvalidCastException($"SCPI query '{query}' returned a value of type '{value?.GetType().Name}', which cannot be cast to '{typeof(T).Name}'.");
            }
        }

        // ------------------------------------------------------------
        // VALIDATION AGAINST ENUM
        // ------------------------------------------------------------

        public ScpiQueryRegistry<TEnum> ValidateAll() {
            var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            var missing = enumValues.Where(e => !_handlers.ContainsKey(e)).ToList();
            if (missing.Any()) throw new InvalidOperationException($"SCPI query handlers are missing for the following enum values: {String.Join(", ", missing)}");
            return this;
        }

        // ------------------------------------------------------------
        // OPTIONAL CHECKER
        // ------------------------------------------------------------

        public Boolean IsRegistered(TEnum query) {
            return _handlers.ContainsKey(query);
        }
    }
}