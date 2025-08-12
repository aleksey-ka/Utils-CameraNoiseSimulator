namespace NoiseSimulator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // UI Control declarations
        private PictureBox pictureBox;
        private Button generateButton;
        private TrackBar backgroundTrackBar;
        private TrackBar signalTrackBar;
        private TrackBar exposureTrackBar;
        private TrackBar readNoiseTrackBar;
        private TrackBar numberOfExposuresTrackBar;
        private Label backgroundLabel;
        private Label signalLabel;
        private Label exposureLabel;
        private Label readNoiseLabel;
        private Label numberOfExposuresLabel;
        private ComboBox patternComboBox;
        private Label patternLabel;
        private RichTextBox statisticsTextBox;
        private NumericUpDown minValueNumeric;
        private NumericUpDown maxValueNumeric;
        private Label minValueLabel;
        private Label maxValueLabel;
        private Button resetRangeButton;
        private NumericUpDown signalSquareNumeric;
        private Label signalSquareLabel;
        private NumericUpDown squareSizeNumeric;
        private Label squareSizeLabel;
        private CheckBox verticalLinesCheckBox;
        private Button saveButton;
        private Label instructionLabel;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.generateButton = new System.Windows.Forms.Button();
            this.backgroundTrackBar = new System.Windows.Forms.TrackBar();
            this.signalTrackBar = new System.Windows.Forms.TrackBar();
            this.exposureTrackBar = new System.Windows.Forms.TrackBar();
            this.readNoiseTrackBar = new System.Windows.Forms.TrackBar();
            this.numberOfExposuresTrackBar = new System.Windows.Forms.TrackBar();
            this.backgroundLabel = new System.Windows.Forms.Label();
            this.signalLabel = new System.Windows.Forms.Label();
            this.exposureLabel = new System.Windows.Forms.Label();
            this.readNoiseLabel = new System.Windows.Forms.Label();
            this.numberOfExposuresLabel = new System.Windows.Forms.Label();
            this.patternComboBox = new System.Windows.Forms.ComboBox();
            this.patternLabel = new System.Windows.Forms.Label();
            this.statisticsTextBox = new System.Windows.Forms.RichTextBox();
            this.minValueNumeric = new System.Windows.Forms.NumericUpDown();
            this.maxValueNumeric = new System.Windows.Forms.NumericUpDown();
            this.minValueLabel = new System.Windows.Forms.Label();
            this.maxValueLabel = new System.Windows.Forms.Label();
            this.resetRangeButton = new System.Windows.Forms.Button();
            this.signalSquareNumeric = new System.Windows.Forms.NumericUpDown();
            this.signalSquareLabel = new System.Windows.Forms.Label();
            this.squareSizeNumeric = new System.Windows.Forms.NumericUpDown();
            this.squareSizeLabel = new System.Windows.Forms.Label();
            this.verticalLinesCheckBox = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.instructionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.signalTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exposureTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.readNoiseTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfExposuresTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minValueNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxValueNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.signalSquareNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.squareSizeNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(5, 330);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1024, 1024);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.pictureBox.TabIndex = 0;
            // 
            // generateButton
            // 
            this.generateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.generateButton.FlatAppearance.BorderColor = System.Drawing.Color.LightBlue;
            this.generateButton.FlatAppearance.BorderSize = 1;
            this.generateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.generateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.generateButton.ForeColor = System.Drawing.Color.White;
            this.generateButton.Location = new System.Drawing.Point(10, 10);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(150, 30);
            this.generateButton.TabIndex = 1;
            this.generateButton.Text = "Generate New";
            this.generateButton.UseVisualStyleBackColor = false;
            this.generateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // backgroundTrackBar
            // 
            this.backgroundTrackBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.backgroundTrackBar.Location = new System.Drawing.Point(120, 45);
            this.backgroundTrackBar.Maximum = 1000;
            this.backgroundTrackBar.Name = "backgroundTrackBar";
            this.backgroundTrackBar.Size = new System.Drawing.Size(1000, 45);
            this.backgroundTrackBar.TabIndex = 2;
            this.backgroundTrackBar.TickFrequency = 100;
            this.backgroundTrackBar.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.backgroundTrackBar.Value = 10;
            this.backgroundTrackBar.ValueChanged += new System.EventHandler(this.BackgroundTrackBar_ValueChanged);
            // 
            // signalTrackBar
            // 
            this.signalTrackBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.signalTrackBar.Location = new System.Drawing.Point(120, 95);
            this.signalTrackBar.Maximum = 1000;
            this.signalTrackBar.Name = "signalTrackBar";
            this.signalTrackBar.Size = new System.Drawing.Size(1000, 45);
            this.signalTrackBar.TabIndex = 3;
            this.signalTrackBar.TickFrequency = 100;
            this.signalTrackBar.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.signalTrackBar.Value = 100;
            this.signalTrackBar.ValueChanged += new System.EventHandler(this.SignalTrackBar_ValueChanged);
            // 
            // exposureTrackBar
            // 
            this.exposureTrackBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.exposureTrackBar.Location = new System.Drawing.Point(120, 145);
            this.exposureTrackBar.Maximum = 1000;
            this.exposureTrackBar.Name = "exposureTrackBar";
            this.exposureTrackBar.Size = new System.Drawing.Size(1000, 45);
            this.exposureTrackBar.TabIndex = 4;
            this.exposureTrackBar.TickFrequency = 100;
            this.exposureTrackBar.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.exposureTrackBar.Value = 10;
            this.exposureTrackBar.ValueChanged += new System.EventHandler(this.ExposureTrackBar_ValueChanged);
            // 
            // readNoiseTrackBar
            // 
            this.readNoiseTrackBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.readNoiseTrackBar.Location = new System.Drawing.Point(120, 195);
            this.readNoiseTrackBar.Maximum = 1000;
            this.readNoiseTrackBar.Name = "readNoiseTrackBar";
            this.readNoiseTrackBar.Size = new System.Drawing.Size(1000, 45);
            this.readNoiseTrackBar.TabIndex = 5;
            this.readNoiseTrackBar.TickFrequency = 100;
            this.readNoiseTrackBar.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.readNoiseTrackBar.Value = 10;
            this.readNoiseTrackBar.ValueChanged += new System.EventHandler(this.ReadNoiseTrackBar_ValueChanged);
            // 
            // numberOfExposuresTrackBar
            // 
            this.numberOfExposuresTrackBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.numberOfExposuresTrackBar.Location = new System.Drawing.Point(120, 245);
            this.numberOfExposuresTrackBar.Maximum = 256;
            this.numberOfExposuresTrackBar.Minimum = 1;
            this.numberOfExposuresTrackBar.Name = "numberOfExposuresTrackBar";
            this.numberOfExposuresTrackBar.Size = new System.Drawing.Size(1000, 45);
            this.numberOfExposuresTrackBar.TabIndex = 6;
            this.numberOfExposuresTrackBar.TickFrequency = 25;
            this.numberOfExposuresTrackBar.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.numberOfExposuresTrackBar.Value = 1;
            this.numberOfExposuresTrackBar.ValueChanged += new System.EventHandler(this.NumberOfExposuresTrackBar_ValueChanged);
            // 
            // backgroundLabel
            // 
            this.backgroundLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.backgroundLabel.ForeColor = System.Drawing.Color.White;
            this.backgroundLabel.Location = new System.Drawing.Point(10, 50);
            this.backgroundLabel.Name = "backgroundLabel";
            this.backgroundLabel.Size = new System.Drawing.Size(110, 20);
            this.backgroundLabel.TabIndex = 7;
            this.backgroundLabel.Text = "Background:";
            this.backgroundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // signalLabel
            // 
            this.signalLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.signalLabel.ForeColor = System.Drawing.Color.White;
            this.signalLabel.Location = new System.Drawing.Point(10, 100);
            this.signalLabel.Name = "signalLabel";
            this.signalLabel.Size = new System.Drawing.Size(110, 20);
            this.signalLabel.TabIndex = 8;
            this.signalLabel.Text = "Signal:";
            this.signalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // exposureLabel
            // 
            this.exposureLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.exposureLabel.ForeColor = System.Drawing.Color.White;
            this.exposureLabel.Location = new System.Drawing.Point(10, 150);
            this.exposureLabel.Name = "exposureLabel";
            this.exposureLabel.Size = new System.Drawing.Size(110, 20);
            this.exposureLabel.TabIndex = 9;
            this.exposureLabel.Text = "Exposure:";
            this.exposureLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // readNoiseLabel
            // 
            this.readNoiseLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.readNoiseLabel.ForeColor = System.Drawing.Color.White;
            this.readNoiseLabel.Location = new System.Drawing.Point(10, 200);
            this.readNoiseLabel.Name = "readNoiseLabel";
            this.readNoiseLabel.Size = new System.Drawing.Size(110, 20);
            this.readNoiseLabel.TabIndex = 10;
            this.readNoiseLabel.Text = "Read Noise:";
            this.readNoiseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numberOfExposuresLabel
            // 
            this.numberOfExposuresLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.numberOfExposuresLabel.ForeColor = System.Drawing.Color.White;
            this.numberOfExposuresLabel.Location = new System.Drawing.Point(10, 250);
            this.numberOfExposuresLabel.Name = "numberOfExposuresLabel";
            this.numberOfExposuresLabel.Size = new System.Drawing.Size(110, 20);
            this.numberOfExposuresLabel.TabIndex = 11;
            this.numberOfExposuresLabel.Text = "Exposures:";
            this.numberOfExposuresLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // patternComboBox
            // 
            this.patternComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.patternComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.patternComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.patternComboBox.ForeColor = System.Drawing.Color.White;
            this.patternComboBox.Location = new System.Drawing.Point(120, 295);
            this.patternComboBox.Name = "patternComboBox";
            this.patternComboBox.Size = new System.Drawing.Size(200, 25);
            this.patternComboBox.TabIndex = 12;
            this.patternComboBox.SelectedIndexChanged += new System.EventHandler(this.PatternComboBox_SelectedIndexChanged);
            // 
            // patternLabel
            // 
            this.patternLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.patternLabel.ForeColor = System.Drawing.Color.White;
            this.patternLabel.Location = new System.Drawing.Point(10, 300);
            this.patternLabel.Name = "patternLabel";
            this.patternLabel.Size = new System.Drawing.Size(100, 20);
            this.patternLabel.TabIndex = 13;
            this.patternLabel.Text = "Signal Pattern:";
            // 
            // statisticsTextBox
            // 
            this.statisticsTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.statisticsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statisticsTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.statisticsTextBox.ForeColor = System.Drawing.Color.White;
            this.statisticsTextBox.Location = new System.Drawing.Point(1040, 500);
            this.statisticsTextBox.Name = "statisticsTextBox";
            this.statisticsTextBox.ReadOnly = true;
            this.statisticsTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.statisticsTextBox.Size = new System.Drawing.Size(300, 400);
            this.statisticsTextBox.TabIndex = 14;
            this.statisticsTextBox.Text = "";
            // 
            // minValueNumeric
            // 
            this.minValueNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.minValueNumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.minValueNumeric.ForeColor = System.Drawing.Color.White;
            this.minValueNumeric.Location = new System.Drawing.Point(1140, 330);
            this.minValueNumeric.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.minValueNumeric.Name = "minValueNumeric";
            this.minValueNumeric.Size = new System.Drawing.Size(80, 25);
            this.minValueNumeric.TabIndex = 15;
            this.minValueNumeric.ValueChanged += new System.EventHandler(this.MinValueNumeric_ValueChanged);
            // 
            // maxValueNumeric
            // 
            this.maxValueNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.maxValueNumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxValueNumeric.ForeColor = System.Drawing.Color.White;
            this.maxValueNumeric.Location = new System.Drawing.Point(1140, 360);
            this.maxValueNumeric.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.maxValueNumeric.Name = "maxValueNumeric";
            this.maxValueNumeric.Size = new System.Drawing.Size(80, 25);
            this.maxValueNumeric.TabIndex = 16;
            this.maxValueNumeric.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.maxValueNumeric.ValueChanged += new System.EventHandler(this.MaxValueNumeric_ValueChanged);
            // 
            // minValueLabel
            // 
            this.minValueLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.minValueLabel.ForeColor = System.Drawing.Color.White;
            this.minValueLabel.Location = new System.Drawing.Point(1040, 330);
            this.minValueLabel.Name = "minValueLabel";
            this.minValueLabel.Size = new System.Drawing.Size(100, 20);
            this.minValueLabel.TabIndex = 17;
            this.minValueLabel.Text = "Min:";
            this.minValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maxValueLabel
            // 
            this.maxValueLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.maxValueLabel.ForeColor = System.Drawing.Color.White;
            this.maxValueLabel.Location = new System.Drawing.Point(1040, 360);
            this.maxValueLabel.Name = "maxValueLabel";
            this.maxValueLabel.Size = new System.Drawing.Size(100, 20);
            this.maxValueLabel.TabIndex = 18;
            this.maxValueLabel.Text = "Max:";
            this.maxValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // resetRangeButton
            // 
            this.resetRangeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.resetRangeButton.FlatAppearance.BorderSize = 0;
            this.resetRangeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetRangeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.resetRangeButton.ForeColor = System.Drawing.Color.White;
            this.resetRangeButton.Location = new System.Drawing.Point(1040, 390);
            this.resetRangeButton.Name = "resetRangeButton";
            this.resetRangeButton.Size = new System.Drawing.Size(140, 30);
            this.resetRangeButton.TabIndex = 19;
            this.resetRangeButton.Text = "Reset Display Range";
            this.resetRangeButton.UseVisualStyleBackColor = false;
            this.resetRangeButton.Click += new System.EventHandler(this.ResetRangeButton_Click);
            // 
            // signalSquareNumeric
            // 
            this.signalSquareNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.signalSquareNumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.signalSquareNumeric.ForeColor = System.Drawing.Color.White;
            this.signalSquareNumeric.Location = new System.Drawing.Point(1140, 430);
            this.signalSquareNumeric.Maximum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.signalSquareNumeric.Name = "signalSquareNumeric";
            this.signalSquareNumeric.Size = new System.Drawing.Size(80, 25);
            this.signalSquareNumeric.TabIndex = 20;
            this.signalSquareNumeric.ValueChanged += new System.EventHandler(this.SignalSquareNumeric_ValueChanged);
            // 
            // signalSquareLabel
            // 
            this.signalSquareLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.signalSquareLabel.ForeColor = System.Drawing.Color.White;
            this.signalSquareLabel.Location = new System.Drawing.Point(1040, 430);
            this.signalSquareLabel.Name = "signalSquareLabel";
            this.signalSquareLabel.Size = new System.Drawing.Size(100, 20);
            this.signalSquareLabel.TabIndex = 21;
            this.signalSquareLabel.Text = "Selected Square:";
            this.signalSquareLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // squareSizeNumeric
            // 
            this.squareSizeNumeric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.squareSizeNumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.squareSizeNumeric.ForeColor = System.Drawing.Color.White;
            this.squareSizeNumeric.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.squareSizeNumeric.Location = new System.Drawing.Point(410, 295);
            this.squareSizeNumeric.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.squareSizeNumeric.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.squareSizeNumeric.Name = "squareSizeNumeric";
            this.squareSizeNumeric.Size = new System.Drawing.Size(80, 25);
            this.squareSizeNumeric.TabIndex = 22;
            this.squareSizeNumeric.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.squareSizeNumeric.ValueChanged += new System.EventHandler(this.SquareSizeNumeric_ValueChanged);
            // 
            // squareSizeLabel
            // 
            this.squareSizeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.squareSizeLabel.ForeColor = System.Drawing.Color.White;
            this.squareSizeLabel.Location = new System.Drawing.Point(330, 300);
            this.squareSizeLabel.Name = "squareSizeLabel";
            this.squareSizeLabel.Size = new System.Drawing.Size(80, 20);
            this.squareSizeLabel.TabIndex = 23;
            this.squareSizeLabel.Text = "Square Size:";
            // 
            // verticalLinesCheckBox
            // 
            this.verticalLinesCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.verticalLinesCheckBox.Checked = true;
            this.verticalLinesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.verticalLinesCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.verticalLinesCheckBox.ForeColor = System.Drawing.Color.White;
            this.verticalLinesCheckBox.Location = new System.Drawing.Point(500, 295);
            this.verticalLinesCheckBox.Name = "verticalLinesCheckBox";
            this.verticalLinesCheckBox.Size = new System.Drawing.Size(120, 25);
            this.verticalLinesCheckBox.TabIndex = 24;
            this.verticalLinesCheckBox.Text = "Vertical Lines";
            this.verticalLinesCheckBox.UseVisualStyleBackColor = false;
            this.verticalLinesCheckBox.CheckedChanged += new System.EventHandler(this.VerticalLinesCheckBox_CheckedChanged);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.saveButton.FlatAppearance.BorderSize = 0;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.ForeColor = System.Drawing.Color.White;
            this.saveButton.Location = new System.Drawing.Point(1150, 10);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(190, 30);
            this.saveButton.TabIndex = 25;
            this.saveButton.Text = "Save FITS";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // instructionLabel
            // 
            this.instructionLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.instructionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.instructionLabel.ForeColor = System.Drawing.Color.LightBlue;
            this.instructionLabel.Location = new System.Drawing.Point(170, 15);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new System.Drawing.Size(800, 20);
            this.instructionLabel.TabIndex = 26;
            this.instructionLabel.Text = "Configure signal and background parameters below:";
            // 
            // MainForm
            // 
            this.AcceptButton = this.generateButton;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(1370, 1400);
            this.Controls.Add(this.instructionLabel);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.verticalLinesCheckBox);
            this.Controls.Add(this.squareSizeLabel);
            this.Controls.Add(this.squareSizeNumeric);
            this.Controls.Add(this.signalSquareLabel);
            this.Controls.Add(this.signalSquareNumeric);
            this.Controls.Add(this.resetRangeButton);
            this.Controls.Add(this.maxValueLabel);
            this.Controls.Add(this.minValueLabel);
            this.Controls.Add(this.maxValueNumeric);
            this.Controls.Add(this.minValueNumeric);
            this.Controls.Add(this.statisticsTextBox);
            this.Controls.Add(this.patternLabel);
            this.Controls.Add(this.patternComboBox);
            this.Controls.Add(this.numberOfExposuresLabel);
            this.Controls.Add(this.numberOfExposuresTrackBar);
            this.Controls.Add(this.readNoiseLabel);
            this.Controls.Add(this.readNoiseTrackBar);
            this.Controls.Add(this.exposureLabel);
            this.Controls.Add(this.exposureTrackBar);
            this.Controls.Add(this.signalLabel);
            this.Controls.Add(this.signalTrackBar);
            this.Controls.Add(this.backgroundLabel);
            this.Controls.Add(this.backgroundTrackBar);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.generateButton);
            this.ForeColor = System.Drawing.Color.White;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Noise Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.signalTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exposureTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.readNoiseTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfExposuresTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minValueNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxValueNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.signalSquareNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.squareSizeNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
