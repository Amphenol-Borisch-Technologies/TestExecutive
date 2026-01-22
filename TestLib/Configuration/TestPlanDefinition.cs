using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static ABT.Test.TestExecutive.TestLib.TestLib;

namespace ABT.Test.TestExecutive.TestLib.Configuration {
    [XmlRoot(nameof(TestPlanDefinition))]
    public class TestPlanDefinition {
        [XmlElement(nameof(UUT))] public UUT UUT { get; set; }
        [XmlElement(nameof(SerialNumberEntry))] public SerialNumberEntry SerialNumberEntry { get; set; }
        [XmlElement(nameof(Development))] public Development Development { get; set; }
        [XmlArray(nameof(Modifications))] public List<Modification> Modifications { get; set; }
        [XmlElement(nameof(InstrumentsTestPlan))] public InstrumentsTestPlan InstrumentsTestPlan { get; set; }
        [XmlElement(nameof(TestSpace))] public TestSpace TestSpace { get; set; }

        public TestPlanDefinition() { }
    }

    public class UUT {
        [XmlElement(nameof(Customer))] public Customer Customer { get; set; }
        [XmlElement(nameof(TestSpecification))] public List<TestSpecification> TestSpecification { get; set; }
        [XmlElement(nameof(Documentation))] public List<Documentation> Documentation { get; set; }
        [XmlAttribute(nameof(Number))] public String Number { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Category))] public Category Category { get; set; }
        internal static String DIVIDER = "|";

        public UUT() { }

        public static String EF(Object o) {
            String s = o.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'");
            return $"\"{s}\"";
        }
    }

    public class Customer {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Division))] public String Division { get; set; }
        [XmlAttribute(nameof(Location))] public String Location { get; set; }

        public Customer() { }
    }

    public class TestSpecification {
        [XmlAttribute(nameof(Document))] public String Document { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Title))] public String Title { get; set; }
        [XmlAttribute(nameof(Date))] public String Date { get; set; }

        public TestSpecification() { }
    }

    public class Documentation {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }

        public Documentation() { }
    }

    public enum Category { Component, CircuitCard, Harness, Unit, System }

    public class SerialNumberEntry {
        [XmlAttribute(nameof(EntryType))] public SerialNumberEntryType EntryType { get; set; }
        [XmlAttribute(nameof(RegularEx))] public String RegularEx { get; set; } = "^.+$";
        [XmlAttribute(nameof(Format))] public String Format { get; set; } = "1 or more characters.";
        public Boolean IsEnabled() { return EntryType != SerialNumberEntryType.None; }
        public SerialNumberEntry() { }
    }

    public enum SerialNumberEntryType { Barcode, Keyboard, None }

    public class Development {
        [XmlElement(nameof(Developer))] public List<Developer> Developer { get; set; }
        [XmlElement(nameof(Documentation))] public List<Documentation> Documentation { get; set; }
        [XmlElement(nameof(Repository))] public List<Repository> Repository { get; set; }
        [XmlAttribute(nameof(Released))] public String Released { get; set; }

        public Development() { }
    }

    public class Developer {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Language))] public Languages Language { get; set; }
        [XmlAttribute(nameof(Comment))] public String Comment { get; set; }

        public enum Languages { CSharp, Python, VEE }

        public Developer() { }
    }

    public class Repository {
        [XmlAttribute(nameof(URL))] public String URL { get; set; }

        public Repository() { }
    }

    public class Modification {
        [XmlAttribute(nameof(Who))] public String Who { get; set; }
        [XmlAttribute(nameof(What))] public String What { get; set; }
        [XmlAttribute(nameof(When))] public String When { get; set; }
        [XmlAttribute(nameof(Where))] public String Where { get; set; }
        [XmlAttribute(nameof(Why))] public String Why { get; set; }

        public Modification() { }
    }

    public class InstrumentsTestPlan {
        [XmlElement(nameof(Stationary))] public List<Stationary> Stationary { get; set; }
        [XmlElement(nameof(Mobile))] public List<Mobile> Mobile { get; set; }

        public InstrumentsTestPlan() { }

        public List<InstrumentInfo> GetInfo() {
            List<InstrumentInfo> instruments = new List<InstrumentInfo>();

            InstrumentsTestExec instrumentsTestExec = Serializing.DeserializeFromFile<InstrumentsTestExec>(TestExecDefinitionXML_Path);
            InstrumentTestExec instrumentTestExec = null;
            foreach (Stationary stationary in Stationary) {
                instrumentTestExec = instrumentsTestExec.InstrumentTestExec.Find(x => x.ID == stationary.ID) ?? throw new ArgumentException(FormatException(stationary.ID));
                instruments.Add(new InstrumentInfo(stationary.ID, stationary.Alias, instrumentTestExec.NameSpacedClassName));
            }

            foreach (Mobile mobile in Mobile) instruments.Add(new InstrumentInfo(mobile.ID, mobile.Alias, mobile.NameSpacedClassName));
            return instruments;
        }

        private String FormatException(String stationaryID) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Instrument with ID '{stationaryID}' not present in file{Environment.NewLine}");
            stringBuilder.AppendLine($"'{TestExecDefinitionXML_Path}'.");
            return stringBuilder.ToString();
        }
    }

    public class InstrumentInfo {
        public String ID;
        public String Alias;
        public String NameSpacedClassName;

        public InstrumentInfo() { }

        public InstrumentInfo(String id, String alias, String nameSpaceClassName) {
            ID = id;
            Alias = alias;
            NameSpacedClassName = nameSpaceClassName;
        }
    }

    public class Stationary {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(Alias))] public String Alias { get; set; }

        public Stationary() { }
    }

    public interface IInstrumentDefinition {
        String ID { get; set; }
        String Detail { get; set; }
        String Address { get; set; }
        String NameSpacedClassName { get; set; }

        String FormatException(Exception e);
    }

    public class Mobile : Stationary, IInstrumentDefinition {
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Address))] public String Address { get; set; }
        [XmlAttribute(nameof(NameSpacedClassName))] public String NameSpacedClassName { get; set; }

        public String FormatException(Exception exception) {
            StringBuilder stringBuilder = new StringBuilder().AppendLine();
            const Int32 PR = 23;
            stringBuilder.AppendLine($"Issue with {nameof(Mobile)}:");
            stringBuilder.AppendLine($"   {nameof(ID)}".PadRight(PR) + $": {ID}");
            stringBuilder.AppendLine($"   {nameof(Alias)}".PadRight(PR) + $": {Alias}");
            stringBuilder.AppendLine($"   {nameof(Detail)}".PadRight(PR) + $": {Detail}");
            stringBuilder.AppendLine($"   {nameof(Address)}".PadRight(PR) + $": {Address}");
            stringBuilder.AppendLine($"   {nameof(NameSpacedClassName)}".PadRight(PR) + $": {NameSpacedClassName}{Environment.NewLine}");
            stringBuilder.AppendLine($"{nameof(Exception)} {nameof(Exception.Message)}(s):");
            stringBuilder.AppendLine($"{exception}{Environment.NewLine}");
            return stringBuilder.ToString();
        }

        public Mobile() { }
    }

    public class TestSpace {
        [XmlAttribute(nameof(NamespaceRoot))] public String NamespaceRoot { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Simulate))] public Boolean Simulate { get; set; }
        [XmlElement(nameof(Parameter))] public List<Parameter> Parameters { get; set; }
        [XmlElement(nameof(TestOperation))] public List<TestOperation> TestOperations { get; set; }

        public TestSpace() { }

        public Statistics Statistics { get; set; } = new Statistics();

        public String StatisticsDisplay() {
            const Int32 L = 6, PR = 17;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{nameof(Statistics.EmergencyStopped)}".PadRight(PR) + $": {Statistics.EmergencyStopped,L}, {Statistics.FractionEmergencyStopped(),L:P1}");
            stringBuilder.AppendLine($"{nameof(Statistics.Errored)}".PadRight(PR) + $": {Statistics.Errored,L}, {Statistics.FractionErrored(),L:P1}");
            stringBuilder.AppendLine($"{nameof(Statistics.Cancelled)}".PadRight(PR) + $": {Statistics.Cancelled,L}, {Statistics.FractionCancelled(),L:P1}");
            stringBuilder.AppendLine($"{nameof(Statistics.Unset)}".PadRight(PR) + $": {Statistics.Unset,L}, {Statistics.FractionUnset(),L:P1}");
            stringBuilder.AppendLine($"{nameof(Statistics.Failed)}".PadRight(PR) + $": {Statistics.Failed,L}, {Statistics.FractionFailed(),L:P1}");
            stringBuilder.AppendLine($"{nameof(Statistics.Passed)}".PadRight(PR) + $": {Statistics.Passed,L}, {Statistics.FractionPassed(),L:P1}");
            stringBuilder.AppendLine($"{nameof(Statistics.Informed)}".PadRight(PR) + $": {Statistics.Informed,L}, {Statistics.FractionInformed(),L:P1}");
            stringBuilder.AppendLine($"------");
            stringBuilder.AppendLine($"{nameof(Statistics.Tested)}".PadRight(PR) + $": {Statistics.Tested(),L}");
            return stringBuilder.ToString();
        }

        public String StatisticsStatus() { return $"   Failed: {Statistics.Failed}     Passed: {Statistics.Passed}   "; }

        public String StatusTime() { return $"   Time: {Statistics.Time()}"; }
    }

    public interface IAssertion { String Assertion(); }

    public class TestOperation : IAssertion {
        [XmlAttribute(nameof(NamespaceTrunk))] public String NamespaceTrunk { get; set; }
        [XmlAttribute(nameof(ProductionTest))] public Boolean ProductionTest { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlElement(nameof(TestGroup))] public List<TestGroup> TestGroups { get; set; }

        public TestOperation() { }

        public String Assertion() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"if ({nameof(TestLib)}.{nameof(TestLib.testSequence)}.{nameof(TestLib.testSequence.IsOperation)}) Debug.Assert({nameof(TestIndices)}.{nameof(TestIndices.TestOperation)}.Assert(");
            stringBuilder.Append($"{nameof(NamespaceTrunk)}: {UUT.EF(GetType().GetProperty(nameof(NamespaceTrunk)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(ProductionTest)}: {UUT.EF(GetType().GetProperty(nameof(ProductionTest)).GetValue(this).ToString().ToLower())}, ");
            stringBuilder.Append($"{nameof(Description)}: {UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(TestGroups)}: {UUT.EF(String.Join(UUT.DIVIDER, TestGroups.Select(tg => tg.Classname)))}));");
            return stringBuilder.ToString();
        }

        public Boolean Assert(String NamespaceTrunk, String ProductionTest, String Description, String TestGroups) {
            Boolean boolean = String.Equals(this.NamespaceTrunk, NamespaceTrunk);
            boolean &= this.ProductionTest == Boolean.Parse(ProductionTest);
            boolean &= String.Equals(this.Description, Description);
            boolean &= String.Equals(String.Join(UUT.DIVIDER, this.TestGroups.Select(tg => tg.Classname)).Replace("\"", ""), TestGroups);
            return boolean;
        }
    }

    public class TestGroup : IAssertion {
        [XmlAttribute(nameof(Classname))] public String Classname { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        [XmlAttribute(nameof(Independent))] public Boolean Independent { get; set; }
        [XmlElement(nameof(MethodCustom), typeof(MethodCustom))]
        [XmlElement(nameof(MethodInterval), typeof(MethodInterval))]
        [XmlElement(nameof(MethodProcess), typeof(MethodProcess))]
        [XmlElement(nameof(MethodTextual), typeof(MethodTextual))]
        public List<Method> Methods { get; set; }

        public TestGroup() { }

        public String Assertion() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Debug.Assert({nameof(TestIndices)}.{nameof(TestIndices.TestGroup)}.Assert(");
            stringBuilder.Append($"{nameof(Classname)}: {UUT.EF(GetType().GetProperty(nameof(Classname)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(Description)}: {UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(CancelNotPassed)}: {UUT.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}, ");
            stringBuilder.Append($"{nameof(Independent)}: {UUT.EF(GetType().GetProperty(nameof(Independent)).GetValue(this).ToString().ToLower())}, ");
            stringBuilder.Append($"{nameof(Methods)}: {UUT.EF(String.Join(UUT.DIVIDER, Methods.Select(m => m.Name)))}));");
            return stringBuilder.ToString();
        }

        public Boolean Assert(String Classname, String Description, String CancelNotPassed, String Independent, String Methods) {
            Boolean boolean = String.Equals(this.Classname, Classname);
            boolean &= String.Equals(this.Description, Description);
            boolean &= this.CancelNotPassed == Boolean.Parse(CancelNotPassed);
            boolean &= this.Independent == Boolean.Parse(Independent);
            boolean &= String.Equals(String.Join(UUT.DIVIDER, this.Methods.Select(m => m.Name)).Replace("\"", ""), Methods);
            return boolean;
        }
    }

    public interface IFormat { String Format(); }

    public interface IEvaluate { EVENTS Evaluate(); }

    public abstract class Method : IAssertion, IEvaluate, IFormat {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        public String Value { get; set; }
        public EVENTS Event { get; set; }
        [XmlIgnore] public StringBuilder Log { get; set; } = new StringBuilder();
        public String LogString { get; set; } = String.Empty;
        public List<String> Documents { get; set; } = new List<String>();
        internal static readonly String EXPECTED = "Expected";
        internal static readonly String ACTUAL = "Actual";

        public Method() { }

        public virtual String Assertion() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{nameof(Name)}: {UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(Description)}: {UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(CancelNotPassed)}: {UUT.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}");
            return stringBuilder.ToString();
        }

        public Boolean Assert(String Name, String Description, String CancelNotPassed) {
            return String.Equals(this.Name, Name) && String.Equals(this.Description, Description) && this.CancelNotPassed == Boolean.Parse(CancelNotPassed);
        }

        public abstract EVENTS Evaluate();

        public virtual String Format() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(FormatMessage($"{nameof(Method)}", Name));
            stringBuilder.AppendLine(FormatMessage($"{nameof(CancelNotPassed)}", CancelNotPassed.ToString()));
            stringBuilder.AppendLine(FormatMessage($"{nameof(Description)}", Description));
            return stringBuilder.ToString();
        }

        public String LogFetchAndClear() {
            String s = Log.ToString();
            Log.Clear();
            return s;
        }
    }

    public class MethodCustom : Method, IAssertion, IEvaluate, IFormat {
        // TODO: Eventually; create XML object formatting method in class MethodCustom, so can actually serialize MethodCustom objects into XML in the Value property.
        [XmlElement(nameof(Parameter))] public List<Parameter> Parameters { get; set; }

        public MethodCustom() { }

        public override String Assertion() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Debug.Assert((({GetType().Name}){nameof(TestIndices)}.{nameof(TestIndices.Method)}).Assert(");
            stringBuilder.Append($"{base.Assertion()}");
            if (Parameters.Count > 0) stringBuilder.Append($", {nameof(Parameters)}: {UUT.EF(String.Join(UUT.DIVIDER, Parameters.Select(p => $"{p.Name}={p.Value}")))}");
            stringBuilder.Append("));");
            return stringBuilder.ToString();
        }

        public Boolean Assert(String Name, String Description, String CancelNotPassed, String Parameters = null) {
            Boolean boolean = base.Assert(Name, Description, CancelNotPassed);
            if (Parameters != null) boolean &= String.Equals(String.Join(UUT.DIVIDER, this.Parameters.Select(p => $"{p.Name}={p.Value}")).Replace("\"", ""), Parameters);
            return boolean;
        }

        public override EVENTS Evaluate() { return Event; } // NOTE:  MethodCustoms have their Events set in their TestPlan methods.

        public override String Format() { return base.Format() + Value + Environment.NewLine; }
    }

    public class Parameter {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Value))] public String Value { get; set; }

        public Parameter() { }
    }

    public class MethodInterval : Method, IAssertion, IEvaluate, IFormat {
        [XmlAttribute(nameof(LowComparator))] public LowComparators LowComparator { get; set; }
        [XmlAttribute(nameof(Low))] public Double Low { get; set; }
        [XmlAttribute(nameof(High))] public Double High { get; set; }
        [XmlAttribute(nameof(HighComparator))] public HighComparators HighComparator { get; set; }
        [XmlAttribute(nameof(FractionalDigits))] public UInt32 FractionalDigits { get; set; }
        [XmlAttribute(nameof(UnitPrefix))] public UnitPrefixes UnitPrefix { get; set; }
        [XmlAttribute(nameof(Unit))] public Units Unit { get; set; }
        [XmlAttribute(nameof(UnitSuffix))] public UnitSuffixes UnitSuffix { get; set; }
        [XmlIgnore]
        public static Dictionary<UnitPrefixes, Double> UnitPrefixMultipliers = new Dictionary<UnitPrefixes, Double>() {
            { UnitPrefixes.peta, 1E15 } ,
            { UnitPrefixes.tera, 1E12 },
            { UnitPrefixes.giga, 1E9 },
            { UnitPrefixes.mega, 1E6 },
            { UnitPrefixes.kilo, 1E3 },
            { UnitPrefixes.NONE, 1E0 },
            { UnitPrefixes.milli, 1E-3 },
            { UnitPrefixes.micro, 1E-6 },
            { UnitPrefixes.nano, 1E-9 },
            { UnitPrefixes.pico, 1E-12 },
            { UnitPrefixes.femto, 1E-15}
        };
        public enum LowComparators { GToE, GT }
        public enum HighComparators { LToE, LT }
        public enum UnitPrefixes { NONE, peta, tera, giga, mega, kilo, milli, micro, nano, pico, femto }
        public enum Units { NONE, Amperes, Celcius, Farads, Henries, Hertz, Ohms, Seconds, Siemens, Volts, VoltAmperes, Watts }
        public enum UnitSuffixes { NONE, AC, DC, Peak, PP, RMS }

        public MethodInterval() { }

        public override String Assertion() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Debug.Assert((({GetType().Name}){nameof(TestIndices)}.{nameof(TestIndices.Method)}).Assert(");
            stringBuilder.Append($"{base.Assertion()}, ");
            stringBuilder.Append($"{nameof(LowComparator)}: {UUT.EF(GetType().GetProperty(nameof(LowComparator)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(Low)}: {UUT.EF(GetType().GetProperty(nameof(Low)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(High)}: {UUT.EF(GetType().GetProperty(nameof(High)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(HighComparator)}: {UUT.EF(GetType().GetProperty(nameof(HighComparator)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(FractionalDigits)}: {UUT.EF(GetType().GetProperty(nameof(FractionalDigits)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(UnitPrefix)}: {UUT.EF(GetType().GetProperty(nameof(UnitPrefix)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(Units)}: {UUT.EF(GetType().GetProperty(nameof(Units)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(UnitSuffix)}: {UUT.EF(GetType().GetProperty(nameof(UnitSuffix)).GetValue(this))}));");
            return stringBuilder.ToString();
        }

        public Boolean Assert(String Name, String Description, String CancelNotPassed, String LowComparator, String Low, String High, String HighComparator, String FractionalDigits, String UnitPrefix, String Unit, String UnitSuffix) {
            Boolean boolean = base.Assert(Name, Description, CancelNotPassed);
            boolean &= this.LowComparator == (LowComparators)Enum.Parse(typeof(LowComparators), LowComparator);
            boolean &= this.Low == Double.Parse(Low);
            boolean &= this.High == Double.Parse(High);
            boolean &= this.HighComparator == (HighComparators)Enum.Parse(typeof(HighComparators), HighComparator);
            boolean &= this.FractionalDigits == UInt32.Parse(FractionalDigits);
            boolean &= this.UnitPrefix == (UnitPrefixes)Enum.Parse(typeof(UnitPrefixes), UnitPrefix);
            boolean &= this.Unit == (Units)Enum.Parse(typeof(Units), Unit);
            boolean &= this.UnitSuffix == (UnitSuffixes)Enum.Parse(typeof(UnitSuffixes), UnitSuffix);
            return boolean;
        }

        public override EVENTS Evaluate() {
            if (!Double.TryParse(Value, NumberStyles.Float, CultureInfo.CurrentCulture, out Double d)) throw new InvalidOperationException($"{nameof(MethodInterval)} '{Name}' {nameof(Value)} '{Value}' ≠ System.Double.");
            d /= UnitPrefixMultipliers[UnitPrefix];
            Value = d.ToString("G");
            if (LowComparator is LowComparators.GToE && HighComparator is HighComparators.LToE) return ((Low <= d) && (d <= High)) ? EVENTS.PASS : EVENTS.FAIL;
            if (LowComparator is LowComparators.GToE && HighComparator is HighComparators.LT) return ((Low <= d) && (d < High)) ? EVENTS.PASS : EVENTS.FAIL;
            if (LowComparator is LowComparators.GT && HighComparator is HighComparators.LToE) return ((Low < d) && (d <= High)) ? EVENTS.PASS : EVENTS.FAIL;
            if (LowComparator is LowComparators.GT && HighComparator is HighComparators.LT) return ((Low < d) && (d < High)) ? EVENTS.PASS : EVENTS.FAIL;
            throw new NotImplementedException($"{nameof(MethodInterval)} '{Name}', {nameof(Description)} '{Description}', contains unimplemented comparators '{LowComparator}' and/or '{HighComparator}'.");
        }

        public override String Format() {
            StringBuilder stringBuilder = new StringBuilder(base.Format());
            stringBuilder.AppendLine(FormatMessage(nameof(High), $"{High:G}"));
            stringBuilder.AppendLine(FormatMessage(nameof(Value), $"{Math.Round(Double.Parse(Value), (Int32)FractionalDigits, MidpointRounding.ToEven)}"));
            stringBuilder.AppendLine(FormatMessage(nameof(Low), $"{Low:G}"));
            String units = String.Empty;
            if (UnitPrefix != UnitPrefixes.NONE) units += $"{Enum.GetName(typeof(UnitPrefixes), UnitPrefix)}";
            units += $"{Enum.GetName(typeof(Units), Unit)}";
            if (UnitSuffix != UnitSuffixes.NONE) units += $" {Enum.GetName(typeof(UnitSuffixes), UnitSuffix)}";
            stringBuilder.AppendLine(FormatMessage(nameof(Units), units));
            return stringBuilder.ToString();
        }
    }

    public class MethodProcess : Method, IAssertion, IEvaluate, IFormat {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        [XmlAttribute(nameof(File))] public String File { get; set; }
        [XmlAttribute(nameof(Parameters))] public String Parameters { get; set; }
        [XmlAttribute(nameof(Expected))] public String Expected { get; set; }

        public MethodProcess() { }

        public override String Assertion() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Debug.Assert((({GetType().Name}){nameof(TestIndices)}.{nameof(TestIndices.Method)}).Assert(");
            stringBuilder.Append($"{base.Assertion()}, ");
            stringBuilder.Append($"{nameof(Folder)}: {UUT.EF(GetType().GetProperty(nameof(Folder)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(File)}: {UUT.EF(GetType().GetProperty(nameof(File)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(Parameters)}: {UUT.EF(GetType().GetProperty(nameof(Parameters)).GetValue(this))}, ");
            stringBuilder.Append($"{nameof(Expected)}: {UUT.EF(GetType().GetProperty(nameof(Expected)).GetValue(this))}));");
            return stringBuilder.ToString();
        }

        public Boolean Assert(String Name, String Description, String CancelNotPassed, String Folder, String File, String Parameters, String Expected) {
            Debug.Assert(TestIndices.Method is MethodProcess);
            Boolean boolean = base.Assert(Name, Description, CancelNotPassed);
            boolean &= String.Equals(this.Folder, Folder);
            boolean &= String.Equals(this.File, File);
            boolean &= String.Equals(this.Parameters, Parameters);
            boolean &= String.Equals(this.Expected, Expected);
            return boolean;
        }

        public override EVENTS Evaluate() { return (String.Equals(Expected, Value, StringComparison.Ordinal)) ? EVENTS.PASS : EVENTS.FAIL; }

        public override String Format() {
            StringBuilder stringBuilder = new StringBuilder(base.Format());
            stringBuilder.AppendLine(FormatMessage(EXPECTED, Expected));
            stringBuilder.AppendLine(FormatMessage(ACTUAL, Value));
            return stringBuilder.ToString();
        }
    }

    public class MethodTextual : Method, IAssertion, IEvaluate, IFormat {
        [XmlAttribute(nameof(Text))] public String Text { get; set; }

        public MethodTextual() { }

        public override String Assertion() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Debug.Assert((({GetType().Name}){nameof(TestIndices)}.{nameof(TestIndices.Method)}).Assert(");
            stringBuilder.Append($"{base.Assertion()}, ");
            stringBuilder.Append($"{nameof(Text)}: {UUT.EF(GetType().GetProperty(nameof(Text)).GetValue(this))}));");
            return stringBuilder.ToString();
        }

        public Boolean Assert(String Name, String Description, String CancelNotPassed, String Text) {
            Debug.Assert(TestIndices.Method is MethodTextual);
            Boolean boolean = base.Assert(Name, Description, CancelNotPassed);
            boolean &= String.Equals(this.Text, Text);
            return boolean;
        }

        public override EVENTS Evaluate() { return (String.Equals(Text, Value, StringComparison.Ordinal)) ? EVENTS.PASS : EVENTS.FAIL; }

        public override String Format() {
            StringBuilder stringBuilder = new StringBuilder(base.Format());
            stringBuilder.AppendLine(FormatMessage(EXPECTED, Text));
            stringBuilder.AppendLine(FormatMessage(ACTUAL, Value));
            return stringBuilder.ToString();
        }
    }

    public class Statistics {
        public UInt32 EmergencyStopped = 0;
        public UInt32 Errored = 0;
        public UInt32 Cancelled = 0;
        public UInt32 Unset = 0;
        public UInt32 Failed = 0;
        public UInt32 Passed = 0;
        public UInt32 Informed = 0;

        private readonly DateTime TestSelected = DateTime.Now;

        public Statistics() { }

        public void Update(EVENTS Event) {
            switch (Event) {
                case EVENTS.EMERGENCY_STOP: EmergencyStopped++; break;
                case EVENTS.ERROR: Errored++; break;
                case EVENTS.CANCEL: Cancelled++; break;
                case EVENTS.UNSET: Unset++; break;
                case EVENTS.FAIL: Failed++; break;
                case EVENTS.PASS: Passed++; break;
                case EVENTS.INFORMATION: Informed++; break;
                default: throw new NotImplementedException(NotImplementedMessageEnum<EVENTS>(Enum.GetName(typeof(EVENTS), Event)));
            }
        }

        public String Time() {
            TimeSpan elapsedTime = DateTime.Now - TestSelected;
            return $"{(elapsedTime.Days != 0 ? elapsedTime.Days.ToString() + ":" : String.Empty)}{elapsedTime.Hours}:{elapsedTime.Minutes:00}";
        }
        public Double FractionEmergencyStopped() { return Convert.ToDouble(EmergencyStopped) / Convert.ToDouble(Tested()); }
        public Double FractionErrored() { return Convert.ToDouble(Errored) / Convert.ToDouble(Tested()); }
        public Double FractionCancelled() { return Convert.ToDouble(Cancelled) / Convert.ToDouble(Tested()); }
        public Double FractionUnset() { return Convert.ToDouble(Unset) / Convert.ToDouble(Tested()); }
        public Double FractionFailed() { return Convert.ToDouble(Failed) / Convert.ToDouble(Tested()); }
        public Double FractionPassed() { return Convert.ToDouble(Passed) / Convert.ToDouble(Tested()); }
        public Double FractionInformed() { return Convert.ToDouble(Informed) / Convert.ToDouble(Tested()); }
        public UInt32 Tested() { return EmergencyStopped + Errored + Cancelled + Unset + Failed + Passed + Informed; }
    }

    public static class TestIndices {
        public static TestOperation TestOperation { get; set; } = null;
        public static TestGroup TestGroup { get; set; } = null;
        public static Method Method { get; set; } = null;
        public static void Nullify() {
            TestOperation = null;
            TestGroup = null;
            Method = null;
        }
    }
}