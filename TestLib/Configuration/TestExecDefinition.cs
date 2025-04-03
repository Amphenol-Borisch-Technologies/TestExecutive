using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ABT.Test.TestLib.Configuration {
    [XmlRoot(nameof(TestExecDefinition))]
    public class TestExecDefinition {
        [XmlElement(nameof(Development))] public Development Development { get; set; }
        [XmlElement(nameof(TestData))] public TestData TestData { get; set; }
        [XmlElement(nameof(BarcodeReader))] public BarcodeReader BarcodeReader { get; set; }
        [XmlElement(nameof(Apps))] public Apps Apps { get; set; }
        [XmlElement(nameof(InstrumentsTestExec))] public InstrumentsTestExec InstrumentsTestExec { get; set; }
        public TestExecDefinition() { }
    }

    public class TestData {
        [XmlElement(nameof(SQL_DB), typeof(SQL_DB))]
        [XmlElement(nameof(TextFiles), typeof(TextFiles))]
        public Object Item { get; set; }
        public TestData() { }
    }

    public class SQL_DB {
        [XmlAttribute(nameof(ConnectionString))] public String ConnectionString { get; set; }
        public SQL_DB() { }
    }

    public class TextFiles {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        public TextFiles() { }
    }

    public class BarcodeReader {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        public BarcodeReader() { }
    }

    public class Apps {
        [XmlElement(nameof(ABT))] public ABT ABT { get; set; }
        [XmlElement(nameof(Keysight))] public Keysight Keysight { get; set; }
        [XmlElement(nameof(Microsoft))] public Microsoft Microsoft { get; set; }
        public Apps() { }
    }

    public class ABT {
        [XmlElement(nameof(TestChooser))] public String TestChooser { get; set; }
        public ABT() { }
    }

    public class Keysight {
        [XmlElement(nameof(CommandExpert))] public String CommandExpert { get; set; }
        [XmlElement(nameof(ConnectionExpert))] public String ConnectionExpert { get; set; }
        public Keysight() { }
    }

    public class Microsoft {
        [XmlElement(nameof(SQLServerManagementStudio))] public String SQLServerManagementStudio { get; set; }
        [XmlElement(nameof(VisualStudio))] public String VisualStudio { get; set; }
        [XmlElement(nameof(VisualStudioCode))] public String VisualStudioCode { get; set; }
        [XmlElement(nameof(XMLNotepad))] public String XMLNotepad { get; set; }
        public Microsoft() { }
    }

    public class InstrumentsTestExec {
        [XmlElement(nameof(InstrumentTestExec))] public List<InstrumentTestExec> InstrumentTestExec { get; set; }
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        public InstrumentsTestExec() { }
    }

    public class InstrumentTestExec {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Address))] public String Address { get; set; }
        [XmlAttribute(nameof(NameSpacedClassName))] public String NameSpacedClassName { get; set; }
        public InstrumentTestExec() { }
    }
}
