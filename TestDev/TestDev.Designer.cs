namespace TestDev {
    partial class TestDev {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestDev));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.TSMI_BarcodeScanner = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_TestDefinitions = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_TestDefinitions_TestExec = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_TestDefinitions_TestPlans = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Definitions_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.TSMI_TestDefinitions_Validate = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_TestPlans = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_TestPlans_Choose = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Generate = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Generate_InstrumentAliases = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Generate_TestPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Generate_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.TSMI_Generate_Project = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Keysight = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Keysight_CommandExpert = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Keysight_ConnectionExpert = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Microsoft = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Microsoft_SQL_ServerManagementStudio = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Microsoft_VisualStudio = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Microsoft_VisualStudioCode = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Microsoft_XML_Notepad = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_BarcodeScanner,
            this.TSMI_TestDefinitions,
            this.TSMI_TestPlans,
            this.TSMI_Generate,
            this.TSMI_Keysight,
            this.TSMI_Microsoft});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(648, 28);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // TSMI_BarcodeScanner
            // 
            this.TSMI_BarcodeScanner.Name = "TSMI_BarcodeScanner";
            this.TSMI_BarcodeScanner.Size = new System.Drawing.Size(134, 24);
            this.TSMI_BarcodeScanner.Text = "&Barcode Scanner";
            this.TSMI_BarcodeScanner.Click += new System.EventHandler(this.TSMI_BarcodeScanner_Click);
            // 
            // TSMI_TestDefinitions
            // 
            this.TSMI_TestDefinitions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_TestDefinitions_TestExec,
            this.TSMI_TestDefinitions_TestPlans,
            this.TSMI_Definitions_Separator,
            this.TSMI_TestDefinitions_Validate});
            this.TSMI_TestDefinitions.Name = "TSMI_TestDefinitions";
            this.TSMI_TestDefinitions.Size = new System.Drawing.Size(121, 24);
            this.TSMI_TestDefinitions.Text = "Test&Definitions";
            // 
            // TSMI_TestDefinitions_TestExec
            // 
            this.TSMI_TestDefinitions_TestExec.Name = "TSMI_TestDefinitions_TestExec";
            this.TSMI_TestDefinitions_TestExec.Size = new System.Drawing.Size(224, 26);
            this.TSMI_TestDefinitions_TestExec.Text = "Test&Exec";
            this.TSMI_TestDefinitions_TestExec.ToolTipText = "Open/Edit a TestExec Definition";
            this.TSMI_TestDefinitions_TestExec.Click += new System.EventHandler(this.TSMI_TestDefinitions_TestExec_Click);
            // 
            // TSMI_TestDefinitions_TestPlans
            // 
            this.TSMI_TestDefinitions_TestPlans.Name = "TSMI_TestDefinitions_TestPlans";
            this.TSMI_TestDefinitions_TestPlans.Size = new System.Drawing.Size(224, 26);
            this.TSMI_TestDefinitions_TestPlans.Text = "Test&Plans";
            this.TSMI_TestDefinitions_TestPlans.ToolTipText = "Open/Edit a TestPlan Definition";
            this.TSMI_TestDefinitions_TestPlans.Click += new System.EventHandler(this.TSMI_TestDefinitions_TestPlans_Click);
            // 
            // TSMI_Definitions_Separator
            // 
            this.TSMI_Definitions_Separator.Name = "TSMI_Definitions_Separator";
            this.TSMI_Definitions_Separator.Size = new System.Drawing.Size(221, 6);
            // 
            // TSMI_TestDefinitions_Validate
            // 
            this.TSMI_TestDefinitions_Validate.Name = "TSMI_TestDefinitions_Validate";
            this.TSMI_TestDefinitions_Validate.Size = new System.Drawing.Size(224, 26);
            this.TSMI_TestDefinitions_Validate.Text = "&Validate";
            this.TSMI_TestDefinitions_Validate.ToolTipText = "Validate a TestPlan Definition";
            this.TSMI_TestDefinitions_Validate.Click += new System.EventHandler(this.TSMI_TestDefinitions_Validate_Click);
            // 
            // TSMI_TestPlans
            // 
            this.TSMI_TestPlans.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_TestPlans_Choose});
            this.TSMI_TestPlans.Name = "TSMI_TestPlans";
            this.TSMI_TestPlans.Size = new System.Drawing.Size(83, 24);
            this.TSMI_TestPlans.Text = "Test&Plans";
            // 
            // TSMI_TestPlans_Choose
            // 
            this.TSMI_TestPlans_Choose.Name = "TSMI_TestPlans_Choose";
            this.TSMI_TestPlans_Choose.Size = new System.Drawing.Size(224, 26);
            this.TSMI_TestPlans_Choose.Text = "&Choose";
            this.TSMI_TestPlans_Choose.ToolTipText = "Choose & Start a TestPlan";
            this.TSMI_TestPlans_Choose.Click += new System.EventHandler(this.TSMI_TestPlans_Choose_Click);
            // 
            // TSMI_Generate
            // 
            this.TSMI_Generate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Generate_InstrumentAliases,
            this.TSMI_Generate_TestPlan,
            this.TSMI_Generate_Separator,
            this.TSMI_Generate_Project});
            this.TSMI_Generate.Name = "TSMI_Generate";
            this.TSMI_Generate.Size = new System.Drawing.Size(83, 24);
            this.TSMI_Generate.Text = "&Generate";
            // 
            // TSMI_Generate_InstrumentAliases
            // 
            this.TSMI_Generate_InstrumentAliases.Name = "TSMI_Generate_InstrumentAliases";
            this.TSMI_Generate_InstrumentAliases.Size = new System.Drawing.Size(212, 26);
            this.TSMI_Generate_InstrumentAliases.Text = "&Instrument Aliases";
            this.TSMI_Generate_InstrumentAliases.ToolTipText = "Generate C# Instrument Aliases";
            this.TSMI_Generate_InstrumentAliases.Click += new System.EventHandler(this.TSMI_Generate_InstrumentAliases_Click);
            // 
            // TSMI_Generate_TestPlan
            // 
            this.TSMI_Generate_TestPlan.Name = "TSMI_Generate_TestPlan";
            this.TSMI_Generate_TestPlan.Size = new System.Drawing.Size(212, 26);
            this.TSMI_Generate_TestPlan.Text = "&TestPlan";
            this.TSMI_Generate_TestPlan.ToolTipText = "Generate C# TestDefinition Classes & Methods";
            this.TSMI_Generate_TestPlan.Click += new System.EventHandler(this.TSMI_Generate_TestPlan_Click);
            // 
            // TSMI_Generate_Separator
            // 
            this.TSMI_Generate_Separator.Name = "TSMI_Generate_Separator";
            this.TSMI_Generate_Separator.Size = new System.Drawing.Size(209, 6);
            // 
            // TSMI_Generate_Project
            // 
            this.TSMI_Generate_Project.Name = "TSMI_Generate_Project";
            this.TSMI_Generate_Project.Size = new System.Drawing.Size(212, 26);
            this.TSMI_Generate_Project.Text = "&Project";
            this.TSMI_Generate_Project.ToolTipText = "Generate new C# TestPlan Project";
            this.TSMI_Generate_Project.Click += new System.EventHandler(this.TSMI_Generate_Project_Click);
            // 
            // TSMI_Keysight
            // 
            this.TSMI_Keysight.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Keysight_CommandExpert,
            this.TSMI_Keysight_ConnectionExpert});
            this.TSMI_Keysight.Name = "TSMI_Keysight";
            this.TSMI_Keysight.Size = new System.Drawing.Size(79, 24);
            this.TSMI_Keysight.Text = "&Keysight";
            // 
            // TSMI_Keysight_CommandExpert
            // 
            this.TSMI_Keysight_CommandExpert.Name = "TSMI_Keysight_CommandExpert";
            this.TSMI_Keysight_CommandExpert.Size = new System.Drawing.Size(213, 26);
            this.TSMI_Keysight_CommandExpert.Text = "Co&mmand Expert";
            this.TSMI_Keysight_CommandExpert.ToolTipText = "Open Keysight Command Expert";
            this.TSMI_Keysight_CommandExpert.Click += new System.EventHandler(this.TSMI_Keysight_CommandExpert_Click);
            // 
            // TSMI_Keysight_ConnectionExpert
            // 
            this.TSMI_Keysight_ConnectionExpert.Name = "TSMI_Keysight_ConnectionExpert";
            this.TSMI_Keysight_ConnectionExpert.Size = new System.Drawing.Size(213, 26);
            this.TSMI_Keysight_ConnectionExpert.Text = "Co&nnection Expert";
            this.TSMI_Keysight_ConnectionExpert.ToolTipText = "Open Keysight Connection Expert";
            this.TSMI_Keysight_ConnectionExpert.Click += new System.EventHandler(this.TSMI_Keysight_ConnectionExpert_Click);
            // 
            // TSMI_Microsoft
            // 
            this.TSMI_Microsoft.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Microsoft_SQL_ServerManagementStudio,
            this.TSMI_Microsoft_VisualStudio,
            this.TSMI_Microsoft_VisualStudioCode,
            this.TSMI_Microsoft_XML_Notepad});
            this.TSMI_Microsoft.Name = "TSMI_Microsoft";
            this.TSMI_Microsoft.Size = new System.Drawing.Size(86, 24);
            this.TSMI_Microsoft.Text = "&Microsoft";
            // 
            // TSMI_Microsoft_SQL_ServerManagementStudio
            // 
            this.TSMI_Microsoft_SQL_ServerManagementStudio.Name = "TSMI_Microsoft_SQL_ServerManagementStudio";
            this.TSMI_Microsoft_SQL_ServerManagementStudio.Size = new System.Drawing.Size(302, 26);
            this.TSMI_Microsoft_SQL_ServerManagementStudio.Text = "&SQL Server Management Studio";
            this.TSMI_Microsoft_SQL_ServerManagementStudio.ToolTipText = "Open SQL Server Management Studio";
            this.TSMI_Microsoft_SQL_ServerManagementStudio.Click += new System.EventHandler(this.TSMI_Microsoft_SQL_ServerManagementStudio_Click);
            // 
            // TSMI_Microsoft_VisualStudio
            // 
            this.TSMI_Microsoft_VisualStudio.Name = "TSMI_Microsoft_VisualStudio";
            this.TSMI_Microsoft_VisualStudio.Size = new System.Drawing.Size(302, 26);
            this.TSMI_Microsoft_VisualStudio.Text = "&Visual Studio";
            this.TSMI_Microsoft_VisualStudio.ToolTipText = "Open Visual Studio";
            this.TSMI_Microsoft_VisualStudio.Click += new System.EventHandler(this.TSMI_Microsoft_VisualStudio_Click);
            // 
            // TSMI_Microsoft_VisualStudioCode
            // 
            this.TSMI_Microsoft_VisualStudioCode.Name = "TSMI_Microsoft_VisualStudioCode";
            this.TSMI_Microsoft_VisualStudioCode.Size = new System.Drawing.Size(302, 26);
            this.TSMI_Microsoft_VisualStudioCode.Text = "Visual Studio &Code";
            this.TSMI_Microsoft_VisualStudioCode.ToolTipText = "Open Visual Studio Code";
            this.TSMI_Microsoft_VisualStudioCode.Click += new System.EventHandler(this.TSMI_Microsoft_VisualStudioCode_Click);
            // 
            // TSMI_Microsoft_XML_Notepad
            // 
            this.TSMI_Microsoft_XML_Notepad.Name = "TSMI_Microsoft_XML_Notepad";
            this.TSMI_Microsoft_XML_Notepad.Size = new System.Drawing.Size(302, 26);
            this.TSMI_Microsoft_XML_Notepad.Text = "&XML Notepad";
            this.TSMI_Microsoft_XML_Notepad.ToolTipText = "Open XML Notepad";
            this.TSMI_Microsoft_XML_Notepad.Click += new System.EventHandler(this.TSMI_Microsoft_XML_Notepad_Click);
            // 
            // TestDev
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 37);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "TestDev";
            this.Text = "TestDev";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Generate;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Generate_TestPlan;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestDefinitions;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestDefinitions_TestExec;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestDefinitions_TestPlans;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestDefinitions_Validate;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Generate_Project;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Generate_InstrumentAliases;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Keysight;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Keysight_CommandExpert;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Keysight_ConnectionExpert;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Microsoft;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Microsoft_SQL_ServerManagementStudio;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Microsoft_VisualStudio;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Microsoft_VisualStudioCode;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Microsoft_XML_Notepad;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestPlans;
        private System.Windows.Forms.ToolStripSeparator TSMI_Definitions_Separator;
        private System.Windows.Forms.ToolStripSeparator TSMI_Generate_Separator;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestPlans_Choose;
        private System.Windows.Forms.ToolStripMenuItem TSMI_BarcodeScanner;
    }
}

