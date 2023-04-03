﻿using System;
using System.Configuration;

namespace TestLibrary.AppConfig {
    public class ConfigUUT {
        public String Customer { get; private set; }
        public String Type { get; private set; }
        public String Number { get; private set; }
        public String Revision { get; private set; }
        public String Description { get; private set; }
        public String TestSpecification { get; private set; }
        public String DocumentationFolder { get; private set; }
        public String SerialNumber { get; set; }
        public String EventCode { get; set; }

        private ConfigUUT(String customer, String type, String number, String revision, String description, String testSpecification, String documentationFolder, String serialNumber, String eventCode) {
            this.Customer = customer;
            this.Type = type;
            this.Number = number;
            this.Revision = revision;
            this.Description = description;
            this.TestSpecification = testSpecification;
            this.DocumentationFolder = documentationFolder;
            this.SerialNumber = serialNumber;
            this.EventCode = eventCode;
        }

        public static ConfigUUT Get() {
            return new ConfigUUT(
                ConfigurationManager.AppSettings["UUT_Customer"].Trim(),
                ConfigurationManager.AppSettings["UUT_Type"].Trim(),
                ConfigurationManager.AppSettings["UUT_Number"].Trim(),
                ConfigurationManager.AppSettings["UUT_Revision"].Trim(),
                ConfigurationManager.AppSettings["UUT_Description"].Trim(),
                ConfigurationManager.AppSettings["UUT_TestSpecification"].Trim(),
                ConfigurationManager.AppSettings["UUT_DocumentationFolder"].Trim(),
                String.Empty, // Input during testing.
                EventCodes.UNSET // Determined post-test.
            );
        }
    }
}
