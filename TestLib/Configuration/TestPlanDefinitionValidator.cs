using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace ABT.Test.TestLib.Configuration {
    public static class TestPlanDefinitionValidator {
        private static Boolean validSpecification = true;
        private static readonly StringBuilder stringBuilder = new StringBuilder();
        private static XmlReader xmlReader;
        private const Int32 PR = 14;

        public static Boolean ValidSpecification(String TestPlanDefinitionXSD, String TestPlanDefinitionXML) {
            if (!File.Exists(TestPlanDefinitionXSD)) throw new ArgumentException($"XSD Test Specification File '{TestPlanDefinitionXSD}' does not exist.");
            if (!File.Exists(TestPlanDefinitionXML)) throw new ArgumentException($"XML Test Specification File '{TestPlanDefinitionXML}' does not exist.");
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(null, TestPlanDefinitionXSD);
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = xmlSchemaSet };
            xmlReaderSettings.ValidationEventHandler += ValidationCallback;

            try {
                using (xmlReader = XmlReader.Create(TestPlanDefinitionXML, xmlReaderSettings)) {
                    Double low, high;
                    String className = String.Empty;
                    HashSet<String> methodTypes = Data.GetDerivedClassnames<Method>();
                    while (xmlReader.Read()) {
                        if (xmlReader.NodeType == XmlNodeType.Element) {
                            if (String.Equals(xmlReader.Name, nameof(MethodInterval))) {
                                // NOTE: This if block required because Microsoft's Visual Studio only supports XML Schema 1.0.
                                // - If Visual Studio supported XSD 1.1, then <xs:assert test="@Low le @High"/> would obviate this block.
                                #region TLDR 
                                // Below compares just some of the many mainstream XML editing options.
                                // NOTE: XML Liquid Studio Community Edition supports XML Schema 1.1.
                                // - Liquid Studio is a powerful but complex external XML editor.
                                // - It's co$t free and licensing permits commericial usage.
                                // - Confirmed it detects Low > High occurences via <xs:assert test="@Low le @High"/>.
                                // - Chose to not utilize Liquid Studio because it adds too much complexity at this time.
                                //   - Non-community/non-co$t free editions are integrated into Visual Studio.
                                //
                                // NOTE: XML Notepad supports XML Schema 1.0.
                                // - XML Notepad is a powerful but simple external XML editor.
                                // - It's co$t free and licensing permits commericial usage.
                                //
                                // NOTE: Visual Studio Code with Red Hat's XML extension supports XML Schema 1.0.
                                // - VS Code is a powerful but complex external multi-purpose editor.
                                // - It's co$t free and licensing permits commericial usage.
                                //   - Red Hat's XML extension provides XML Schema 1.0 support.
                                //   - Tried several other provider's XML extensions, but none supported XML schema 1.1.
                                //   - XML editing integrated with Visual Studio Code is incredibly convenient.
                                //   - As a multi-purpose editor, can develop C# .Net applications.  Plus many other languages.
                                //
                                // NOTE: Visual Studio's supports XML Schema 1.0.
                                // - Visual Studio's integrated XML editor is powerful but complex.
                                //   - Visual Studio isn't co$t free, but licensing permits commercial use.
                                //   - XML editing integrated with Visual Studio is incredibly convenient.
                                //   - As a multi-purpose editor, can develop C# .Net applications.  Plus many other languages.
                                #endregion TLDR
                                low = Double.Parse(xmlReader.GetAttribute(nameof(MethodInterval.Low)));
                                high = Double.Parse(xmlReader.GetAttribute(nameof(MethodInterval.High)));
                                if (low > high) {
                                    validSpecification = false;
                                    stringBuilder.AppendLine($"{nameof(MethodInterval)}'s {nameof(MethodInterval.Low)} > {nameof(MethodInterval.High)}:");
                                    stringBuilder.AppendLine($"\t{nameof(IXmlLineInfo.LineNumber)}".PadRight(PR) + $": {(xmlReader as IXmlLineInfo).LineNumber}");
                                    stringBuilder.AppendLine($"\t{nameof(IXmlLineInfo.LinePosition)}".PadRight(PR) + $": {(xmlReader as IXmlLineInfo).LinePosition}");
                                    stringBuilder.AppendLine($"\t{nameof(xmlReader.NodeType)}".PadRight(PR) + $": {xmlReader.NodeType}");
                                    stringBuilder.AppendLine($"\t\t{nameof(MethodInterval.Description)}".PadRight(PR) + $": {xmlReader.GetAttribute(nameof(MethodInterval.Description))}");
                                    stringBuilder.AppendLine($"\t\t{nameof(MethodInterval.Name)}".PadRight(PR) + $": {xmlReader.GetAttribute(nameof(MethodInterval.Name))}");
                                    stringBuilder.AppendLine($"\t\t{nameof(MethodInterval.Low)}".PadRight(PR) + $":{xmlReader.GetAttribute(nameof(MethodInterval.Low))}");
                                    stringBuilder.AppendLine($"\t\t{nameof(MethodInterval.High)}".PadRight(PR) + $":{xmlReader.GetAttribute(nameof(MethodInterval.High))}{Environment.NewLine}{Environment.NewLine}");
                                }
                            }

                            if (String.Equals(xmlReader.Name, nameof(TestGroup))) {
                                // NOTE: This if block required because Microsoft's Visual Studio only supports XML Schema 1.0.
                                // - If Visual Studio supported XSD 1.1, then below xs:assert would obviate this block:
                                // <xs:assert test="not(Classname = MethodInterval/@Name or Classname = MethodProcess/@Name or Classname = MethodTextual/@Name or Classname = MethodCustom/@Name)"/>.
                                className = xmlReader.GetAttribute(nameof(TestGroup.Classname));
                            }

                            if (methodTypes.Contains(xmlReader.Name)) {
                                // NOTE: This if block required because Microsoft's Visual Studio only supports XML Schema 1.0.
                                // - If Visual Studio supported XSD 1.1, then below xs:assert would obviate this block:
                                // <xs:assert test="not(Classname = MethodInterval/@Name or Classname = MethodProcess/@Name or Classname = MethodTextual/@Name or Classname = MethodCustom/@Name)"/>.
                                String methodName = xmlReader.GetAttribute(nameof(Method.Name));
                                if (className == methodName) {
                                    validSpecification = false;
                                    stringBuilder.AppendLine($"{nameof(Method)}'s {nameof(Method.Name)} '{methodName}' identical to {nameof(TestGroup)}'s {nameof(TestGroup.Classname)} '{className}':");
                                    stringBuilder.AppendLine($"\t{nameof(IXmlLineInfo.LineNumber)}".PadRight(PR) + $": {(xmlReader as IXmlLineInfo).LineNumber}");
                                    stringBuilder.AppendLine($"\t{nameof(IXmlLineInfo.LinePosition)}".PadRight(PR) + $": {(xmlReader as IXmlLineInfo).LinePosition}");
                                    stringBuilder.AppendLine($"\t{nameof(xmlReader.NodeType)}".PadRight(PR) + $": {xmlReader.NodeType}");
                                    stringBuilder.AppendLine($"\t\t{nameof(Method.Description)}".PadRight(PR) + $": {xmlReader.GetAttribute(nameof(Method.Description))}");
                                    stringBuilder.AppendLine($"\t\t{nameof(Method.Name)}".PadRight(PR) + $": {xmlReader.GetAttribute(nameof(Method.Name))}");
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                validSpecification = false;
                stringBuilder.AppendLine($"{nameof(Exception)}:");
                stringBuilder.AppendLine($"\t{e.Message}".PadRight(PR) + Environment.NewLine);
            }

            if (!validSpecification) {
                stringBuilder.AppendLine($"Invalid XML Test Specification File: file:///{TestPlanDefinitionXML}.{Environment.NewLine}");
                Miscellaneous.CustomMessageBox.Show(Title: "Invalid XML Test Specification File", Message: stringBuilder.ToString(), OptionalIcon: System.Drawing.SystemIcons.Error);
            }
            return validSpecification;
        }

        private static void ValidationCallback(Object sender, ValidationEventArgs vea) {
            validSpecification = false;
            stringBuilder.AppendLine($"Validation Event:");
            stringBuilder.AppendLine($"\t{nameof(vea.Exception.LineNumber)}".PadRight(PR) + $": {vea.Exception.LineNumber}");
            stringBuilder.AppendLine($"\t{nameof(vea.Exception.LinePosition)}".PadRight(PR) + $": {vea.Exception.LinePosition}");
            stringBuilder.AppendLine($"\t{nameof(xmlReader.NodeType)}".PadRight(PR) + $": {xmlReader.NodeType}");
            stringBuilder.AppendLine($"\t{nameof(Method.Description)}".PadRight(PR) + $": {xmlReader.GetAttribute(nameof(Method.Description))}");
            stringBuilder.AppendLine($"\t{nameof(vea.Severity)}".PadRight(PR) + $": {vea.Severity}");
            stringBuilder.AppendLine($"\tAttribute".PadRight(PR) + $": {xmlReader.Name} = {xmlReader.Value}");
            stringBuilder.AppendLine($"\t{nameof(vea.Message)}".PadRight(PR) + $": {vea.Message}{Environment.NewLine}{Environment.NewLine}");
        }
    }
}
