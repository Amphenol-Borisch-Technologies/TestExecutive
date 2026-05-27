using System;
using System.Collections.Generic;
using System.Linq;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Base {
    public static class EnumFlagsHelper {
        // Convert integer bitmask (from instrument) -> comma-separated mnemonic String
        public static String FlagsToMnemonics<TEnum>(Int32 flags) where TEnum : Enum {
            ValidateEnumIsFlags<TEnum>();
            Int32 allMask = GetAllMask<TEnum>();
            // Check for bits outside defined enum
            if ((flags & ~allMask) != 0) throw new ArgumentOutOfRangeException(nameof(flags), $"Value {flags} contains bits not defined in {typeof(TEnum).Name}.");

            var enumType = typeof(TEnum);
            var values = Enum.GetValues(enumType).Cast<TEnum>();
            // If NONE or ALL exactly match, return those literal names
            if (flags == 0 && Enum.IsDefined(enumType, 0)) return Enum.GetName(enumType, 0);
            if (flags == allMask && Enum.IsDefined(enumType, allMask)) return Enum.GetName(enumType, allMask);
            List<String> mnemonics = new List<String>();

            foreach (var val in values) {
                Int32 v = Convert.ToInt32(val);
                if (v == 0 || v == allMask) continue; // skip NONE or ALL
                if ((flags & v) == v) mnemonics.Add(val.ToString());
            }

            if (mnemonics.Count == 0) return Enum.GetName(enumType, 0) ?? "NONE";
            return String.Join(", ", mnemonics);
        }

        // Convert comma/space-separated mnemonics -> integer bitmask
        public static Int32 MnemonicsToFlags<TEnum>(String mnemonics) where TEnum : Enum {
            ValidateEnumIsFlags<TEnum>();
            if (String.IsNullOrWhiteSpace(mnemonics)) throw new ArgumentException("Value cannot be null, empty, or whitespace.", nameof(mnemonics));
            mnemonics = mnemonics.ToUpper().Replace(" ", ",");
            Int32 allMask = GetAllMask<TEnum>();

            // Single literal NONE or ALL
            if (String.Equals(mnemonics, "NONE", StringComparison.OrdinalIgnoreCase)) return 0;
            if (String.Equals(mnemonics, "ALL", StringComparison.OrdinalIgnoreCase)) return allMask;

            String[] parts = mnemonics.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Int32 result = 0;
            foreach (String s in parts) {
                if (!Enum.IsDefined(typeof(TEnum), s)) throw new ArgumentException($"Value '{s}' is not a valid {typeof(TEnum).Name} mnemonic.", nameof(mnemonics));
                TEnum val = (TEnum)Enum.Parse(typeof(TEnum), s);
                result |= Convert.ToInt32(val);
            }
            return result;
        }

        // ------------------------------------------------------------
        // INTERNAL HELPERS
        // ------------------------------------------------------------

        private static void ValidateEnumIsFlags<TEnum>() where TEnum : Enum {
            var t = typeof(TEnum);
            Boolean hasFlags = t.GetCustomAttributes(typeof(FlagsAttribute), inherit: false).Any();
            if (!hasFlags) throw new InvalidOperationException($"Enum type {t.Name} must have [Flags] attribute.");
        }

        private static Int32 GetAllMask<TEnum>() where TEnum : Enum {
            var enumType = typeof(TEnum);
            // ALL must be defined or the maximum bitmask is computed
            var values = Enum.GetValues(enumType).Cast<TEnum>();
            var allMember = values.FirstOrDefault(v => v.ToString().Equals("ALL", StringComparison.OrdinalIgnoreCase));
            if (!Equals(allMember, default(TEnum))) return Convert.ToInt32(allMember);

            // Else OR all values together
            Int32 mask = 0;
            foreach (var v in values) mask |= Convert.ToInt32(v);
            return mask;
        }
    }
}