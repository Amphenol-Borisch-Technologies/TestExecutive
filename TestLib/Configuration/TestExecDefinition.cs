using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ABT.Test.TestExecutive.TestLib.Configuration {
    // TODO: Eventually; Move TestDev specific classes to TestDev project.
    // Classes Apps, ABT, Keysight & Microsoft.
    // In TestExecDefinition.xml, move <Apps> element/sub-elements to a TestDeveDefinition.xml file.
    // In TestExecDefinition.xsd, move <Apps> element/sub-elements to a TestDevDefinition.xsd file.
    // In TestExec.cs, address OpenApp(testExecDefinition.Apps.Microsoft.SQLServerManagementStudio);
    [XmlRoot(nameof(TestExecDefinition))]
    public class TestExecDefinition {
        [XmlElement(nameof(Development))] public Development Development { get; set; }
        [XmlElement(nameof(TestExecutiveURL))] public String TestExecutiveURL { get; set; }
        [XmlElement(nameof(TestPlansFolder))] public String TestPlansFolder { get; set; }
        [XmlElement(nameof(WindowsEventLog))] public WindowsEventLog WindowsEventLog { get; set; }
        [XmlElement(nameof(ActiveDirectoryPermissions))] public ActiveDirectoryPermissions ActiveDirectoryPermissions { get; set; }
        [XmlElement(nameof(TestData))] public TestData TestData { get; set; }
        [XmlElement(nameof(BarcodeReader))] public BarcodeReader BarcodeReader { get; set; }
        [XmlElement(nameof(Apps))] public Apps Apps { get; set; }
        [XmlElement(nameof(InstrumentsTestExec))] public InstrumentsTestExec InstrumentsTestExec { get; set; }
        public TestExecDefinition() { }
    }

    public class WindowsEventLog {
        [XmlAttribute(nameof(Source))] public String Source { get; set; }
        [XmlAttribute(nameof(Log))] public String Log { get; set; }
        public WindowsEventLog() { }
    }

    public class ActiveDirectoryPermissions {
        [XmlAttribute(nameof(ReadAndExecute))] public String ReadAndExecute { get; set; }
        [XmlAttribute(nameof(ModifyWrite))] public String ModifyWrite { get; set; }
        [XmlAttribute(nameof(FullControl))] public String FullControl { get; set; }
        public ActiveDirectoryPermissions() { }
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
        public ABT() { TestChooser = TestLib.TestExecutiveFolder + @"\" + TestChooser; }
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

    public class InstrumentTestExec : IInstrumentDefinition {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Address))] public String Address { get; set; }
        [XmlAttribute(nameof(NameSpacedClassName))] public String NameSpacedClassName { get; set; }
        public String FormatException(Exception exception) {
            StringBuilder stringBuilder = new StringBuilder().AppendLine();
            const Int32 PR = 23;
            stringBuilder.AppendLine($"Issue with {nameof(InstrumentTestExec)}:");
            stringBuilder.AppendLine($"   {nameof(ID)}".PadRight(PR) + $": {ID}");
            stringBuilder.AppendLine($"   {nameof(Detail)}".PadRight(PR) + $": {Detail}");
            stringBuilder.AppendLine($"   {nameof(Address)}".PadRight(PR) + $": {Address}");
            stringBuilder.AppendLine($"   {nameof(NameSpacedClassName)}".PadRight(PR) + $": {NameSpacedClassName}{Environment.NewLine}");
            stringBuilder.AppendLine($"{nameof(Exception)} {nameof(Exception.Message)}(s):");
            stringBuilder.AppendLine($"{exception}{Environment.NewLine}");
            return stringBuilder.ToString();
        }
        public InstrumentTestExec() { }
    }
}
