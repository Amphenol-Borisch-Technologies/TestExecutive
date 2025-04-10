using System;
using System.Collections.Generic;

namespace ABT.Test.TestExecutive.TestLib.InstrumentDrivers.Interfaces {
    public interface IDiagnostics { (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(List<Configuration.Parameter> Parameters); }

    public class DiagnosticsResult {
        public readonly String Label;
        public readonly String Message;
        public EVENTS Event;
        public DiagnosticsResult(String Label, String Message, EVENTS Event) { this.Label = Label; this.Message = Message; this.Event = Event; }
    }
}
