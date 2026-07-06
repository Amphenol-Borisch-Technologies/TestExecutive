using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Agilent.CommandExpert.ScpiNet.Ag33500B_33600A_2_09;
using Agilent.CommandExpert.ScpiNet.Ag3466x_2_08;
using Agilent.CommandExpert.ScpiNet.AgE3610XB_1_0_0_1_00;
using Agilent.CommandExpert.ScpiNet.AgE36200_1_0_0_1_0_2_1_00;
using Agilent.CommandExpert.ScpiNet.AgEL30000_1_2_5_1_0_6_17_114;

namespace ABT.TestSpace.TestExec.SCPI_VISA_Instruments {
    // NOTE:  https://forums.ni.com/t5/Instrument-Control-GPIB-Serial/IVI-Drivers-Pros-and-Cons/td-p/4165671.
    public enum SCPI_IDENTITY { Manufacturer, Model, SerialNumber, FirmwareRevision }
    // Example: "Keysight Technologies,E36103B,MY61001983,1.0.2-1.02".

    public class SCPI_VISA_Instrument {
        public readonly Alias ID;
        public readonly String Description;
        public readonly String Address;
        public readonly String Identity;
        public readonly Boolean LoadOrStimulus;
        public readonly Object Instrument; // NOTE:  The assumption, thus far proven correct, is that Keysight's SCPI drivers don't contain state, thus can be readonly.

        private SCPI_VISA_Instrument(Alias id, String description, String address) {
            ID = id;
            Description = description;
            Address = address;

            try {
                Identity = SCPI99.IdentityGet(address, SCPI_IDENTITY.Model);

                switch (Identity) {
                    case EL_34143A.MODEL:
                        Instrument = new AgEL30000(Address);
                        LoadOrStimulus = EL_34143A.LoadOrStimulus;
                        EL_34143A.Initialize(this);
                        break;
                    case MM_34461A.MODEL:
                        Instrument = new Ag3466x(Address);
                        LoadOrStimulus = MM_34461A.LoadOrStimulus;
                        MM_34461A.Initialize(this);
                        MM_34461A.TerminalsSetRear(this);
                        break;
                    case PS_E36103B.MODEL:
                    case PS_E36105B.MODEL:
                        Instrument = new AgE3610XB(Address);
                        LoadOrStimulus = PS_E3610xB.LoadOrStimulus;
                        PS_E3610xB.Initialize(this);
                        break;
                    case PS_E36234A.MODEL:
                        Instrument = new AgE36200(Address);
                        LoadOrStimulus = PS_E36234A.LoadOrStimulus;
                        PS_E36234A.Initialize(this);
                        break;
                    case WG_33509B.MODEL:
                        Instrument = new Ag33500B_33600A(Address);
                        LoadOrStimulus = WG_33509B.LoadOrStimulus;
                        WG_33509B.Initialize(this);
                        break;
                    default:
                        throw new NotImplementedException($"Unimplemented SCPI VISA Instrument '{Identity}'.");
                }
            } catch (Exception e) {
                throw new InvalidOperationException(SCPI99.ErrorMessageGet(this, "Check to see if Instrument is powered and its interface communicating."), e);
            }
        }

        //public static Dictionary<Alias, SCPI_VISA_Instrument> Get() {
        //    IEnumerable<SCPI_VISA_Instrument> svis =
        //        from svi in XElement.Load(TestExecutive.GlobalConfigurationFile).Elements("SCPI_VISA_Instruments").Elements("SVI")
        //        select new SCPI_VISA_Instrument(new Alias(svi.Element("ID").Value), svi.Element("Description").Value, svi.Element("Address").Value);
        //    Dictionary<Alias, SCPI_VISA_Instrument> SVIs = new Dictionary<Alias, SCPI_VISA_Instrument>();
        //    foreach (SCPI_VISA_Instrument svi in svis) SVIs.Add(new Alias(svi.ID.ToString()), svi);
        //    return SVIs;
        //}

        //public static String GetInfo(SCPI_VISA_Instrument SVI, String optionalHeader = "") {
        //    String info = (String.Equals(optionalHeader, "")) ? optionalHeader : optionalHeader += Environment.NewLine;
        //    foreach (PropertyInfo pi in SVI.GetType().GetProperties()) info += $"{pi.Name.PadLeft(Logger.SPACES_21.Length)}: '{pi.GetValue(SVI)}'{Environment.NewLine}";
        //    return info;
        //}

        public class Alias {
            public readonly String ID;

            public Alias(String name) { ID = name; }

            public override Boolean Equals(Object obj) {
                Alias a = obj as Alias;
                if (ReferenceEquals(this, a)) return true;
                return a != null && ID == a.ID;
            }

            public override Int32 GetHashCode() { return 3 * ID.GetHashCode(); }

            public override String ToString() { return ID; }
        }
    }
}
