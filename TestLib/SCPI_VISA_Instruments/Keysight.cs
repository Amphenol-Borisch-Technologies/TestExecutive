using System;
using System.Collections.Generic;

// All Agilent.CommandExpert.ScpiNet drivers are procured by adding new SCPI VISA Instruments in Keysight's Command Expert app software.
//  - Command Expert literally downloads & installs Agilent.CommandExpert.ScpiNet drivers when new SVIs are added.
//  - The Agilent.CommandExpert.ScpiNet drivers are installed into folder C:\ProgramData\Keysight\Command Expert\ScpiNetDrivers.
// https://www.keysight.com/us/en/lib/software-detail/computer-software/command-expert-downloads-2151326.html
//
// Enthusiastically recommend using Command Expert to generate SCPI commands, which are directly exportable as .Net statements.
// https://www.keysight.com/us/en/search.html/command+expert
//
// NOTE:  Unlike all other classes in namespace ABT.TestSpace.TestExec.SCPI_VISA_Instruments, classes in SCPI_VISA utilize only VISA Addresses,
// not Instrument objects contained in their SCPI_VISA_Instrument objects.
namespace ABT.TestSpace.TestExec.SCPI_VISA_Instruments {
    public enum CHANNEL { C1, C2 }
    public enum MMD { MINimum, MAXimum, DEFault }
    public enum TERMINAL { Front, Rear };

    public static class Keysight {
        public const String AUTO = "AUTO";
        public const String ASCII = "ASCii";

        public static readonly Dictionary<CHANNEL, String> Channels = new Dictionary<CHANNEL, String> {
            { CHANNEL.C1, "(@1)" },
            { CHANNEL.C2, "(@2)" }
        };

        public static readonly String MINimum = Enum.GetName(typeof(MMD), MMD.MINimum);
        public static readonly String MAXimum = Enum.GetName(typeof(MMD), MMD.MAXimum);
        public static readonly String DEFault = Enum.GetName(typeof(MMD), MMD.DEFault);
    }
}