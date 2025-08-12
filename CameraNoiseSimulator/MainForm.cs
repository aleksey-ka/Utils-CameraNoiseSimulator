using System.Threading.Tasks;
using System.Threading;
using System.Drawing.Imaging;

namespace NoiseSimulator;

public partial class MainForm : Form
{
    private Random random = new Random();
    private PictureBox? pictureBox;
    private Button? generateButton;
    private TrackBar? backgroundTrackBar;
    private TrackBar? signalTrackBar;
    private TrackBar? exposureTrackBar;
    private TrackBar? readNoiseTrackBar;
    private TrackBar? numberOfExposuresTrackBar;
    private Label? backgroundLabel;
    private Label? signalLabel;
    private Label? exposureLabel;
    private Label? readNoiseLabel;
    private Label? numberOfExposuresLabel;
    private ComboBox? patternComboBox;
    private Label? patternLabel;
    private RichTextBox? statisticsTextBox;
    private NumericUpDown? minValueNumeric;
    private NumericUpDown? maxValueNumeric;
    private Label? minValueLabel;
    private Label? maxValueLabel;
    private Button? resetRangeButton;
    private NumericUpDown? signalSquareNumeric;
    private Label? signalSquareLabel;
    private NumericUpDown? squareSizeNumeric;
    private Label? squareSizeLabel;
    private CheckBox? verticalLinesCheckBox;
    private SignalGenerator signalGenerator;
    private StatisticsCalculator statisticsCalculator;
    private uint currentMinValue = 0;
    private uint currentMaxValue = 255;
    private uint[]? currentImageData = null;
    private uint actualDataMin = 0;
    private uint actualDataMax = 255;


    public MainForm()
    {
        // Initialize components
        InitializeComponent();
        
        // Initialize signal generator and statistics calculator
        signalGenerator = new SignalGenerator();
        statisticsCalculator = new StatisticsCalculator();
        
        // Update labels initially
        UpdateBackgroundLabel();
        UpdateSignalLabel();
        UpdateExposureLabel();
        UpdateReadNoiseLabel();
        UpdateNumberOfExposuresLabel();
        UpdateSignalSquareRange();
        UpdateSquareSizeLabel();
        
        // Generate initial image
        GenerateNoiseImage();
    }

    private void InitializeComponent()
    {
        this.Text = "Noise Simulator";
        this.Size = new Size(1370, 1400);
        this.StartPosition = FormStartPosition.CenterScreen;
        
        // Apply dark theme to form
        this.BackColor = Color.FromArgb(32, 32, 32);
        this.ForeColor = Color.White;
        
        // Create instruction label
        Label instructionLabel = new Label
        {
            Text = "Configure signal and background parameters below:",
            Size = new Size(800, 20),
            Location = new Point(170, 15),
            Font = new Font(FontFamily.GenericSansSerif, 10),
            ForeColor = Color.LightBlue,
            BackColor = Color.FromArgb(32, 32, 32)
        };

        // Create PictureBox for displaying the image
        pictureBox = new PictureBox
        {
            Size = new Size(1024, 1024),
            SizeMode = PictureBoxSizeMode.Normal,
            BorderStyle = BorderStyle.FixedSingle,
            Location = new Point(5, 330),
            BackColor = Color.FromArgb(48, 48, 48)
        };

        // Create generate button
        generateButton = new Button
        {
            Text = "Generate New",
            Size = new Size(150, 30),
            Location = new Point(10, 10),
            BackColor = Color.FromArgb(80, 80, 80), // Slightly lighter for prominence
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 1, BorderColor = Color.LightBlue }, // Add border to make it stand out
            Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold) // Bold font for emphasis
        };
        generateButton.Click += (sender, e) => GenerateNoiseImage();

        // Set as default button
        this.AcceptButton = generateButton;

        // Create save button
        Button saveButton = new Button
        {
            Text = "Save FITS",
            Size = new Size(190, 30),
            Location = new Point(1150, 10),
            BackColor = Color.FromArgb(64, 64, 64),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 }
        };
        saveButton.Click += (sender, e) => SaveImageAsFits();

        // Create background controls
        backgroundLabel = new Label
        {
            Text = "Background:",
            Size = new Size(110, 20),
            Location = new Point(10, 50),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32),
            TextAlign = ContentAlignment.MiddleRight
        };

        backgroundTrackBar = new TrackBar
        {
            Minimum = 0,
            Maximum = 1000,
            Value = 10,
            Size = new Size(1000, 45),
            Location = new Point(120, 45),
            TickFrequency = 100,
            BackColor = Color.FromArgb(32, 32, 32),
            TickStyle = TickStyle.BottomRight
        };
        backgroundTrackBar!.ValueChanged += (sender, e) => UpdateBackgroundLabel();

        // Create signal controls
        signalLabel = new Label
        {
            Text = "Signal:",
            Size = new Size(110, 20),
            Location = new Point(10, 100),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32),
            TextAlign = ContentAlignment.MiddleRight
        };

        signalTrackBar = new TrackBar
        {
            Minimum = 0,
            Maximum = 1000,
            Value = 100,
            Size = new Size(1000, 45),
            Location = new Point(120, 95),
            TickFrequency = 100,
            BackColor = Color.FromArgb(32, 32, 32),
            TickStyle = TickStyle.BottomRight
        };
        signalTrackBar!.ValueChanged += (sender, e) => UpdateSignalLabel();

        // Create exposure controls
        exposureLabel = new Label
        {
            Text = "Exposure:",
            Size = new Size(110, 20),
            Location = new Point(10, 150),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32),
            TextAlign = ContentAlignment.MiddleRight
        };

        exposureTrackBar = new TrackBar
        {
            Minimum = 0,
            Maximum = 1000,
            Value = 10, // Default to 1.0
            Size = new Size(1000, 45),
            Location = new Point(120, 145),
            TickFrequency = 100,
            BackColor = Color.FromArgb(32, 32, 32),
            TickStyle = TickStyle.BottomRight
        };
        exposureTrackBar!.ValueChanged += (sender, e) => UpdateExposureLabel();

        // Create read noise controls
        readNoiseLabel = new Label
        {
            Text = "Read Noise:",
            Size = new Size(110, 20),
            Location = new Point(10, 200),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32),
            TextAlign = ContentAlignment.MiddleRight
        };

        readNoiseTrackBar = new TrackBar
        {
            Minimum = 0,
            Maximum = 1000,
            Value = 10, // Default to 1.0
            Size = new Size(1000, 45),
            Location = new Point(120, 195),
            TickFrequency = 100,
            BackColor = Color.FromArgb(32, 32, 32),
            TickStyle = TickStyle.BottomRight
        };
        readNoiseTrackBar!.ValueChanged += (sender, e) => UpdateReadNoiseLabel();

        // Create number of exposures controls
        numberOfExposuresLabel = new Label
        {
            Text = "Exposures:",
            Size = new Size(110, 20),
            Location = new Point(10, 250),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32),
            TextAlign = ContentAlignment.MiddleRight
        };

        numberOfExposuresTrackBar = new TrackBar
        {
            Minimum = 1,
            Maximum = 256,
            Value = 1,
            Size = new Size(1000, 45),
            Location = new Point(120, 245),
            TickFrequency = 25,
            BackColor = Color.FromArgb(32, 32, 32),
            TickStyle = TickStyle.BottomRight
        };
        numberOfExposuresTrackBar!.ValueChanged += (sender, e) => UpdateNumberOfExposuresLabel();

        // Create pattern controls
        patternLabel = new Label
        {
            Text = "Signal Pattern:",
            Size = new Size(100, 20),
            Location = new Point(10, 300),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32)
        };

        patternComboBox = new ComboBox
        {
            Size = new Size(200, 25),
            Location = new Point(120, 295),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(48, 48, 48),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        patternComboBox.Items.Add("Square");
        patternComboBox.Items.Add("3x3 Squares");
        patternComboBox.Items.Add("5x5 Squares");
        patternComboBox.Items.Add("7x7 Squares");
        patternComboBox.Items.Add("9x9 Squares");
        patternComboBox.Items.Add("Gaussian Spots");
        patternComboBox.Items.Add("Continuous lines");
        patternComboBox.Items.Add("No Signal");
        patternComboBox.SelectedIndex = 4; // Default to 9x9 Squares
        patternComboBox.SelectedIndexChanged += (sender, e) => 
        {
            UpdateSignalSquareRange();
            UpdateSquareSizeLabel();
            GenerateNoiseImage(); // Auto-regenerate image when pattern changes
        };

        // Create square size controls
        squareSizeLabel = new Label
        {
            Text = "Square Size:",
            Size = new Size(80, 20),
            Location = new Point(330, 300),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32)
        };

        squareSizeNumeric = new NumericUpDown
        {
            Minimum = 10,
            Maximum = 200,
            Value = 60, // Default square size
            Size = new Size(80, 25),
            Location = new Point(410, 295),
            DecimalPlaces = 0,
            Increment = 5,
            BackColor = Color.FromArgb(48, 48, 48),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        squareSizeNumeric.ValueChanged += (sender, e) => GenerateNoiseImage();

        // Create vertical lines checkbox
        verticalLinesCheckBox = new CheckBox
        {
            Text = "Vertical Lines",
            Size = new Size(120, 25),
            Location = new Point(500, 295),
            Checked = true,
            BackColor = Color.FromArgb(32, 32, 32),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        verticalLinesCheckBox.CheckedChanged += (sender, e) => GenerateNoiseImage();

        // Create statistics text box
        statisticsTextBox = new RichTextBox
        {
            Size = new Size(300, 400),
            Location = new Point(1040, 500),
            Font = new Font(FontFamily.GenericMonospace, 9, FontStyle.Regular),
            BackColor = Color.FromArgb(48, 48, 48),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            ScrollBars = RichTextBoxScrollBars.None
        };

        // Create min value controls
        minValueLabel = new Label
        {
            Text = "Min:",
            Size = new Size(100, 20),
            Location = new Point(1040, 330),
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32)
        };

        minValueNumeric = new NumericUpDown
        {
            Minimum = 0,
            Maximum = int.MaxValue, // Handle very large photon counts
            Value = 0,
            Size = new Size(80, 25),
            Location = new Point(1140, 330),
            DecimalPlaces = 0,
            Increment = 1,
            BackColor = Color.FromArgb(48, 48, 48),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        minValueNumeric!.ValueChanged += MinValueNumeric_ValueChanged;

        // Create max value controls
        maxValueLabel = new Label
        {
            Text = "Max:",
            Size = new Size(100, 20),
            Location = new Point(1040, 360),
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32)
        };

        maxValueNumeric = new NumericUpDown
        {
            Minimum = 0,
            Maximum = int.MaxValue, // Handle very large photon counts
            Value = 255,
            Size = new Size(80, 25),
            Location = new Point(1140, 360),
            DecimalPlaces = 0,
            Increment = 1,
            BackColor = Color.FromArgb(48, 48, 48),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        maxValueNumeric!.ValueChanged += MaxValueNumeric_ValueChanged;

        // Create reset range button
        resetRangeButton = new Button
        {
            Text = "Reset Display Range",
            Size = new Size(140, 30),
            Location = new Point(1040, 390),
            Font = new Font(FontFamily.GenericSansSerif, 8),
            BackColor = Color.FromArgb(64, 64, 64),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 }
        };
        resetRangeButton!.Click += ResetRangeButton_Click;

        // Create signal square selection controls
        signalSquareLabel = new Label
        {
            Text = "Selected Square:",
            Size = new Size(100, 20),
            Location = new Point(1040, 430),
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(32, 32, 32)
        };

        signalSquareNumeric = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 80, // Maximum for 9x9 pattern (81 squares, 0-80)
            Value = 0, // Default to central square (index 0)
            Size = new Size(80, 25),
            Location = new Point(1140, 430),
            DecimalPlaces = 0,
            Increment = 1,
            BackColor = Color.FromArgb(48, 48, 48),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        signalSquareNumeric!.ValueChanged += SignalSquareNumeric_ValueChanged;

        // Add controls to form
        this.Controls.Add(instructionLabel);
        this.Controls.Add(pictureBox);
        this.Controls.Add(generateButton);
        this.Controls.Add(saveButton); // Add save button
        this.Controls.Add(backgroundLabel);
        this.Controls.Add(backgroundTrackBar);
        this.Controls.Add(signalLabel);
        this.Controls.Add(signalTrackBar);
        this.Controls.Add(exposureLabel);
        this.Controls.Add(exposureTrackBar);
        this.Controls.Add(readNoiseLabel);
        this.Controls.Add(readNoiseTrackBar);
        this.Controls.Add(numberOfExposuresLabel);
        this.Controls.Add(numberOfExposuresTrackBar);
        this.Controls.Add(patternLabel);
        this.Controls.Add(patternComboBox);
        this.Controls.Add(statisticsTextBox);
        this.Controls.Add(minValueLabel);
        this.Controls.Add(minValueNumeric);
        this.Controls.Add(maxValueLabel);
        this.Controls.Add(maxValueNumeric);
        this.Controls.Add(resetRangeButton);
        this.Controls.Add(signalSquareLabel);
        this.Controls.Add(signalSquareNumeric);
        this.Controls.Add(squareSizeLabel);
        this.Controls.Add(squareSizeNumeric);
        this.Controls.Add(verticalLinesCheckBox);
    }

    private void GenerateNoiseImage()
    {
        // Create photon detector for generating Poisson noise
        var detector = new PhotonDetector();
        
        // Set read noise from trackbar
        float readNoise = readNoiseTrackBar!.Value / 10.0f;
        detector.ReadNoise = readNoise;
        

        
        // Generate the average photon flux array
        float[] fluxArray = GenerateAveragePhotonFluxArray();
        
        // Apply photon detection simulation
        uint[] detectedArray = ApplyPhotonDetection(fluxArray, detector);

        // Create an array of floats by dividing values in detectedArray by numberOfExposures
        float[] detectedArrayFloats = new float[detectedArray.Length];
        int offset = detector.Offset;
        
        // Calculate theoretical expectations for comparison
        float backgroundFlux = backgroundTrackBar!.Value / 10.0f;
        float signalFlux = signalTrackBar!.Value / 10.0f;
        var theoreticalBackground = detector.GetTheoreticalStatistics(backgroundFlux, exposureTrackBar!.Value / 10.0f, numberOfExposuresTrackBar!.Value);
        

        
        for (int i = 0; i < detectedArray.Length; i++)
        {
            // For multiple exposures, we need to subtract offset * numberOfExposures
            // because each exposure added its own offset
            detectedArrayFloats[i] = ((float)detectedArray[i] - offset * numberOfExposuresTrackBar!.Value) / numberOfExposuresTrackBar!.Value;
        }
        

        
        // Get pattern and settings for statistics calculation
        string selectedPattern = patternComboBox!.SelectedItem?.ToString() ?? "Square";
        int squareSize = (int)squareSizeNumeric!.Value;
        bool useVerticalLines = verticalLinesCheckBox!.Checked;
        float baseSignalFlux = signalTrackBar!.Value / 10.0f;
        int selectedSquareIndex = (int)signalSquareNumeric!.Value;
        
        // Calculate statistics for background areas only
        var backgroundStats = statisticsCalculator.CalculateBackgroundStatistics(
            detectedArrayFloats, signalGenerator, selectedPattern, squareSize, useVerticalLines, baseSignalFlux);
        
        // Calculate statistics for central square signal area
        var signalStats = statisticsCalculator.CalculateCentralSquareStatistics(
            detectedArrayFloats, signalGenerator, selectedPattern, selectedSquareIndex, squareSize);
        
        // Calculate signal-to-background noise ratio
        float signalToNoiseRatio = statisticsCalculator.CalculateSignalToNoiseRatio(signalStats, backgroundStats);
        
        // Calculate source signal flux for the selected square
        float sourceSignalFlux = 0;
        if (selectedPattern != "No Signal")
        {
            sourceSignalFlux = statisticsCalculator.GetSourceSignalFlux(selectedSquareIndex, selectedPattern, baseSignalFlux, signalGenerator);
        }
        
        // Calculate theoretical signal statistics using the actual signal flux for the selected square
        var theoreticalSignal = detector.GetTheoreticalStatistics(backgroundFlux + sourceSignalFlux, exposureTrackBar!.Value / 10.0f, numberOfExposuresTrackBar!.Value);
        
        string squareInfo;
        if (selectedPattern == "No Signal")
        {
            squareInfo = "No Signal";
        }
        else
        {
            // Calculate magnitude assuming central square (index 0) is 0 magnitude
            float centralFlux = statisticsCalculator.GetSourceSignalFlux(0, selectedPattern, baseSignalFlux, signalGenerator);
            float magnitude = CalculateMagnitude(sourceSignalFlux, centralFlux);
            
            if (float.IsInfinity(magnitude))
            {
                squareInfo = $"Square {signalSquareNumeric!.Value} (flux={sourceSignalFlux:F2}, mag=∞)";
            }
            else
            {
                squareInfo = $"Square {signalSquareNumeric!.Value} (flux={sourceSignalFlux:F2}, mag={magnitude:F2})";
            }
        }
        

        
        // Don't show theoretical signal statistics if vertical lines are enabled (they're not accurate due to bimodal distribution)
        var displayTheoreticalSignal = useVerticalLines ? default : theoreticalSignal;
        
        UpdateStatisticsDisplay(backgroundStats, signalStats, squareInfo, signalToNoiseRatio, theoreticalBackground, displayTheoreticalSignal, offset, detectedArray, detectedArrayFloats, useVerticalLines);
        
        // Store the current image data for regeneration
        currentImageData = detectedArray;
        
        // Update min/max sliders to match actual data range
        uint actualMin = detectedArray.Min();
        uint actualMax = detectedArray.Max();
        
        // Temporarily disable value changed events to avoid recursion
        minValueNumeric!.ValueChanged -= MinValueNumeric_ValueChanged;
        maxValueNumeric!.ValueChanged -= MaxValueNumeric_ValueChanged;
        
        // Store actual data range for normalization
        actualDataMin = actualMin;
        actualDataMax = actualMax;
        
        // Initialize current min/max to actual data range
        currentMinValue = actualMin;
        currentMaxValue = actualMax;
        
        // Set numeric controls to actual data range (no clamping needed)
        minValueNumeric!.Value = (int)actualMin;
        maxValueNumeric!.Value = (int)actualMax;
        
        // Re-enable value changed events
        minValueNumeric!.ValueChanged += MinValueNumeric_ValueChanged;
        maxValueNumeric!.ValueChanged += MaxValueNumeric_ValueChanged;
        
        // Create bitmap and normalize the values
        Bitmap bitmap = CreateNormalizedBitmap(detectedArray);

        // Add green corner dots to visualize the selected square
        AddCornerDots(bitmap);

        // Display the bitmap
        pictureBox!.Image?.Dispose();
        pictureBox!.Image = bitmap;
    }
    
    /// <summary>
    /// Updates the background flux label with current trackbar value
    /// </summary>
    /// <summary>
    /// Updates a label with a trackbar value divided by 10
    /// </summary>
    private void UpdateTrackBarLabel(TrackBar trackBar, Label label, string prefix)
    {
        float value = trackBar.Value / 10.0f;
        label.Text = $"{prefix}: {value:F1}";
    }

    /// <summary>
    /// Updates a label with a trackbar value
    /// </summary>
    private void UpdateTrackBarLabel(TrackBar trackBar, Label label, string prefix, bool divideByTen = false)
    {
        if (divideByTen)
        {
            float value = trackBar.Value / 10.0f;
            label.Text = $"{prefix}: {value:F1}";
        }
        else
        {
            int value = trackBar.Value;
            label.Text = $"{prefix}: {value}";
        }
    }

    private void UpdateBackgroundLabel() => UpdateTrackBarLabel(backgroundTrackBar!, backgroundLabel!, "Background", true);
    private void UpdateSignalLabel() => UpdateTrackBarLabel(signalTrackBar!, signalLabel!, "Signal", true);
    private void UpdateExposureLabel() => UpdateTrackBarLabel(exposureTrackBar!, exposureLabel!, "Exposure", true);
    private void UpdateReadNoiseLabel() => UpdateTrackBarLabel(readNoiseTrackBar!, readNoiseLabel!, "Read Noise", true);
    private void UpdateNumberOfExposuresLabel() => UpdateTrackBarLabel(numberOfExposuresTrackBar!, numberOfExposuresLabel!, "Exposures", false);

    /// <summary>
    /// Gets the current application settings
    /// </summary>
    private (string pattern, int squareSize, bool useVerticalLines, float baseSignalFlux, int selectedSquareIndex) GetCurrentSettings()
    {
        return (
            patternComboBox!.SelectedItem?.ToString() ?? "Square",
            (int)squareSizeNumeric!.Value,
            verticalLinesCheckBox!.Checked,
            signalTrackBar!.Value / 10.0f,
            (int)signalSquareNumeric!.Value
        );
    }

    /// <summary>
    /// Updates the min value label with current numeric value
    /// </summary>




    /// <summary>
    /// Regenerates the bitmap with current min/max values
    /// </summary>
    private void RegenerateBitmap()
    {
        if (currentImageData is not null)
        {
            Bitmap bitmap = CreateNormalizedBitmap(currentImageData);
            AddCornerDots(bitmap);
            pictureBox!.Image?.Dispose();
            pictureBox!.Image = bitmap;
        }
    }

    /// <summary>
    /// Event handler for min value numeric control
    /// </summary>
    private void MinValueNumeric_ValueChanged(object? sender, EventArgs e)
    {
        currentMinValue = (uint)minValueNumeric!.Value;
        RegenerateBitmap();
    }

    /// <summary>
    /// Event handler for max value numeric control
    /// </summary>
    private void MaxValueNumeric_ValueChanged(object? sender, EventArgs e)
    {
        currentMaxValue = (uint)maxValueNumeric!.Value;
        RegenerateBitmap();
    }

    /// <summary>
    /// Event handler for reset range button
    /// </summary>
    private void ResetRangeButton_Click(object? sender, EventArgs e)
    {
        // Temporarily disable value changed events to avoid recursion
        minValueNumeric!.ValueChanged -= MinValueNumeric_ValueChanged;
        maxValueNumeric!.ValueChanged -= MaxValueNumeric_ValueChanged;
        
        // Reset to actual data range with safety checks
        int minValue = (int)Math.Min(actualDataMin, int.MaxValue);
        int maxValue = (int)Math.Min(actualDataMax, int.MaxValue);
        
        minValueNumeric!.Value = minValue;
        maxValueNumeric!.Value = maxValue;
        currentMinValue = (uint)minValue;
        currentMaxValue = (uint)maxValue;
        
        // Re-enable value changed events
        minValueNumeric!.ValueChanged += MinValueNumeric_ValueChanged;
        maxValueNumeric!.ValueChanged += MaxValueNumeric_ValueChanged;
        
        // Regenerate bitmap
        RegenerateBitmap();
    }

    /// <summary>
    /// Event handler for signal square numeric control
    /// </summary>
    private void SignalSquareNumeric_ValueChanged(object? sender, EventArgs e)
    {
        // Direct update without timer
        if (currentImageData is not null)
        {
            RecalculateStatistics();
        }
    }

    
    /// <summary>
    /// Generates an array representing average photon flux values for each pixel
    /// </summary>
    /// <returns>Array of float values representing average photon flux</returns>
    private float[] GenerateAveragePhotonFluxArray()
    {
        // Get current flux values from trackbars
        float backgroundFlux = backgroundTrackBar!.Value / 10.0f;
        float signalFlux = signalTrackBar!.Value / 10.0f;
        
        // Create array to hold the average photon flux values
        float[] fluxArray = new float[1024 * 1024];
        
        // Get pattern and settings
        string selectedPattern = patternComboBox!.SelectedItem?.ToString() ?? "Square";
        int squareSize = (int)squareSizeNumeric!.Value;
        bool useVerticalLines = verticalLinesCheckBox!.Checked;
        
        // Generate the base image with background and selected pattern
        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                int index = y * 1024 + x;
                
                // Apply selected pattern with appropriate signal flux and store the average photon flux value
                fluxArray[index] = backgroundFlux + signalGenerator.GetPatternSignalFlux(x, y, signalFlux, selectedPattern, squareSize, useVerticalLines);
            }
        }
        
        return fluxArray;
    }
    

    
    /// <summary>
    /// Applies photon detection simulation to an array of average photon flux values
    /// </summary>
    /// <param name="fluxArray">Array of average photon flux values</param>
    /// <param name="detector">PhotonDetector instance for generating Poisson noise</param>
    /// <returns>Array of uint values representing detected photon counts</returns>
    private uint[] ApplyPhotonDetection(float[] fluxArray, PhotonDetector detector)
    {
        // Create array to hold the detected photon counts (automatically initialized to 0)
        uint[] detectedArray = new uint[fluxArray.Length];
        
        // Set read noise from trackbar
        detector.ReadNoise = readNoiseTrackBar!.Value / 10.0f;

        // Get exposure time from trackbar
        float exposureTime = exposureTrackBar!.Value / 10.0f;

        // Get number of exposures from trackbar
        int numberOfExposures = numberOfExposuresTrackBar!.Value;
        
        // Use parallel processing for multiple exposures to improve performance
        if (numberOfExposures > 1)
        {
            // Create thread-local detectors for parallel processing
            var detectors = new PhotonDetector[Environment.ProcessorCount];
            for (int i = 0; i < detectors.Length; i++)
            {
                detectors[i] = new PhotonDetector(readNoise: detector.ReadNoise);
            }
            
            // Process exposures in parallel
            Parallel.For(0, numberOfExposures, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, n =>
            {
                var localDetector = detectors[n % detectors.Length];
                
                // Apply photon detection to each pixel
                for (int i = 0; i < fluxArray.Length; i++)
                {
                    // Simulate photon detection for the specified exposure time with the given flux
                    int detectedPhotons = localDetector.GeneratePhotonDetection(fluxArray[i], exposureTime);
                    
                    // Thread-safe addition using Interlocked (allow negative values)
                    Interlocked.Add(ref detectedArray[i], (uint)Math.Max(0, detectedPhotons));
                }
            });
        }
        else
        {
            // Single exposure - no need for parallel processing
            for (int i = 0; i < fluxArray.Length; i++)
            {
                // Simulate photon detection for the specified exposure time with the given flux
                int detectedPhotons = detector.GeneratePhotonDetection(fluxArray[i], exposureTime);
                
                // Store the detected photon count (ensure non-negative)
                detectedArray[i] = (uint)Math.Max(0, detectedPhotons);
            }
        }
        return detectedArray;
    }
    
    /// <summary>
    /// Creates a normalized bitmap from the photon flux array
    /// </summary>
    /// <param name="imageValues">Array of photon flux values</param>
    /// <returns>Normalized grayscale bitmap</returns>
    private Bitmap CreateNormalizedBitmap(uint[] imageValues)
    {
        // Use custom min and max values for normalization (user-controlled display range)
        uint minValue = currentMinValue;
        uint maxValue = currentMaxValue;
        
        // Create bitmap and normalize the values using LockBits for speed
        Bitmap bitmap = new Bitmap(1024, 1024, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        
        // Lock the bitmap for fast pixel access
        System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(
            new Rectangle(0, 0, 1024, 1024),
            System.Drawing.Imaging.ImageLockMode.WriteOnly,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        
        try
        {
            // Get the address of the first line
            IntPtr ptr = bmpData.Scan0;
            
            // Declare an array to hold the bytes of the bitmap
            int bytes = Math.Abs(bmpData.Stride) * 1024;
            byte[] rgbValues = new byte[bytes];
            
            // Calculate normalization factor once
            double normalizationFactor = (maxValue == minValue) ? 0 : 255.0 / (maxValue - minValue);
            
            // Fill the array with normalized pixel values
        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                int index = y * 1024 + x;
                    int pixelIndex = y * bmpData.Stride + x * 3;
                
                byte normalizedValue;
                if (maxValue == minValue)
                {
                    normalizedValue = 128; // If all values are the same, use middle gray
                }
                else
                {
                        // Clamp the pixel value to the display range and normalize
                        uint clampedPixelValue = Math.Max(minValue, Math.Min(maxValue, imageValues[index]));
                        normalizedValue = (byte)((clampedPixelValue - minValue) * normalizationFactor);
                    }
                    
                    // Set BGR values (Format24bppRgb is BGR, not RGB)
                    rgbValues[pixelIndex] = normalizedValue;     // Blue
                    rgbValues[pixelIndex + 1] = normalizedValue; // Green
                    rgbValues[pixelIndex + 2] = normalizedValue; // Red
                }
            }
            
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
        }
        finally
        {
            // Unlock the bitmap
            bitmap.UnlockBits(bmpData);
        }
        
        return bitmap;
    }
    

    

    

    
    /// <summary>
    /// Adds green corner dots to visualize the selected square
    /// </summary>
    /// <param name="bitmap">The bitmap to add corner dots to</param>
    private void AddCornerDots(Bitmap bitmap)
    {
        // Check if "No Signal" pattern is selected
        string selectedPattern = patternComboBox!.SelectedItem?.ToString() ?? "Square";
        if (selectedPattern == "No Signal")
        {
            return; // No square to highlight
        }
        
        // Get the selected square index
        int selectedSquareIndex = (int)signalSquareNumeric!.Value;
        
        // Determine the maximum valid square index for the current pattern
        int maxValidIndex = signalGenerator.GetMaxValidIndex(selectedPattern);
        
        // Check if the selected square index is valid
        if (selectedSquareIndex > maxValidIndex)
        {
            return; // Invalid square index - don't add dots
        }
        
        // Don't show dots for the central square (index 0)
        if (selectedSquareIndex == 0)
        {
            return; // Central square - don't add dots
        }
        
        // Define square parameters
        const int centerX = 1024 / 2;
        const int centerY = 1024 / 2;
        int squareSize = (int)squareSizeNumeric!.Value;
        const int gap = 30;
        int halfSquare = squareSize / 2;
        
        // Calculate the position of the selected square
        int squareX = centerX + signalGenerator.SpiralPatternDx[selectedSquareIndex] * (squareSize + gap);
        int squareY = centerY + signalGenerator.SpiralPatternDy[selectedSquareIndex] * (squareSize + gap);
        
        // Calculate corner positions
        int topLeftX = squareX - halfSquare;
        int topLeftY = squareY - halfSquare;
        int topRightX = squareX + halfSquare - 1;
        int topRightY = squareY - halfSquare;
        int bottomLeftX = squareX - halfSquare;
        int bottomLeftY = squareY + halfSquare - 1;
        int bottomRightX = squareX + halfSquare - 1;
        int bottomRightY = squareY + halfSquare - 1;
        
        // Define green color for corner dots
        Color greenColor = Color.Lime;
        
        // Add corner dots (3x3 pixel squares for visibility)
        const int dotSize = 3;
        
        // Top-left corner
        for (int y = topLeftY; y < topLeftY + dotSize && y < 1024; y++)
        {
            for (int x = topLeftX; x < topLeftX + dotSize && x < 1024; x++)
            {
                if (x >= 0 && y >= 0)
                {
                    bitmap.SetPixel(x, y, greenColor);
                }
            }
        }
        
        // Top-right corner
        for (int y = topRightY; y < topRightY + dotSize && y < 1024; y++)
        {
            for (int x = topRightX - dotSize + 1; x <= topRightX && x < 1024; x++)
            {
                if (x >= 0 && y >= 0)
                {
                    bitmap.SetPixel(x, y, greenColor);
                }
            }
        }
        
        // Bottom-left corner
        for (int y = bottomLeftY - dotSize + 1; y <= bottomLeftY && y < 1024; y++)
        {
            for (int x = bottomLeftX; x < bottomLeftX + dotSize && x < 1024; x++)
            {
                if (x >= 0 && y >= 0)
                {
                    bitmap.SetPixel(x, y, greenColor);
                }
            }
        }
        
        // Bottom-right corner
        for (int y = bottomRightY - dotSize + 1; y <= bottomRightY && y < 1024; y++)
        {
            for (int x = bottomRightX - dotSize + 1; x <= bottomRightX && x < 1024; x++)
            {
                if (x >= 0 && y >= 0)
                {
                    bitmap.SetPixel(x, y, greenColor);
                }
            }
        }
    }
    
    /// <summary>
    /// Updates the signal square numeric control range based on the selected pattern
    /// </summary>
    private void UpdateSignalSquareRange()
    {
        string selectedPattern = patternComboBox!.SelectedItem?.ToString() ?? "Square";
        int maxValidIndex = signalGenerator.GetMaxValidIndex(selectedPattern);
        
        // Update the numeric control range
        signalSquareNumeric!.Maximum = maxValidIndex;
        
        // If current value exceeds new maximum, reset to 0
        if (signalSquareNumeric!.Value > maxValidIndex)
        {
            signalSquareNumeric!.Value = 0;
        }
    }

    /// <summary>
    /// Updates the square size label based on the selected pattern
    /// </summary>
    private void UpdateSquareSizeLabel()
    {
        string selectedPattern = patternComboBox!.SelectedItem?.ToString() ?? "Square";
        
        if (selectedPattern == "Gaussian Spots")
        {
            squareSizeLabel!.Text = "Spot Size:";
            // Update numeric control for Gaussian spots
            squareSizeNumeric!.Minimum = 1;
            squareSizeNumeric!.Maximum = 200;
            if (squareSizeNumeric!.Value < 1)
            {
                squareSizeNumeric!.Value = 3; // Default for Gaussian spots
            }
        }
        else
        {
            squareSizeLabel!.Text = "Square Size:";
            // Update numeric control for square patterns
            squareSizeNumeric!.Minimum = 10;
            squareSizeNumeric!.Maximum = 200;
            if (squareSizeNumeric!.Value < 10)
            {
                squareSizeNumeric!.Value = 60; // Default for squares
            }
        }
    }
    
    /// <summary>
    /// Calculates magnitude from flux, assuming central square is 0 magnitude
    /// </summary>
    /// <param name="flux">Flux value</param>
    /// <param name="centralFlux">Central square flux (0 magnitude reference)</param>
    /// <returns>Magnitude value</returns>
    private float CalculateMagnitude(float flux, float centralFlux)
    {
        if (flux <= 0 || centralFlux <= 0) return float.PositiveInfinity;
        return -2.5f * MathF.Log10(flux / centralFlux);
    }

    /// <summary>
    /// Recalculates statistics without regenerating the image
    /// </summary>
    private void RecalculateStatistics()
    {
        if (currentImageData is null) return;
        
        // Create an array of floats by dividing values in currentImageData by numberOfExposures
        float[] detectedArrayFloats = new float[currentImageData!.Length];
        float readNoise = readNoiseTrackBar!.Value / 10.0f;
        var detector = new PhotonDetector(readNoise: readNoise);
        int offset = detector.Offset;
        int numberOfExposures = numberOfExposuresTrackBar!.Value;
        for (int i = 0; i < currentImageData!.Length; i++)
        {
            detectedArrayFloats[i] = ((float)currentImageData[i] - offset) / numberOfExposures;
        }
        
        // Calculate theoretical expectations for comparison
        float backgroundFlux = backgroundTrackBar!.Value / 10.0f;
        float signalFlux = signalTrackBar!.Value / 10.0f;
        var theoreticalBackground = detector.GetTheoreticalStatistics(backgroundFlux, exposureTrackBar!.Value / 10.0f, numberOfExposuresTrackBar!.Value);
        
        // Get pattern and settings for statistics calculation
        string selectedPattern = patternComboBox!.SelectedItem?.ToString() ?? "Square";
        int squareSize = (int)squareSizeNumeric!.Value;
        bool useVerticalLines = verticalLinesCheckBox!.Checked;
        float baseSignalFlux = signalTrackBar!.Value / 10.0f;
        int selectedSquareIndex = (int)signalSquareNumeric!.Value;
        
        // Calculate statistics for background areas only
        var backgroundStats = statisticsCalculator.CalculateBackgroundStatistics(
            detectedArrayFloats, signalGenerator, selectedPattern, squareSize, useVerticalLines, baseSignalFlux);
        
        // Calculate statistics for selected square signal area
        var signalStats = statisticsCalculator.CalculateCentralSquareStatistics(
            detectedArrayFloats, signalGenerator, selectedPattern, selectedSquareIndex, squareSize);
        
        // Calculate signal-to-background noise ratio
        float signalToNoiseRatio = statisticsCalculator.CalculateSignalToNoiseRatio(signalStats, backgroundStats);
        
        // Calculate source signal flux for the selected square
        float sourceSignalFlux = 0;
        if (selectedPattern != "No Signal")
        {
            sourceSignalFlux = statisticsCalculator.GetSourceSignalFlux(selectedSquareIndex, selectedPattern, baseSignalFlux, signalGenerator);
        }
        
        // Calculate theoretical signal statistics using the actual signal flux for the selected square
        var theoreticalSignal = detector.GetTheoreticalStatistics(backgroundFlux + sourceSignalFlux, exposureTrackBar!.Value / 10.0f, numberOfExposuresTrackBar!.Value);
        
        // Show the results in the statistics label with syntax coloring
        string squareInfo;
        if (selectedPattern == "No Signal")
        {
            squareInfo = "No Signal";
        }
        else
        {
            // Calculate magnitude assuming central square (index 0) is 0 magnitude
            float centralFlux = statisticsCalculator.GetSourceSignalFlux(0, selectedPattern, baseSignalFlux, signalGenerator);
            float magnitude = CalculateMagnitude(sourceSignalFlux, centralFlux);
            
            if (float.IsInfinity(magnitude))
            {
                squareInfo = $"Square {signalSquareNumeric!.Value} (flux={sourceSignalFlux:F2}, mag=∞)";
            }
            else
            {
                squareInfo = $"Square {signalSquareNumeric!.Value} (flux={sourceSignalFlux:F2}, mag={magnitude:F2})";
            }
        }
        
        // Don't show theoretical signal statistics if vertical lines are enabled (they're not accurate due to bimodal distribution)
        var displayTheoreticalSignal = useVerticalLines ? default : theoreticalSignal;
        
        UpdateStatisticsDisplay(backgroundStats, signalStats, squareInfo, signalToNoiseRatio, theoreticalBackground, displayTheoreticalSignal, offset, currentImageData, detectedArrayFloats, useVerticalLines);
    }
    
    /// <summary>
    /// Saves the current image as a FITS file with 16-bit dynamic range
    /// </summary>
    private void SaveImageAsFits()
    {
        if (currentImageData == null)
        {
            MessageBox.Show("No image data available. Please generate an image first.", "No Image", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using (SaveFileDialog saveDialog = new SaveFileDialog())
        {
            saveDialog.Filter = "FITS files (*.fits)|*.fits|All files (*.*)|*.*";
            saveDialog.FilterIndex = 1;
            saveDialog.DefaultExt = "fits";
            saveDialog.Title = "Save Image as FITS";
            
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Convert uint[] to ushort[,] for FITS
                    ushort[,] fitsData = new ushort[1024, 1024];
                    
                    // Use the raw detected data for FITS (not display-normalized)
                    // This ensures proper brightness values for astronomical viewing
                    uint[] rawData = currentImageData;
                    
                    // Find the actual data range for proper scaling
                    uint minVal = rawData.Min();
                    uint maxVal = rawData.Max();
                    uint range = maxVal - minVal;
                    
                    // For FITS 16-bit data, use simple offset with BSCALE=1.0
                    // Physical value = BZERO + BSCALE * FITS_value
                    // Where FITS_value is signed (-32768 to +32767)
                    
                    // Use simple offset approach:
                    // BZERO = offset to shift data range appropriately
                    // BSCALE = 1.0 (no scaling, just offset)
                    double bzero = 32768.0;  // Standard offset for 16-bit data
                    double bscale = 1.0;     // No scaling
                    
                    // Debug information
                    string debugInfo = $"Data range: {minVal} to {maxVal} (range: {range})\n";
                    debugInfo += $"Background level: {backgroundTrackBar!.Value / 10.0f:F1}\n";
                    debugInfo += $"Signal level: {signalTrackBar!.Value / 10.0f:F1}\n";
                    debugInfo += $"Read noise: {readNoiseTrackBar!.Value / 10.0f:F1}\n";
                    debugInfo += $"Exposures: {numberOfExposuresTrackBar!.Value}\n";
                    debugInfo += $"BZERO: {bzero:F1}, BSCALE: {bscale:F1}\n";
                    debugInfo += $"FITS mapping: -32768 to +32767 with offset {bzero}";
                    
                    // Convert data to 16-bit without scaling, just use the raw values
                    for (int y = 0; y < 1024; y++)
                    {
                        for (int x = 0; x < 1024; x++)
                        {
                            uint pixelValue = rawData[y * 1024 + x];
                            
                            // Use the raw values directly, no scaling
                            // The FITS writer will apply the offset
                            ushort scaledValue = (ushort)Math.Min(pixelValue, 65535);
                            
                            fitsData[y, x] = scaledValue;
                        }
                    }
                    
                    // Save as FITS with proper scaling parameters
                    // Use BZERO and BSCALE to ensure correct data interpretation
                    FitsWriter.SaveFits(saveDialog.FileName, fitsData);
                    
                    // Show success message with debug info
                    MessageBox.Show($"Image saved successfully as FITS to:\n{saveDialog.FileName}\n\nFile size: {new FileInfo(saveDialog.FileName).Length} bytes\n16-bit dynamic range preserved.\n\n{debugInfo}", "Save Successful", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving FITS file: {ex.Message}\n\nTry saving with a different filename or location.", "Save Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    /// <summary>
    /// Updates the statistics display with color formatting
    /// </summary>
    private void UpdateStatisticsDisplay((float mean, float std, int count) backgroundStats, 
        (float mean, float std, int count) signalStats, string squareInfo, float signalToNoiseRatio,
        (double mean, double stdDev) theoreticalBackground = default,
        (double mean, double stdDev) theoreticalSignal = default,
        int offset = 0,
        uint[]? detectedArray = null,
        float[]? detectedArrayFloats = null,
        bool useVerticalLines = false)
    {
        // Create RTF string with color formatting and proper spacing for alignment
        string rtfText =
            "{\\rtf1\\ansi\\deff0" +
            "{\\colortbl;\\red255\\green255\\blue255;\\red128\\green128\\blue128;\\red100\\green255\\blue100;\\red255\\green255\\blue0;}" +
            "\\viewkind4\\uc1\\pard" +
            "\\cf2 Background:\\par" +
            "  Mean: \\cf1\\b " + $"{backgroundStats.mean,8:F3}" + "\\cf2\\b0\\par" +
            "  Std:  \\cf1\\b " + $"{backgroundStats.std,8:F3}" + "\\cf2\\b0\\par";
        
        // Add theoretical values if provided
        if (theoreticalBackground != default)
        {
            rtfText += "\\par\\cf2 Theory (Background):\\par" +
                      "  Mean: \\cf1\\b " + $"{theoreticalBackground.mean,8:F3}" + "\\cf2\\b0\\par" +
                      "  Std:  \\cf1\\b " + $"{theoreticalBackground.stdDev,8:F3}" + "\\cf2\\b0\\par";
        }
        
        rtfText += "\\par" +
                  $"\\cf2 {squareInfo}:\\par" +
                  "  Mean: \\cf1\\b " + $"{signalStats.mean,8:F3}" + "\\cf2\\b0\\par" +
                  "  Std:  \\cf1\\b " + $"{signalStats.std,8:F3}" + "\\cf2\\b0\\par";
        
        // Add theoretical values if provided
        if (theoreticalSignal != default)
        {
            rtfText += "\\par\\cf2 Theory (Signal):\\par" +
                      "  Mean: \\cf1\\b " + $"{theoreticalSignal.mean,8:F3}" + "\\cf2\\b0\\par" +
                      "  Std:  \\cf1\\b " + $"{theoreticalSignal.stdDev,8:F3}" + "\\cf2\\b0\\par";
        }
        
        rtfText += "\\par\\cf2 S/N Ratio: \\cf3\\b " + $"{signalToNoiseRatio,4:F2}" + "\\cf2\\b0\\par";
        
        // Add warning if vertical lines are enabled
        if (useVerticalLines)
        {
            rtfText += "\\par\\cf4 Signal contains vertical lines pattern!\\cf2\\b0\\par";
        }
                  
        
        if (offset > 0 && detectedArray is not null && detectedArrayFloats is not null)
        {
            rtfText += "\\par\\cf2 Offset: \\cf1\\b " + $"{offset}" + "\\cf2\\b0";
            rtfText += "\\par\\cf2 Raw Range: \\cf1\\b " + $"{detectedArray.Min()}-{detectedArray.Max()}" + "\\cf2\\b0";
            rtfText += "\\par\\cf2 After Offset: \\cf1\\b " + $"{detectedArrayFloats.Min():F1}-{detectedArrayFloats.Max():F1}" + "\\cf2\\b0";
        }
        
        rtfText += "}";
        statisticsTextBox!.Rtf = rtfText;
    }
} 