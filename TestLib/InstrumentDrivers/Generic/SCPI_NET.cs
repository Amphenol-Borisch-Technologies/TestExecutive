using ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces;
using Agilent.CommandExpert.ScpiNet.AgSCPI99_1_0;
using System;
using System.Collections.Generic;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Generic {
    public class SCPI_NET : IInstrument, IDiagnostics {
        public enum IDN_FIELDS { Manufacturer, Model, SerialNumber, FirmwareRevision } // Example: "Keysight Technologies,E36103B,MY61001983,1.0.2-1.02".  

        public String Address { get; }
        public String Detail { get; }
        public AgSCPI99 AgSCPI99 { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            AgSCPI99.SCPI.RST.Command();
            AgSCPI99.SCPI.CLS.Command();
        }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 result;
            try {
                AgSCPI99.SCPI.TST.Query(out result);
            } catch (Exception exception) {
                Instruments.SelfTestFailure(this, exception);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)result;
        }

        public SCPI_NET(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            AgSCPI99 = new AgSCPI99(Address);
            InstrumentType = INSTRUMENT_TYPES.UNKNOWN;
        }

        public String Identity(IDN_FIELDS Property) {
            AgSCPI99.SCPI.IDN.Query(out String Identity);
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