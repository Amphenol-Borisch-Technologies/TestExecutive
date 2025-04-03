using ABT.Test.TestLib.InstrumentDrivers.Interfaces;
using Agilent.CommandExpert.ScpiNet.AgSCPI99_1_0;
using System;
using System.Collections.Generic;

namespace ABT.Test.TestLib.InstrumentDrivers.Generic {
    public class SCPI_NET : AgSCPI99, IInstrument, IDiagnostics {
        public enum IDN_FIELDS { Manufacturer, Model, SerialNumber, FirmwareRevision } // Example: "Keysight Technologies,E36103B,MY61001983,1.0.2-1.02".  

        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 result;
            try {
                SCPI.TST.Query(out result);
            } catch (Exception e) {
                Instruments.SelfTestFailure(this, e);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)result;
        }

        public SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.UNKNOWN;
        }

        public String Identity(IDN_FIELDS Property) {
            SCPI.IDN.Query(out String Identity);
            return Identity.Split(',')[(Int32)Property];
        }

        public static String Identity(String Address, IDN_FIELDS Property) {
            new AgSCPI99(Address).SCPI.IDN.Query(out String Identity);
            return Identity.Split(',')[(Int32)Property];
        }

        public static String Identity(Object Instrument, IDN_FIELDS Property) {
            String Address = ((IInstrument)Instrument).Address;
            return Identity(Address, Property);
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            return (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
        }
    }
}