using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.Configuration {
    public static class TestPlanGenerator {

        public static void GenerateTestPlan(String testPlanDefinitionXML_Path) {
            TestSpace testSpace = Serializing.DeserializeFromFile<TestSpace>(testPlanDefinitionXML_Path);

            for (Int32 testOperation = 0; testOperation < testSpace.TestOperations.Count; testOperation++) {
                CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
                CodeNamespace codeNamespace = GetNamespace(testSpace, testOperation);
                _ = codeCompileUnit.Namespaces.Add(codeNamespace);
                for (Int32 testGroup = 0; testGroup < testSpace.TestOperations[testOperation].TestGroups.Count; testGroup++) {
                    CodeTypeDeclaration codeTypeDeclaration = AddClass(codeNamespace, testSpace.TestOperations[testOperation].TestGroups[testGroup]);
                    for (Int32 method = 0; method < testSpace.TestOperations[testOperation].TestGroups[testGroup].Methods.Count; method++) {
                        AddMethod(codeTypeDeclaration, testSpace.TestOperations[testOperation], testGroup, method);
                    }
                }
                CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
                CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions {
                    BlankLinesBetweenMembers = true,
                    BracingStyle = "Block",
                    IndentString = "    "
                };

                SaveFileDialog saveFileDialog = new SaveFileDialog {
                    Filter = "C# files (*.cs)|*.cs",
                    Title = "Save the generated C# TestPlan file",
                    DefaultExt = "cs",
                    FileName = $"{testSpace.TestOperations[testOperation].NamespaceTrunk}.cs",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                    using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName)) { cSharpCodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter, codeGeneratorOptions); }
                }
            }
        }

        private static CodeNamespace GetNamespace(TestSpace testSpace, Int32 testOperation) {
            CodeNamespace codeNamespace = new CodeNamespace(testSpace.NamespaceRoot + "." + testSpace.TestOperations[testOperation].NamespaceTrunk);
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(System)}"));
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(System)}.{nameof(System.Diagnostics)}"));
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(ABT)}.{nameof(Test)}.{TestLib.TestExecutive}.{nameof(TestLib)}"));
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(ABT)}.{nameof(Test)}.{TestLib.TestExecutive}.{nameof(TestLib)}.{nameof(Configuration)}"));
            codeNamespace.Imports.Add(new CodeNamespaceImport($"static {nameof(ABT)}.{nameof(Test)}.{TestLib.TestExecutive}.{nameof(TestLib)}.{nameof(TestLib)}"));
            return codeNamespace;
        }

        private static CodeTypeDeclaration AddClass(CodeNamespace codeNamespace, TestGroup testGroup) {
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(testGroup.Classname) {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.NotPublic | System.Reflection.TypeAttributes.Class,
            };
            _ = codeNamespace.Types.Add(codeTypeDeclaration);
            return codeTypeDeclaration;
        }

        private static void AddMethod(CodeTypeDeclaration codeTypeDeclaration, TestOperation testOperation, Int32 testGroup, Int32 method) {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod {
                Name = testOperation.TestGroups[testGroup].Methods[method].Name,
                Attributes = MemberAttributes.Static | MemberAttributes.Assembly,
                ReturnType = new CodeTypeReference(typeof(String))
            };
            if (testGroup == 0 && method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertion)testOperation).Assertion()}"));
            if (method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertion)testOperation.TestGroups[testGroup]).Assertion()}"));
            _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertion)testOperation.TestGroups[testGroup].Methods[method]).Assertion()}"));

            Method m = testOperation.TestGroups[testGroup].Methods[method];
            if (m is MethodCustom) {
                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{nameof(TestIndices)}.{nameof(TestIndices.Method)}.{nameof(TestIndices.Method.Event)} = {typeof(EVENTS).Name}.{EVENTS.UNSET};"));
                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\treturn String.Empty;"));
            } else if (m is MethodInterval) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\treturn Double.NaN.ToString();"));
            else if (m is MethodProcess) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\treturn \"{-1}\";"));
            else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\treturn String.Empty;"));
            _ = codeTypeDeclaration.Members.Add(codeMemberMethod);
        }

        public static void GenerateInstrumentAliases(String testPlanDefinitionXML_Path) {
            TestPlanDefinition testPlanDefinition = Serializing.DeserializeFromFile<TestPlanDefinition>(testPlanDefinitionXML_Path);
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace($"{testPlanDefinition.TestSpace.NamespaceRoot}.{nameof(InstrumentDrivers)}");
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(ABT)}.{nameof(Test)}.{TestLib.TestExecutive}.{nameof(TestLib)}"));
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(ABT)}.{nameof(Test)}.{TestLib.TestExecutive}.{nameof(TestLib)}.{nameof(InstrumentDrivers)}.{nameof(InstrumentDrivers.Base)}"));
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(ABT)}.{nameof(Test)}.{TestLib.TestExecutive}.{nameof(TestLib)}.{nameof(InstrumentDrivers)}.{nameof(InstrumentDrivers.Multifunction)}"));
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(ABT)}.{nameof(Test)}.{TestLib.TestExecutive}.{nameof(TestLib)}.{nameof(InstrumentDrivers)}.{nameof(InstrumentDrivers.Oscilloscopes)}"));
            codeNamespace.Imports.Add(new CodeNamespaceImport($"{nameof(ABT)}.{nameof(Test)}.{TestLib.TestExecutive}.{nameof(TestLib)}.{nameof(InstrumentDrivers)}.{nameof(InstrumentDrivers.PowerSupplies)}"));
            _ = codeCompileUnit.Namespaces.Add(codeNamespace);

            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration("IA") {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.NotPublic,
                Attributes = MemberAttributes.Static
            };
            codeNamespace.Types.Add(codeTypeDeclaration);
            try {
                foreach (InstrumentInfo instrumentInfo in testPlanDefinition.InstrumentsTestPlan.GetInfo()) codeTypeDeclaration.Members.Add(CreateField(instrumentInfo));
            } catch (ArgumentException argumentException) {
                // Handle the case where an instrument alias is duplicated in the TestPlanDefinition.
                // This can happen if the same instrument is defined more than once in the XML file.
                MessageBox.Show($"{argumentException.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
            CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions {
                BlankLinesBetweenMembers = false,
                BracingStyle = "Block",
                IndentString = "    "
            };

            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "C# files (*.cs)|*.cs",
                Title = "Save the generated Instrument Aliases C# file",
                DefaultExt = "cs",
                FileName = "InstrumentAliases.cs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName)) { cSharpCodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter, codeGeneratorOptions); }
            }
        }

        private static CodeMemberField CreateField(InstrumentInfo instrumentInfo) {
            Int32 i = instrumentInfo.NameSpacedClassName.LastIndexOf(".");
            String Classname = instrumentInfo.NameSpacedClassName.Substring(i + 1);

            CodeMemberField field = new CodeMemberField {
                Attributes = MemberAttributes.Static | MemberAttributes.Assembly,
                Type = new CodeTypeReference(Classname),
                Name = instrumentInfo.Alias,
                InitExpression = new CodeCastExpression(Classname,
                    new CodeIndexerExpression(
                        new CodePropertyReferenceExpression(new CodeTypeReferenceExpression($"{nameof(TestLib)}"), nameof(InstrumentDrivers)),
                        new CodePrimitiveExpression(instrumentInfo.ID)))
            };

            return field;
        }
    }
}
