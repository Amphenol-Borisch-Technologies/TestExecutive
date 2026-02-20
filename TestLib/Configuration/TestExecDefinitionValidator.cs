using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace ABT.Test.TestExecutive.TestLib.Configuration {
    public static class TestExecDefinitionValidator {
        private static Boolean validDefinition = true;
        private static readonly StringBuilder stringBuilder = new StringBuilder();
        private static XmlReader xmlReader;
        private const Int32 PR = 14;

        public static Boolean ValidDefinition(String testExecDefinitionXML_Path) {
            if (!File.Exists(testExecDefinitionXML_Path)) throw new ArgumentException($"XML TestExec Definition File '{testExecDefinitionXML_Path}' does not exist.");
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(null, TestLib.TestExecDefinitionXSD_Path);
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings {
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings,
                Schemas = xmlSchemaSet
            };
            xmlReaderSettings.ValidationEventHandler += ValidationCallback;

            try {
                using (xmlReader = XmlReader.Create(testExecDefinitionXML_Path, xmlReaderSettings)) { while (xmlReader.Read()) { } }
            } catch (Exception exception) {
                validDefinition = false;
                stringBuilder.AppendLine($"{nameof(Exception)}:");
                stringBuilder.AppendLine($"\t{exception.Message}".PadRight(PR) + Environment.NewLine);
            }

            if (!validDefinition) {
                stringBuilder.AppendLine($"Invalid XML TestExec Definition File: file:///{testExecDefinitionXML_Path}.{Environment.NewLine}");
                Miscellaneous.CustomMessageBox.Show(Title: "Invalid XML TestExec Definition File", Message: stringBuilder.ToString(), OptionalIcon: System.Drawing.SystemIcons.Error);
            }
            return validDefinition;
        }

        private static void ValidationCallback(Object sender, ValidationEventArgs vea) {
            validDefinition = false;
            stringBuilder.AppendLine($"Validation Event:");
            stringBuilder.AppendLine($"\t{nameof(vea.Exception.LineNumber)}".PadRight(PR) + $": {vea.Exception.LineNumber}");
            stringBuilder.AppendLine($"\t{nameof(vea.Exception.LinePosition)}".PadRight(PR) + $": {vea.Exception.LinePosition}");
            stringBuilder.AppendLine($"\t{nameof(xmlReader.NodeType)}".PadRight(PR) + $": {xmlReader.NodeType}");
            stringBuilder.AppendLine($"\t{nameof(vea.Severity)}".PadRight(PR) + $": {vea.Severity}");
            stringBuilder.AppendLine($"\tAttribute".PadRight(PR) + $": {xmlReader.Name} = {xmlReader.Value}");
            stringBuilder.AppendLine($"\t{nameof(vea.Message)}".PadRight(PR) + $": {vea.Message}{Environment.NewLine}{Environment.NewLine}");
        }
    }
}
