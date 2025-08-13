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
        private PictureBox graphPictureBox;
        private CheckBox logarithmicCheckBox;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox = new PictureBox();
            generateButton = new Button();
            backgroundTrackBar = new TrackBar();
            signalTrackBar = new TrackBar();
            exposureTrackBar = new TrackBar();
            readNoiseTrackBar = new TrackBar();
            numberOfExposuresTrackBar = new TrackBar();
            backgroundLabel = new Label();
            signalLabel = new Label();
            exposureLabel = new Label();
            readNoiseLabel = new Label();
            numberOfExposuresLabel = new Label();
            patternComboBox = new ComboBox();
            patternLabel = new Label();
            statisticsTextBox = new RichTextBox();
            minValueNumeric = new NumericUpDown();
            maxValueNumeric = new NumericUpDown();
            minValueLabel = new Label();
            maxValueLabel = new Label();
            resetRangeButton = new Button();
            signalSquareNumeric = new NumericUpDown();
            signalSquareLabel = new Label();
            squareSizeNumeric = new NumericUpDown();
            squareSizeLabel = new Label();
            verticalLinesCheckBox = new CheckBox();
            saveButton = new Button();
            instructionLabel = new Label();
            graphPictureBox = new PictureBox();
            logarithmicCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)backgroundTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)signalTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)exposureTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)readNoiseTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numberOfExposuresTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)minValueNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)maxValueNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)signalSquareNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)squareSizeNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)graphPictureBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.BackColor = Color.FromArgb(48, 48, 48);
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.Location = new Point(7, 303);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(1024, 1024);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // generateButton
            // 
            generateButton.BackColor = Color.FromArgb(80, 80, 80);
            generateButton.FlatAppearance.BorderColor = Color.LightBlue;
            generateButton.FlatStyle = FlatStyle.Flat;
            generateButton.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            generateButton.ForeColor = Color.White;
            generateButton.Location = new Point(7, 7);
            generateButton.Name = "generateButton";
            generateButton.Size = new Size(150, 30);
            generateButton.TabIndex = 1;
            generateButton.Text = "Generate New";
            generateButton.UseVisualStyleBackColor = false;
            generateButton.Click += GenerateButton_Click;
            // 
            // backgroundTrackBar
            // 
            backgroundTrackBar.BackColor = Color.FromArgb(32, 32, 32);
            backgroundTrackBar.Location = new Point(120, 45);
            backgroundTrackBar.Maximum = 1000;
            backgroundTrackBar.Name = "backgroundTrackBar";
            backgroundTrackBar.Size = new Size(914, 45);
            backgroundTrackBar.TabIndex = 2;
            backgroundTrackBar.TickFrequency = 100;
            backgroundTrackBar.Value = 10;
            backgroundTrackBar.ValueChanged += BackgroundTrackBar_ValueChanged;
            // 
            // signalTrackBar
            // 
            signalTrackBar.BackColor = Color.FromArgb(32, 32, 32);
            signalTrackBar.Location = new Point(120, 90);
            signalTrackBar.Maximum = 1000;
            signalTrackBar.Name = "signalTrackBar";
            signalTrackBar.Size = new Size(914, 45);
            signalTrackBar.TabIndex = 3;
            signalTrackBar.TickFrequency = 100;
            signalTrackBar.Value = 100;
            signalTrackBar.ValueChanged += SignalTrackBar_ValueChanged;
            // 
            // exposureTrackBar
            // 
            exposureTrackBar.BackColor = Color.FromArgb(32, 32, 32);
            exposureTrackBar.Location = new Point(120, 135);
            exposureTrackBar.Maximum = 1000;
            exposureTrackBar.Name = "exposureTrackBar";
            exposureTrackBar.Size = new Size(914, 45);
            exposureTrackBar.TabIndex = 4;
            exposureTrackBar.TickFrequency = 100;
            exposureTrackBar.Value = 10;
            exposureTrackBar.ValueChanged += ExposureTrackBar_ValueChanged;
            // 
            // readNoiseTrackBar
            // 
            readNoiseTrackBar.BackColor = Color.FromArgb(32, 32, 32);
            readNoiseTrackBar.Location = new Point(120, 180);
            readNoiseTrackBar.Maximum = 1000;
            readNoiseTrackBar.Name = "readNoiseTrackBar";
            readNoiseTrackBar.Size = new Size(914, 45);
            readNoiseTrackBar.TabIndex = 5;
            readNoiseTrackBar.TickFrequency = 100;
            readNoiseTrackBar.Value = 10;
            readNoiseTrackBar.ValueChanged += ReadNoiseTrackBar_ValueChanged;
            // 
            // numberOfExposuresTrackBar
            // 
            numberOfExposuresTrackBar.BackColor = Color.FromArgb(32, 32, 32);
            numberOfExposuresTrackBar.Location = new Point(120, 225);
            numberOfExposuresTrackBar.Maximum = 256;
            numberOfExposuresTrackBar.Minimum = 1;
            numberOfExposuresTrackBar.Name = "numberOfExposuresTrackBar";
            numberOfExposuresTrackBar.Size = new Size(914, 45);
            numberOfExposuresTrackBar.TabIndex = 6;
            numberOfExposuresTrackBar.TickFrequency = 25;
            numberOfExposuresTrackBar.Value = 1;
            numberOfExposuresTrackBar.ValueChanged += NumberOfExposuresTrackBar_ValueChanged;
            // 
            // backgroundLabel
            // 
            backgroundLabel.BackColor = Color.FromArgb(32, 32, 32);
            backgroundLabel.ForeColor = Color.White;
            backgroundLabel.Location = new Point(10, 45);
            backgroundLabel.Name = "backgroundLabel";
            backgroundLabel.Size = new Size(101, 20);
            backgroundLabel.TabIndex = 7;
            backgroundLabel.Text = "Background:";
            backgroundLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // signalLabel
            // 
            signalLabel.BackColor = Color.FromArgb(32, 32, 32);
            signalLabel.ForeColor = Color.White;
            signalLabel.Location = new Point(10, 90);
            signalLabel.Name = "signalLabel";
            signalLabel.Size = new Size(101, 20);
            signalLabel.TabIndex = 8;
            signalLabel.Text = "Signal:";
            signalLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // exposureLabel
            // 
            exposureLabel.BackColor = Color.FromArgb(32, 32, 32);
            exposureLabel.ForeColor = Color.White;
            exposureLabel.Location = new Point(10, 134);
            exposureLabel.Name = "exposureLabel";
            exposureLabel.Size = new Size(101, 20);
            exposureLabel.TabIndex = 9;
            exposureLabel.Text = "Exposure:";
            exposureLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // readNoiseLabel
            // 
            readNoiseLabel.BackColor = Color.FromArgb(32, 32, 32);
            readNoiseLabel.ForeColor = Color.White;
            readNoiseLabel.Location = new Point(10, 178);
            readNoiseLabel.Name = "readNoiseLabel";
            readNoiseLabel.Size = new Size(101, 20);
            readNoiseLabel.TabIndex = 10;
            readNoiseLabel.Text = "Read Noise:";
            readNoiseLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // numberOfExposuresLabel
            // 
            numberOfExposuresLabel.BackColor = Color.FromArgb(32, 32, 32);
            numberOfExposuresLabel.ForeColor = Color.White;
            numberOfExposuresLabel.Location = new Point(10, 222);
            numberOfExposuresLabel.Name = "numberOfExposuresLabel";
            numberOfExposuresLabel.Size = new Size(101, 20);
            numberOfExposuresLabel.TabIndex = 11;
            numberOfExposuresLabel.Text = "Exposures:";
            numberOfExposuresLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // patternComboBox
            // 
            patternComboBox.BackColor = Color.FromArgb(48, 48, 48);
            patternComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            patternComboBox.FlatStyle = FlatStyle.Flat;
            patternComboBox.ForeColor = Color.White;
            patternComboBox.Location = new Point(98, 274);
            patternComboBox.Name = "patternComboBox";
            patternComboBox.Size = new Size(150, 23);
            patternComboBox.TabIndex = 12;
            patternComboBox.SelectedIndexChanged += PatternComboBox_SelectedIndexChanged;
            // 
            // patternLabel
            // 
            patternLabel.BackColor = Color.FromArgb(32, 32, 32);
            patternLabel.ForeColor = Color.White;
            patternLabel.Location = new Point(10, 276);
            patternLabel.Name = "patternLabel";
            patternLabel.Size = new Size(85, 20);
            patternLabel.TabIndex = 13;
            patternLabel.Text = "Signal Pattern:";
            // 
            // statisticsTextBox
            // 
            statisticsTextBox.BackColor = Color.FromArgb(48, 48, 48);
            statisticsTextBox.BorderStyle = BorderStyle.None;
            statisticsTextBox.Font = new Font("Consolas", 9F);
            statisticsTextBox.ForeColor = Color.White;
            statisticsTextBox.Location = new Point(1037, 306);
            statisticsTextBox.Name = "statisticsTextBox";
            statisticsTextBox.ReadOnly = true;
            statisticsTextBox.ScrollBars = RichTextBoxScrollBars.None;
            statisticsTextBox.Size = new Size(490, 551);
            statisticsTextBox.TabIndex = 14;
            statisticsTextBox.Text = "";
            // 
            // minValueNumeric
            // 
            minValueNumeric.BackColor = Color.FromArgb(48, 48, 48);
            minValueNumeric.BorderStyle = BorderStyle.FixedSingle;
            minValueNumeric.ForeColor = Color.White;
            minValueNumeric.Location = new Point(813, 274);
            minValueNumeric.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            minValueNumeric.Name = "minValueNumeric";
            minValueNumeric.Size = new Size(80, 23);
            minValueNumeric.TabIndex = 15;
            minValueNumeric.ValueChanged += MinValueNumeric_ValueChanged;
            // 
            // maxValueNumeric
            // 
            maxValueNumeric.BackColor = Color.FromArgb(48, 48, 48);
            maxValueNumeric.BorderStyle = BorderStyle.FixedSingle;
            maxValueNumeric.ForeColor = Color.White;
            maxValueNumeric.Location = new Point(948, 274);
            maxValueNumeric.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            maxValueNumeric.Name = "maxValueNumeric";
            maxValueNumeric.Size = new Size(80, 23);
            maxValueNumeric.TabIndex = 16;
            maxValueNumeric.Value = new decimal(new int[] { 255, 0, 0, 0 });
            maxValueNumeric.ValueChanged += MaxValueNumeric_ValueChanged;
            // 
            // minValueLabel
            // 
            minValueLabel.BackColor = Color.FromArgb(32, 32, 32);
            minValueLabel.ForeColor = Color.White;
            minValueLabel.Location = new Point(770, 276);
            minValueLabel.Name = "minValueLabel";
            minValueLabel.Size = new Size(36, 20);
            minValueLabel.TabIndex = 17;
            minValueLabel.Text = "Min:";
            minValueLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // maxValueLabel
            // 
            maxValueLabel.BackColor = Color.FromArgb(32, 32, 32);
            maxValueLabel.ForeColor = Color.White;
            maxValueLabel.Location = new Point(905, 276);
            maxValueLabel.Name = "maxValueLabel";
            maxValueLabel.Size = new Size(36, 20);
            maxValueLabel.TabIndex = 18;
            maxValueLabel.Text = "Max:";
            maxValueLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // resetRangeButton
            // 
            resetRangeButton.BackColor = Color.FromArgb(64, 64, 64);
            resetRangeButton.FlatAppearance.BorderSize = 0;
            resetRangeButton.FlatStyle = FlatStyle.Flat;
            resetRangeButton.Font = new Font("Microsoft Sans Serif", 8F);
            resetRangeButton.ForeColor = Color.White;
            resetRangeButton.Location = new Point(1040, 270);
            resetRangeButton.Name = "resetRangeButton";
            resetRangeButton.Size = new Size(130, 30);
            resetRangeButton.TabIndex = 19;
            resetRangeButton.Text = "Reset Display Range";
            resetRangeButton.UseVisualStyleBackColor = false;
            resetRangeButton.Click += ResetRangeButton_Click;
            // 
            // signalSquareNumeric
            // 
            signalSquareNumeric.BackColor = Color.FromArgb(48, 48, 48);
            signalSquareNumeric.BorderStyle = BorderStyle.FixedSingle;
            signalSquareNumeric.ForeColor = Color.White;
            signalSquareNumeric.Location = new Point(1280, 276);
            signalSquareNumeric.Maximum = new decimal(new int[] { 80, 0, 0, 0 });
            signalSquareNumeric.Name = "signalSquareNumeric";
            signalSquareNumeric.Size = new Size(56, 23);
            signalSquareNumeric.TabIndex = 20;
            signalSquareNumeric.ValueChanged += SignalSquareNumeric_ValueChanged;
            // 
            // signalSquareLabel
            // 
            signalSquareLabel.BackColor = Color.FromArgb(32, 32, 32);
            signalSquareLabel.ForeColor = Color.White;
            signalSquareLabel.Location = new Point(1185, 276);
            signalSquareLabel.Name = "signalSquareLabel";
            signalSquareLabel.Size = new Size(93, 20);
            signalSquareLabel.TabIndex = 21;
            signalSquareLabel.Text = "Selected Square:";
            signalSquareLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // squareSizeNumeric
            // 
            squareSizeNumeric.BackColor = Color.FromArgb(48, 48, 48);
            squareSizeNumeric.BorderStyle = BorderStyle.FixedSingle;
            squareSizeNumeric.ForeColor = Color.White;
            squareSizeNumeric.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            squareSizeNumeric.Location = new Point(423, 274);
            squareSizeNumeric.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            squareSizeNumeric.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            squareSizeNumeric.Name = "squareSizeNumeric";
            squareSizeNumeric.Size = new Size(80, 23);
            squareSizeNumeric.TabIndex = 22;
            squareSizeNumeric.Value = new decimal(new int[] { 60, 0, 0, 0 });
            squareSizeNumeric.ValueChanged += SquareSizeNumeric_ValueChanged;
            // 
            // squareSizeLabel
            // 
            squareSizeLabel.BackColor = Color.FromArgb(32, 32, 32);
            squareSizeLabel.ForeColor = Color.White;
            squareSizeLabel.Location = new Point(343, 276);
            squareSizeLabel.Name = "squareSizeLabel";
            squareSizeLabel.Size = new Size(80, 20);
            squareSizeLabel.TabIndex = 23;
            squareSizeLabel.Text = "Square Size:";
            // 
            // verticalLinesCheckBox
            // 
            verticalLinesCheckBox.BackColor = Color.FromArgb(32, 32, 32);
            verticalLinesCheckBox.Checked = true;
            verticalLinesCheckBox.CheckState = CheckState.Checked;
            verticalLinesCheckBox.FlatStyle = FlatStyle.Flat;
            verticalLinesCheckBox.ForeColor = Color.White;
            verticalLinesCheckBox.Location = new Point(513, 271);
            verticalLinesCheckBox.Name = "verticalLinesCheckBox";
            verticalLinesCheckBox.Size = new Size(120, 25);
            verticalLinesCheckBox.TabIndex = 24;
            verticalLinesCheckBox.Text = "Vertical Lines";
            verticalLinesCheckBox.UseVisualStyleBackColor = false;
            verticalLinesCheckBox.CheckedChanged += VerticalLinesCheckBox_CheckedChanged;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            saveButton.BackColor = Color.FromArgb(64, 64, 64);
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.ForeColor = Color.White;
            saveButton.Location = new Point(1405, 8);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(122, 30);
            saveButton.TabIndex = 25;
            saveButton.Text = "Save FITS";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += SaveButton_Click;
            // 
            // instructionLabel
            // 
            instructionLabel.BackColor = Color.FromArgb(32, 32, 32);
            instructionLabel.Font = new Font("Microsoft Sans Serif", 10F);
            instructionLabel.ForeColor = Color.LightBlue;
            instructionLabel.Location = new Point(170, 15);
            instructionLabel.Name = "instructionLabel";
            instructionLabel.Size = new Size(800, 20);
            instructionLabel.TabIndex = 26;
            instructionLabel.Text = "Configure signal and background parameters below:";
            // 
            // graphPictureBox
            // 
            graphPictureBox.BackColor = Color.FromArgb(48, 48, 48);
            graphPictureBox.Location = new Point(1037, 863);
            graphPictureBox.Name = "graphPictureBox";
            graphPictureBox.Size = new Size(490, 442);
            graphPictureBox.TabIndex = 27;
            graphPictureBox.TabStop = false;
            // 
            // logarithmicCheckBox
            // 
            logarithmicCheckBox.BackColor = Color.FromArgb(32, 32, 32);
            logarithmicCheckBox.ForeColor = Color.White;
            logarithmicCheckBox.Location = new Point(1037, 1315);
            logarithmicCheckBox.Name = "logarithmicCheckBox";
            logarithmicCheckBox.Size = new Size(150, 20);
            logarithmicCheckBox.TabIndex = 28;
            logarithmicCheckBox.Text = "Logarithmic Y-axis";
            logarithmicCheckBox.UseVisualStyleBackColor = false;
            logarithmicCheckBox.CheckedChanged += LogarithmicCheckBox_CheckedChanged;
            // 
            // MainForm
            // 
            AcceptButton = generateButton;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(1539, 1335);
            Controls.Add(graphPictureBox);
            Controls.Add(logarithmicCheckBox);
            Controls.Add(instructionLabel);
            Controls.Add(saveButton);
            Controls.Add(verticalLinesCheckBox);
            Controls.Add(squareSizeLabel);
            Controls.Add(squareSizeNumeric);
            Controls.Add(signalSquareLabel);
            Controls.Add(signalSquareNumeric);
            Controls.Add(resetRangeButton);
            Controls.Add(maxValueLabel);
            Controls.Add(minValueLabel);
            Controls.Add(maxValueNumeric);
            Controls.Add(minValueNumeric);
            Controls.Add(statisticsTextBox);
            Controls.Add(patternLabel);
            Controls.Add(patternComboBox);
            Controls.Add(numberOfExposuresLabel);
            Controls.Add(numberOfExposuresTrackBar);
            Controls.Add(readNoiseLabel);
            Controls.Add(readNoiseTrackBar);
            Controls.Add(exposureLabel);
            Controls.Add(exposureTrackBar);
            Controls.Add(signalLabel);
            Controls.Add(signalTrackBar);
            Controls.Add(backgroundLabel);
            Controls.Add(backgroundTrackBar);
            Controls.Add(pictureBox);
            Controls.Add(generateButton);
            ForeColor = Color.White;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Noise Simulator";
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)backgroundTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)signalTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)exposureTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)readNoiseTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)numberOfExposuresTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)minValueNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxValueNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)signalSquareNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)squareSizeNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)graphPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
