namespace ABT.Test.TestExecutive.TestExec {
    public abstract partial class TestExec : System.Windows.Forms.Form {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestExec));
            this.ButtonRun = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.TextTest = new System.Windows.Forms.TextBox();
            this.LabelEvent = new System.Windows.Forms.Label();
            this.rtfResults = new System.Windows.Forms.RichTextBox();
            this.ButtonSelect = new System.Windows.Forms.Button();
            this.ButtonEmergencyStop = new System.Windows.Forms.Button();
            this.MS = new System.Windows.Forms.MenuStrip();
            this.TSMI_TestPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_TestPlan_Choose = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_TestPlan_SaveResults = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_File_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.TSMI_TestPlan_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Feedback = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Feedback_Compliments = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Feedback_ComplimentsPraiseεPlaudits = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Feedback_ComplimentsMoney = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Feedback_Critiques = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Feedback_CritiquesBugReport = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Feedback_CritiquesImprovementRequest = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_System = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_System_ColorCode = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_System_Manuals = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_System_ManualsBarcodeScanner = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_System_ManualsInstruments = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_System_SelfTests = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_System_DiagnosticsInstruments = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT_eDocs = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT_Manuals = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT_Statistics = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT_StatisticsDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT_StatisticsReset = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT_TestData = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT_TestDataP_DriveTDR_Folder = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_UUT_TestDataSQL_ReportingAndQuerying = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_About = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_About_TestExec = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_About_TestPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusTimeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusStatisticsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusModeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusCustomLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MS.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonRun
            // 
            this.ButtonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonRun.BackColor = System.Drawing.Color.Green;
            this.ButtonRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonRun.Location = new System.Drawing.Point(203, 661);
            this.ButtonRun.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonRun.Name = "ButtonRun";
            this.ButtonRun.Size = new System.Drawing.Size(117, 64);
            this.ButtonRun.TabIndex = 1;
            this.ButtonRun.TabStop = false;
            this.ButtonRun.Text = "Run";
            this.ButtonRun.UseVisualStyleBackColor = false;
            this.ButtonRun.Click += new System.EventHandler(this.ButtonRun_Clicked);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonCancel.BackColor = System.Drawing.Color.Yellow;
            this.ButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonCancel.Location = new System.Drawing.Point(383, 658);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(117, 64);
            this.ButtonCancel.TabIndex = 2;
            this.ButtonCancel.TabStop = false;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = false;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Clicked);
            this.ButtonCancel.Enter += new System.EventHandler(this.ButtonCancel_Enter);
            // 
            // TextTest
            // 
            this.TextTest.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.TextTest.Location = new System.Drawing.Point(741, 689);
            this.TextTest.Margin = new System.Windows.Forms.Padding(4);
            this.TextTest.Name = "TextTest";
            this.TextTest.ReadOnly = true;
            this.TextTest.Size = new System.Drawing.Size(169, 22);
            this.TextTest.TabIndex = 9;
            this.TextTest.TabStop = false;
            this.TextTest.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LabelEvent
            // 
            this.LabelEvent.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.LabelEvent.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelEvent.Location = new System.Drawing.Point(765, 661);
            this.LabelEvent.Margin = new System.Windows.Forms.Padding(4);
            this.LabelEvent.Name = "LabelEvent";
            this.LabelEvent.Size = new System.Drawing.Size(120, 20);
            this.LabelEvent.TabIndex = 8;
            this.LabelEvent.Text = "Event";
            this.LabelEvent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LabelEvent.UseWaitCursor = true;
            // 
            // rtfResults
            // 
            this.rtfResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtfResults.BackColor = System.Drawing.SystemColors.Window;
            this.rtfResults.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtfResults.Location = new System.Drawing.Point(31, 43);
            this.rtfResults.Margin = new System.Windows.Forms.Padding(4);
            this.rtfResults.Name = "rtfResults";
            this.rtfResults.ReadOnly = true;
            this.rtfResults.Size = new System.Drawing.Size(1511, 582);
            this.rtfResults.TabIndex = 7;
            this.rtfResults.TabStop = false;
            this.rtfResults.Text = "";
            // 
            // ButtonSelect
            // 
            this.ButtonSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonSelect.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.ButtonSelect.Location = new System.Drawing.Point(31, 662);
            this.ButtonSelect.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonSelect.Name = "ButtonSelect";
            this.ButtonSelect.Size = new System.Drawing.Size(117, 58);
            this.ButtonSelect.TabIndex = 0;
            this.ButtonSelect.TabStop = false;
            this.ButtonSelect.Text = "Select";
            this.ButtonSelect.UseVisualStyleBackColor = true;
            this.ButtonSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // ButtonEmergencyStop
            // 
            this.ButtonEmergencyStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonEmergencyStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonEmergencyStop.Image = ((System.Drawing.Image)(resources.GetObject("ButtonEmergencyStop.Image")));
            this.ButtonEmergencyStop.Location = new System.Drawing.Point(1453, 646);
            this.ButtonEmergencyStop.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonEmergencyStop.Name = "ButtonEmergencyStop";
            this.ButtonEmergencyStop.Size = new System.Drawing.Size(89, 88);
            this.ButtonEmergencyStop.TabIndex = 5;
            this.ButtonEmergencyStop.TabStop = false;
            this.ButtonEmergencyStop.Text = "Emergency Stop";
            this.ButtonEmergencyStop.UseVisualStyleBackColor = true;
            this.ButtonEmergencyStop.Click += new System.EventHandler(this.ButtonEmergencyStop_Clicked);
            this.ButtonEmergencyStop.Enter += new System.EventHandler(this.ButtonEmergencyStop_Enter);
            // 
            // MS
            // 
            this.MS.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.MS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_TestPlan,
            this.TSMI_Feedback,
            this.TSMI_System,
            this.TSMI_UUT,
            this.TSMI_About});
            this.MS.Location = new System.Drawing.Point(0, 0);
            this.MS.Name = "MS";
            this.MS.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.MS.Size = new System.Drawing.Size(1573, 28);
            this.MS.TabIndex = 6;
            this.MS.TabStop = true;
            // 
            // TSMI_TestPlan
            // 
            this.TSMI_TestPlan.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_TestPlan_Choose,
            this.TSMI_TestPlan_SaveResults,
            this.TSMI_File_Separator,
            this.TSMI_TestPlan_Exit});
            this.TSMI_TestPlan.Name = "TSMI_TestPlan";
            this.TSMI_TestPlan.Size = new System.Drawing.Size(77, 24);
            this.TSMI_TestPlan.Text = "&TestPlan";
            // 
            // TSMI_TestPlan_Choose
            // 
            this.TSMI_TestPlan_Choose.Name = "TSMI_TestPlan_Choose";
            this.TSMI_TestPlan_Choose.Size = new System.Drawing.Size(173, 26);
            this.TSMI_TestPlan_Choose.Text = "&Choose";
            this.TSMI_TestPlan_Choose.ToolTipText = "Closes current TestPlan & open another.";
            this.TSMI_TestPlan_Choose.Click += new System.EventHandler(this.TSMI_TestPlan_Choose_Click);
            // 
            // TSMI_TestPlan_SaveResults
            // 
            this.TSMI_TestPlan_SaveResults.Image = ((System.Drawing.Image)(resources.GetObject("TSMI_TestPlan_SaveResults.Image")));
            this.TSMI_TestPlan_SaveResults.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSMI_TestPlan_SaveResults.Name = "TSMI_TestPlan_SaveResults";
            this.TSMI_TestPlan_SaveResults.Size = new System.Drawing.Size(173, 26);
            this.TSMI_TestPlan_SaveResults.Text = "&Save Results";
            this.TSMI_TestPlan_SaveResults.ToolTipText = "Save UUT results.";
            this.TSMI_TestPlan_SaveResults.Click += new System.EventHandler(this.TSMI_TestPlan_SaveResults_Click);
            // 
            // TSMI_File_Separator
            // 
            this.TSMI_File_Separator.Name = "TSMI_File_Separator";
            this.TSMI_File_Separator.Size = new System.Drawing.Size(170, 6);
            // 
            // TSMI_TestPlan_Exit
            // 
            this.TSMI_TestPlan_Exit.Name = "TSMI_TestPlan_Exit";
            this.TSMI_TestPlan_Exit.Size = new System.Drawing.Size(173, 26);
            this.TSMI_TestPlan_Exit.Text = "&Exit";
            this.TSMI_TestPlan_Exit.ToolTipText = "Close TestPlan.";
            this.TSMI_TestPlan_Exit.Click += new System.EventHandler(this.TSMI_TestPlan_Exit_Click);
            // 
            // TSMI_Feedback
            // 
            this.TSMI_Feedback.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Feedback_Compliments,
            this.TSMI_Feedback_Critiques});
            this.TSMI_Feedback.Name = "TSMI_Feedback";
            this.TSMI_Feedback.Size = new System.Drawing.Size(86, 24);
            this.TSMI_Feedback.Text = "Feed&back";
            // 
            // TSMI_Feedback_Compliments
            // 
            this.TSMI_Feedback_Compliments.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Feedback_ComplimentsPraiseεPlaudits,
            this.TSMI_Feedback_ComplimentsMoney});
            this.TSMI_Feedback_Compliments.Name = "TSMI_Feedback_Compliments";
            this.TSMI_Feedback_Compliments.Size = new System.Drawing.Size(180, 26);
            this.TSMI_Feedback_Compliments.Text = "&Compliments";
            // 
            // TSMI_Feedback_ComplimentsPraiseεPlaudits
            // 
            this.TSMI_Feedback_ComplimentsPraiseεPlaudits.Name = "TSMI_Feedback_ComplimentsPraiseεPlaudits";
            this.TSMI_Feedback_ComplimentsPraiseεPlaudits.Size = new System.Drawing.Size(203, 26);
            this.TSMI_Feedback_ComplimentsPraiseεPlaudits.Text = "&Praise && Plaudits";
            this.TSMI_Feedback_ComplimentsPraiseεPlaudits.ToolTipText = "\"I can live for two months on a good compliment.\" - Mark Twain";
            this.TSMI_Feedback_ComplimentsPraiseεPlaudits.Click += new System.EventHandler(this.TSMI_Feedback_ComplimentsPraiseεPlaudits_Click);
            // 
            // TSMI_Feedback_ComplimentsMoney
            // 
            this.TSMI_Feedback_ComplimentsMoney.Name = "TSMI_Feedback_ComplimentsMoney";
            this.TSMI_Feedback_ComplimentsMoney.Size = new System.Drawing.Size(203, 26);
            this.TSMI_Feedback_ComplimentsMoney.Text = "&Money!";
            this.TSMI_Feedback_ComplimentsMoney.ToolTipText = "For a good cause! ";
            this.TSMI_Feedback_ComplimentsMoney.Click += new System.EventHandler(this.TSMI_Feedback_ComplimentsMoney_Click);
            // 
            // TSMI_Feedback_Critiques
            // 
            this.TSMI_Feedback_Critiques.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Feedback_CritiquesBugReport,
            this.TSMI_Feedback_CritiquesImprovementRequest});
            this.TSMI_Feedback_Critiques.Name = "TSMI_Feedback_Critiques";
            this.TSMI_Feedback_Critiques.Size = new System.Drawing.Size(180, 26);
            this.TSMI_Feedback_Critiques.Text = "Criti&ques";
            // 
            // TSMI_Feedback_CritiquesBugReport
            // 
            this.TSMI_Feedback_CritiquesBugReport.Name = "TSMI_Feedback_CritiquesBugReport";
            this.TSMI_Feedback_CritiquesBugReport.Size = new System.Drawing.Size(238, 26);
            this.TSMI_Feedback_CritiquesBugReport.Text = "&Bug Report";
            this.TSMI_Feedback_CritiquesBugReport.ToolTipText = "\"The devil is is in the details.\" - Friedrich Nietzsche";
            this.TSMI_Feedback_CritiquesBugReport.Click += new System.EventHandler(this.TSMI_Feedback_CritiqueBugReport_Click);
            // 
            // TSMI_Feedback_CritiquesImprovementRequest
            // 
            this.TSMI_Feedback_CritiquesImprovementRequest.Name = "TSMI_Feedback_CritiquesImprovementRequest";
            this.TSMI_Feedback_CritiquesImprovementRequest.Size = new System.Drawing.Size(238, 26);
            this.TSMI_Feedback_CritiquesImprovementRequest.Text = "&Improvement Request";
            this.TSMI_Feedback_CritiquesImprovementRequest.ToolTipText = "\"God is in the details.\" - Mies van der Rohe";
            this.TSMI_Feedback_CritiquesImprovementRequest.Click += new System.EventHandler(this.TSMI_Feedback_CritiqueImprovementRequest_Click);
            // 
            // TSMI_System
            // 
            this.TSMI_System.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_System_ColorCode,
            this.TSMI_System_Manuals,
            this.TSMI_System_SelfTests});
            this.TSMI_System.Name = "TSMI_System";
            this.TSMI_System.Size = new System.Drawing.Size(70, 24);
            this.TSMI_System.Text = "S&ystem";
            // 
            // TSMI_System_ColorCode
            // 
            this.TSMI_System_ColorCode.Name = "TSMI_System_ColorCode";
            this.TSMI_System_ColorCode.Size = new System.Drawing.Size(167, 26);
            this.TSMI_System_ColorCode.Text = "&Color Code";
            this.TSMI_System_ColorCode.ToolTipText = "EVENTful!";
            this.TSMI_System_ColorCode.Click += new System.EventHandler(this.TSMI_System_ColorCode_Click);
            // 
            // TSMI_System_Manuals
            // 
            this.TSMI_System_Manuals.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_System_ManualsBarcodeScanner,
            this.TSMI_System_ManualsInstruments});
            this.TSMI_System_Manuals.Name = "TSMI_System_Manuals";
            this.TSMI_System_Manuals.Size = new System.Drawing.Size(167, 26);
            this.TSMI_System_Manuals.Text = "&Manuals";
            // 
            // TSMI_System_ManualsBarcodeScanner
            // 
            this.TSMI_System_ManualsBarcodeScanner.Name = "TSMI_System_ManualsBarcodeScanner";
            this.TSMI_System_ManualsBarcodeScanner.Size = new System.Drawing.Size(203, 26);
            this.TSMI_System_ManualsBarcodeScanner.Text = "&Barcode Scanner";
            this.TSMI_System_ManualsBarcodeScanner.ToolTipText = "If you\'re bored...";
            this.TSMI_System_ManualsBarcodeScanner.Click += new System.EventHandler(this.TSMI_System_ManualsBarcodeScanner_Click);
            // 
            // TSMI_System_ManualsInstruments
            // 
            this.TSMI_System_ManualsInstruments.Name = "TSMI_System_ManualsInstruments";
            this.TSMI_System_ManualsInstruments.Size = new System.Drawing.Size(203, 26);
            this.TSMI_System_ManualsInstruments.Text = "&Instruments";
            this.TSMI_System_ManualsInstruments.ToolTipText = "...really bored...";
            this.TSMI_System_ManualsInstruments.Click += new System.EventHandler(this.TSMI_System_ManualsInstruments_Click);
            // 
            // TSMI_System_SelfTests
            // 
            this.TSMI_System_SelfTests.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_System_DiagnosticsInstruments});
            this.TSMI_System_SelfTests.Name = "TSMI_System_SelfTests";
            this.TSMI_System_SelfTests.Size = new System.Drawing.Size(167, 26);
            this.TSMI_System_SelfTests.Text = "Self-&Tests";
            // 
            // TSMI_System_DiagnosticsInstruments
            // 
            this.TSMI_System_DiagnosticsInstruments.Name = "TSMI_System_DiagnosticsInstruments";
            this.TSMI_System_DiagnosticsInstruments.Size = new System.Drawing.Size(168, 26);
            this.TSMI_System_DiagnosticsInstruments.Text = "&Instruments";
            this.TSMI_System_DiagnosticsInstruments.ToolTipText = "Run TestPlan\'s instruments\' power-on self-tests.  Quicker but less comprehensive " +
    "than Diagnostics.";
            this.TSMI_System_DiagnosticsInstruments.Click += new System.EventHandler(this.TSMI_System_SelfTestsInstruments_Click);
            // 
            // TSMI_UUT
            // 
            this.TSMI_UUT.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_UUT_eDocs,
            this.TSMI_UUT_Manuals,
            this.TSMI_UUT_Statistics,
            this.TSMI_UUT_TestData});
            this.TSMI_UUT.Name = "TSMI_UUT";
            this.TSMI_UUT.Size = new System.Drawing.Size(51, 24);
            this.TSMI_UUT.Text = "&UUT";
            // 
            // TSMI_UUT_eDocs
            // 
            this.TSMI_UUT_eDocs.Name = "TSMI_UUT_eDocs";
            this.TSMI_UUT_eDocs.Size = new System.Drawing.Size(154, 26);
            this.TSMI_UUT_eDocs.Text = "&eDocs";
            this.TSMI_UUT_eDocs.ToolTipText = "UUT\'s P: drive eDocs folder.";
            this.TSMI_UUT_eDocs.Click += new System.EventHandler(this.TSMI_UUT_eDocs_Click);
            // 
            // TSMI_UUT_Manuals
            // 
            this.TSMI_UUT_Manuals.Name = "TSMI_UUT_Manuals";
            this.TSMI_UUT_Manuals.Size = new System.Drawing.Size(154, 26);
            this.TSMI_UUT_Manuals.Text = "&Manuals";
            this.TSMI_UUT_Manuals.Click += new System.EventHandler(this.TSMI_UUT_Manuals_Click);
            // 
            // TSMI_UUT_Statistics
            // 
            this.TSMI_UUT_Statistics.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_UUT_StatisticsDisplay,
            this.TSMI_UUT_StatisticsReset});
            this.TSMI_UUT_Statistics.Name = "TSMI_UUT_Statistics";
            this.TSMI_UUT_Statistics.Size = new System.Drawing.Size(154, 26);
            this.TSMI_UUT_Statistics.Text = "&Statistics";
            // 
            // TSMI_UUT_StatisticsDisplay
            // 
            this.TSMI_UUT_StatisticsDisplay.Name = "TSMI_UUT_StatisticsDisplay";
            this.TSMI_UUT_StatisticsDisplay.Size = new System.Drawing.Size(141, 26);
            this.TSMI_UUT_StatisticsDisplay.Text = "&Display";
            this.TSMI_UUT_StatisticsDisplay.ToolTipText = "How\'re we doing?";
            this.TSMI_UUT_StatisticsDisplay.Click += new System.EventHandler(this.TSMI_UUT_StatisticsDisplay_Click);
            // 
            // TSMI_UUT_StatisticsReset
            // 
            this.TSMI_UUT_StatisticsReset.Name = "TSMI_UUT_StatisticsReset";
            this.TSMI_UUT_StatisticsReset.Size = new System.Drawing.Size(141, 26);
            this.TSMI_UUT_StatisticsReset.Text = "&Reset";
            this.TSMI_UUT_StatisticsReset.ToolTipText = "Nothing like starting over...";
            this.TSMI_UUT_StatisticsReset.CheckStateChanged += new System.EventHandler(this.TSMI_UUT_StatisticsReset_Click);
            // 
            // TSMI_UUT_TestData
            // 
            this.TSMI_UUT_TestData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_UUT_TestDataP_DriveTDR_Folder,
            this.TSMI_UUT_TestDataSQL_ReportingAndQuerying});
            this.TSMI_UUT_TestData.Name = "TSMI_UUT_TestData";
            this.TSMI_UUT_TestData.Size = new System.Drawing.Size(154, 26);
            this.TSMI_UUT_TestData.Text = "&Test Data";
            // 
            // TSMI_UUT_TestDataP_DriveTDR_Folder
            // 
            this.TSMI_UUT_TestDataP_DriveTDR_Folder.Name = "TSMI_UUT_TestDataP_DriveTDR_Folder";
            this.TSMI_UUT_TestDataP_DriveTDR_Folder.Size = new System.Drawing.Size(268, 26);
            this.TSMI_UUT_TestDataP_DriveTDR_Folder.Text = "&P: Drive TDR Folder";
            this.TSMI_UUT_TestDataP_DriveTDR_Folder.ToolTipText = "P:\\Test\\TDR";
            this.TSMI_UUT_TestDataP_DriveTDR_Folder.Click += new System.EventHandler(this.TSMI_UUT_TestData_P_DriveTDR_Folder_Click);
            // 
            // TSMI_UUT_TestDataSQL_ReportingAndQuerying
            // 
            this.TSMI_UUT_TestDataSQL_ReportingAndQuerying.Enabled = false;
            this.TSMI_UUT_TestDataSQL_ReportingAndQuerying.Name = "TSMI_UUT_TestDataSQL_ReportingAndQuerying";
            this.TSMI_UUT_TestDataSQL_ReportingAndQuerying.Size = new System.Drawing.Size(268, 26);
            this.TSMI_UUT_TestDataSQL_ReportingAndQuerying.Text = "&SQL Reporting && Querying";
            this.TSMI_UUT_TestDataSQL_ReportingAndQuerying.ToolTipText = "Coming soon!";
            this.TSMI_UUT_TestDataSQL_ReportingAndQuerying.Click += new System.EventHandler(this.TSMI_UUT_TestDataSQL_ReportingAndQuerying_Click);
            // 
            // TSMI_About
            // 
            this.TSMI_About.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_About_TestExec,
            this.TSMI_About_TestPlan});
            this.TSMI_About.Name = "TSMI_About";
            this.TSMI_About.Size = new System.Drawing.Size(64, 24);
            this.TSMI_About.Text = "&About";
            // 
            // TSMI_About_TestExec
            // 
            this.TSMI_About_TestExec.Name = "TSMI_About_TestExec";
            this.TSMI_About_TestExec.Size = new System.Drawing.Size(148, 26);
            this.TSMI_About_TestExec.Text = "Test&Exec";
            this.TSMI_About_TestExec.ToolTipText = "TestExec\'s details.";
            this.TSMI_About_TestExec.Click += new System.EventHandler(this.TSMI_About_TestExec_Click);
            // 
            // TSMI_About_TestPlan
            // 
            this.TSMI_About_TestPlan.Name = "TSMI_About_TestPlan";
            this.TSMI_About_TestPlan.Size = new System.Drawing.Size(148, 26);
            this.TSMI_About_TestPlan.Text = "Test&Plan";
            this.TSMI_About_TestPlan.ToolTipText = "TestPlan\'s details.";
            this.TSMI_About_TestPlan.Click += new System.EventHandler(this.TSMI_About_TestPlan_Click);
            // 
            // StatusStrip
            // 
            this.StatusStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.StatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusTimeLabel,
            this.StatusStatisticsLabel,
            this.StatusModeLabel,
            this.StatusCustomLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 747);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.StatusStrip.Size = new System.Drawing.Size(1573, 26);
            this.StatusStrip.TabIndex = 10;
            // 
            // StatusTimeLabel
            // 
            this.StatusTimeLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.StatusTimeLabel.Name = "StatusTimeLabel";
            this.StatusTimeLabel.Size = new System.Drawing.Size(42, 20);
            this.StatusTimeLabel.Text = "Time";
            this.StatusTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusStatisticsLabel
            // 
            this.StatusStatisticsLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.StatusStatisticsLabel.Name = "StatusStatisticsLabel";
            this.StatusStatisticsLabel.Size = new System.Drawing.Size(67, 20);
            this.StatusStatisticsLabel.Text = "Statistics";
            this.StatusStatisticsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusModeLabel
            // 
            this.StatusModeLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.StatusModeLabel.Name = "StatusModeLabel";
            this.StatusModeLabel.Size = new System.Drawing.Size(48, 20);
            this.StatusModeLabel.Text = "Mode";
            this.StatusModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusCustomLabel
            // 
            this.StatusCustomLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.StatusCustomLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.StatusCustomLabel.Name = "StatusCustomLabel";
            this.StatusCustomLabel.Size = new System.Drawing.Size(1396, 20);
            this.StatusCustomLabel.Spring = true;
            this.StatusCustomLabel.Text = "Custom";
            this.StatusCustomLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TestExec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1573, 773);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.ButtonEmergencyStop);
            this.Controls.Add(this.ButtonSelect);
            this.Controls.Add(this.rtfResults);
            this.Controls.Add(this.LabelEvent);
            this.Controls.Add(this.TextTest);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonRun);
            this.Controls.Add(this.MS);
            this.MainMenuStrip = this.MS;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "TestExec";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestExec";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.MS.ResumeLayout(false);
            this.MS.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button ButtonRun;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.TextBox TextTest;
        private System.Windows.Forms.Label LabelEvent;
        private System.Windows.Forms.RichTextBox rtfResults;
        private System.Windows.Forms.Button ButtonSelect;
        private System.Windows.Forms.MenuStrip MS;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestPlan;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestPlan_SaveResults;
        private System.Windows.Forms.ToolStripMenuItem TSMI_System;
        private System.Windows.Forms.ToolStripMenuItem TSMI_System_SelfTests;
        private System.Windows.Forms.ToolStripMenuItem TSMI_System_DiagnosticsInstruments;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT_eDocs;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT_TestData;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT_TestDataP_DriveTDR_Folder;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT_TestDataSQL_ReportingAndQuerying;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT_Manuals;
        private System.Windows.Forms.ToolStripMenuItem TSMI_System_Manuals;
        private System.Windows.Forms.ToolStripMenuItem TSMI_System_ManualsBarcodeScanner;
        private System.Windows.Forms.ToolStripMenuItem TSMI_System_ManualsInstruments;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Feedback;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Feedback_Compliments;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Feedback_ComplimentsPraiseεPlaudits;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Feedback_ComplimentsMoney;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Feedback_Critiques;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Feedback_CritiquesBugReport;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Feedback_CritiquesImprovementRequest;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusStatisticsLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusCustomLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusTimeLabel;
        private System.Windows.Forms.ToolStripStatusLabel StatusModeLabel;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestPlan_Exit;
        private System.Windows.Forms.ToolStripSeparator TSMI_File_Separator;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT_Statistics;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT_StatisticsDisplay;
        private System.Windows.Forms.ToolStripMenuItem TSMI_UUT_StatisticsReset;
        private System.Windows.Forms.ToolStripMenuItem TSMI_System_ColorCode;
        private System.Windows.Forms.ToolStripMenuItem TSMI_About;
        private System.Windows.Forms.ToolStripMenuItem TSMI_About_TestExec;
        private System.Windows.Forms.ToolStripMenuItem TSMI_About_TestPlan;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TestPlan_Choose;
        private System.Windows.Forms.Button ButtonEmergencyStop;
    }
}
