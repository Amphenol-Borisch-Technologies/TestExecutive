using System;
using System.Xml;
using System.Xml.Serialization;

namespace ABT.Test.TestExecutive.TestLib.Configuration {

    [XmlRoot(ElementName = nameof(TestSequence), Namespace = "", IsNullable = false)]
    public class TestSequence {
        [XmlAttribute("noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public String NoNamespaceSchemaLocation { get; set; } = "file:///" + TestLib.TestPlanDefinitionXSD_Path.Replace(@"\", "/") + " " + TestLib.TestSequenceXSD_URL;
        public UUT UUT { get; set; } = Serializing.DeserializeFromFile<UUT>(xmlFile: TestLib.TestPlanDefinitionXML_Path);
        public TestOperation TestOperation { get; set; }
        [XmlIgnore] public Boolean IsOperation { get; set; } = false;
        public String Computer { get; set; } = Environment.MachineName;
        public String SerialNumber { get; set; } = String.Empty;
        public String Operator { get; set; } = TestLib.UserName;
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public String TimeTotal { get; set; } // NOTE:  XmlSerializer doesn't natively support TimeSpan, so have to serialize TimeTotal as a string.
        public EVENTS Event { get; set; } = EVENTS.UNSET;

        public TestSequence() { }

        public void PreRun() {
            TimeStart = DateTime.Now;
            Event = EVENTS.UNSET;
            foreach (TestGroup testGroup in TestOperation.TestGroups)
                foreach (Method method in testGroup.Methods) {
                    method.Event = EVENTS.UNSET;
                    _ = method.Log.Clear();
                    method.LogString = String.Empty;
                    method.Value = null;
                }
        }

        public void PostRun(EVENTS OperationEvent) {
            Event = OperationEvent;
            TimeEnd = DateTime.Now;
            TimeTotal = (TimeEnd - TimeStart).ToString(@"dd\.hh\:mm\:ss");
        }
    }
}