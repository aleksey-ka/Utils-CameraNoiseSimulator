using System.Threading.Tasks;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing;

namespace NoiseSimulator
{
    public partial class MainForm : Form
    {
        private Random random = new Random();
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
            
            // Initialize pattern ComboBox
            InitializePatternComboBox();
            
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

        #region Event Handlers

        // Event handlers for Designer compatibility
        private void GenerateButton_Click(object sender, EventArgs e)
        {
            GenerateNoiseImage();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveImageAsFits();
        }

        private void BackgroundTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateBackgroundLabel();
        }

        private void SignalTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateSignalLabel();
        }

        private void ExposureTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateExposureLabel();
        }

        private void ReadNoiseTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateReadNoiseLabel();
        }

        private void NumberOfExposuresTrackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateNumberOfExposuresLabel();
        }

        private void PatternComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSignalSquareRange();
            UpdateSquareSizeLabel();
            GenerateNoiseImage(); // Auto-regenerate image when pattern changes
        }

        private void SquareSizeNumeric_ValueChanged(object sender, EventArgs e)
        {
            GenerateNoiseImage();
        }

        private void VerticalLinesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateNoiseImage();
        }

        #endregion

        #region UI Initialization

        private void InitializePatternComboBox()
        {
            patternComboBox.Items.Clear();
            patternComboBox.Items.Add("Square");
            patternComboBox.Items.Add("3x3 Squares");
            patternComboBox.Items.Add("5x5 Squares");
            patternComboBox.Items.Add("7x7 Squares");
            patternComboBox.Items.Add("9x9 Squares");
            patternComboBox.Items.Add("Gaussian Spots");
            patternComboBox.Items.Add("Continuous lines");
            patternComboBox.Items.Add("No Signal");
            patternComboBox.SelectedIndex = 4; // Default to 9x9 Squares
        }

        #endregion

        #region Core Image Generation

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
        
        // Generate and display the ADU sum graph
        uint[] squareADUSums = CalculateSquareADUSums(detectedArray);
        GenerateIntensityGraph(squareADUSums);
    }
        
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

        #endregion

        #region UI Update Methods

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
        /// Event handler for logarithmic checkbox
        /// </summary>
        private void LogarithmicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
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

        #endregion

        #region Image Processing

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
        
        // Update the ADU sum graph
        if (currentImageData != null)
        {
            uint[] squareADUSums = CalculateSquareADUSums(currentImageData);
            GenerateIntensityGraph(squareADUSums);
        }
    }
        
        #endregion

        #region File Operations

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

        #endregion

        #region Graph Generation

                /// <summary>
        /// Calculates the sum of ADUs (pixel values) for each square in the current pattern
        /// Uses actual generated image data, not theoretical calculations
        /// </summary>
        /// <param name="imageData">The current generated image data</param>
        /// <returns>Array of ADU sums for each square, indexed by square number</returns>
        private uint[] CalculateSquareADUSums(uint[] imageData)
        {
            string selectedPattern = patternComboBox!.SelectedItem?.ToString() ?? "Square";
            int squareSize = (int)squareSizeNumeric!.Value;
            bool useVerticalLines = verticalLinesCheckBox!.Checked;
            
            // Get the maximum valid square index for the current pattern
            int maxValidIndex = signalGenerator.GetMaxValidIndex(selectedPattern);
            
            // Create array to hold ADU sums for each square
            uint[] squareADUSums = new uint[maxValidIndex + 1];
            
            // Define square parameters - center the pattern in the image
            const int centerX = 1024 / 2;
            const int centerY = 1024 / 2;
            const int gap = 30;
            int halfSquare = squareSize / 2;
            
                            // Calculate ADU sum for each square
                for (int squareIndex = 0; squareIndex <= maxValidIndex; squareIndex++)
                {
                    // Calculate the position of the current square
                    int squareX = centerX + signalGenerator.SpiralPatternDx[squareIndex] * (squareSize + gap);
                    int squareY = centerY + signalGenerator.SpiralPatternDy[squareIndex] * (squareSize + gap);
                    
                    uint squareSum = 0;
                    
                    // Sum all pixels within the square
                    for (int y = squareY - halfSquare; y < squareY + halfSquare; y++)
                    {
                        for (int x = squareX - halfSquare; x < squareX + halfSquare; x++)
                        {
                            if (x >= 0 && x < 1024 && y >= 0 && y < 1024)
                            {
                                int index = y * 1024 + x;
                                squareSum += imageData[index];
                            }
                        }
                    }
                    
                    // Store the sum of ADUs for this square
                    squareADUSums[squareIndex] = squareSum;
                }
            
            return squareADUSums;
        }

            /// <summary>
    /// Generates and displays a bar graph showing ADU sum distribution across pattern squares
    /// </summary>
    /// <param name="squareADUSums">Array of ADU sum values for each square</param>
    private void GenerateIntensityGraph(uint[] squareADUSums)
    {
        if (squareADUSums.Length == 0) return;
        
        const int graphWidth = 490;
        const int graphHeight = 433;
        const int margin = 40;
        const int barSpacing = 2;
        
        // Create bitmap for the graph
        Bitmap graphBitmap = new Bitmap(graphWidth, graphHeight);
        using (Graphics g = Graphics.FromImage(graphBitmap))
        {
            // Set up graphics quality
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Fill background
            g.Clear(Color.FromArgb(48, 48, 48));
            
                        // Find data range for scaling
            uint minADU = squareADUSums.Min();
            uint maxADU = squareADUSums.Max();
            uint range = maxADU - minADU;
            
            // Avoid division by zero
            if (range == 0) range = 1;
            
            // Check if logarithmic scaling is enabled
            bool useLogarithmic = logarithmicCheckBox!.Checked;
                
                // Calculate bar dimensions
                int availableWidth = graphWidth - 2 * margin;
                int barWidth = Math.Max(1, (availableWidth - (squareADUSums.Length - 1) * barSpacing) / squareADUSums.Length);
                int availableHeight = graphHeight - 2 * margin;
            
                // Ensure the last bar fits within the available width
                int totalBarWidth = squareADUSums.Length * barWidth + (squareADUSums.Length - 1) * barSpacing;
                if (totalBarWidth > availableWidth)
                {
                    barWidth = Math.Max(1, (availableWidth - (squareADUSums.Length - 1) * barSpacing) / squareADUSums.Length);
                }
                
                // Draw grid lines
                using (Pen gridPen = new Pen(Color.FromArgb(64, 64, 64), 1))
                {
                    // Vertical grid lines (every 5 squares)
                    for (int i = 0; i <= squareADUSums.Length; i += 5)
                    {
                        int x = margin + i * (barWidth + barSpacing);
                        if (x <= graphWidth - margin)
                        {
                            g.DrawLine(gridPen, x, margin, x, graphHeight - margin);
                        }
                    }
                    
                    // Horizontal grid lines (5 lines)
                    for (int i = 0; i <= 5; i++)
                    {
                        int y = margin + (i * availableHeight) / 5;
                        g.DrawLine(gridPen, margin, y, graphWidth - margin, y);
                    }
                }
                
                // Draw bars
                for (int i = 0; i < squareADUSums.Length; i++)
                {
                    uint aduSum = squareADUSums[i];
                    
                    // Calculate bar height (normalized to available height)
                    int barHeight;
                    if (useLogarithmic && minADU > 0)
                    {
                        // Logarithmic scaling: log(ADU) / log(maxADU) * availableHeight
                        float logMin = (float)Math.Log(minADU);
                        float logMax = (float)Math.Log(maxADU);
                        float logRange = logMax - logMin;
                        if (logRange > 0)
                        {
                            float normalizedValue = (float)Math.Log(aduSum) - logMin;
                            barHeight = (int)((normalizedValue / logRange) * availableHeight);
                        }
                        else
                        {
                            barHeight = 0;
                        }
                    }
                    else
                    {
                        // Linear scaling
                        barHeight = (int)(((aduSum - minADU) / (float)range) * availableHeight);
                    }
                    
                    // Calculate bar position
                    int x = margin + i * (barWidth + barSpacing);
                    int y = graphHeight - margin - barHeight;
                        
                                        // Choose color based on selection
                    Color barColor;
                    if (i == (int)signalSquareNumeric!.Value)
                    {
                        // Selected square - highlight in yellow
                        barColor = Color.Yellow;
                    }
                    else
                    {
                        // All other squares - use consistent blue color
                        barColor = Color.FromArgb(0, 150, 255);
                    }
                    
                    // Draw the bar
                    using (SolidBrush brush = new SolidBrush(barColor))
                    {
                        g.FillRectangle(brush, x, y, barWidth, barHeight);
                    }
                    
                    // Draw bar border
                    using (Pen borderPen = new Pen(Color.White, 1))
                    {
                        if( barHeight > 0 )
                        {
                            g.DrawRectangle(borderPen, x, y, barWidth, barHeight);
                        }
                        else
                        {
                            g.DrawLine( borderPen, x, y, x + barWidth, y);
                        }
                    }
                    
                    // Draw square index label below the bar (every 5th bar to avoid clutter)
                    if (i % 5 == 0 || i == squareADUSums.Length - 1)
                    {
                        using (Font labelFont = new Font("Arial", 8))
                        using (SolidBrush textBrush = new SolidBrush(Color.White))
                        {
                            string label = i.ToString();
                            SizeF textSize = g.MeasureString(label, labelFont);
                            float textX = x + (barWidth - textSize.Width) / 2;
                            g.DrawString(label, labelFont, textBrush, textX, graphHeight - margin + 5);
                        }
                    }
                }
                
                // Draw axis labels
                using (Font axisFont = new Font("Arial", 10, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    // Y-axis label (rotated)
                    string yLabel = useLogarithmic ? "Sum of ADUs (Log Scale)" : "Sum of ADUs";
                    g.TranslateTransform(margin - 20, graphHeight / 2);
                    g.RotateTransform(-90);
                    SizeF yLabelSize = g.MeasureString(yLabel, axisFont);
                    g.DrawString(yLabel, axisFont, textBrush, -yLabelSize.Width / 2, -yLabelSize.Height / 2);
                    g.ResetTransform();
                    
                    // X-axis label
                    string xLabel = "Square Index";
                    SizeF xLabelSize = g.MeasureString(xLabel, axisFont);
                    float xLabelX = margin + (availableWidth - xLabelSize.Width) / 2;
                    g.DrawString(xLabel, axisFont, textBrush, xLabelX, graphHeight - margin + 25);
                    
                    // Title
                    string title = $"Sum of ADUs - Pattern: {patternComboBox!.SelectedItem} {(useLogarithmic ? "(Log Scale)" : "")}";
                    SizeF titleSize = g.MeasureString(title, axisFont);
                    float titleX = margin + (availableWidth - titleSize.Width) / 2;
                    g.DrawString(title, axisFont, textBrush, titleX, 10);
                }
                
                // Display ADU sum value for the selected square
                int selectedSquareIndex = (int)signalSquareNumeric!.Value;
                if (selectedSquareIndex < squareADUSums.Length)
                {
                    uint selectedADUSum = squareADUSums[selectedSquareIndex];
                    string intensityText = $"Selected Square {selectedSquareIndex}: {selectedADUSum} ADUs";
                    
                    using (Font intensityFont = new Font("Arial", 12, FontStyle.Bold))
                    using (SolidBrush intensityBrush = new SolidBrush(Color.Yellow))
                    {
                        SizeF intensitySize = g.MeasureString(intensityText, intensityFont);
                        float intensityX = margin + (availableWidth - intensitySize.Width) / 2;
                        g.DrawString(intensityText, intensityFont, intensityBrush, intensityX, 35);
                    }
                }
                
                // No legend needed - simplified color scheme
            }
            
            // Display the graph
            graphPictureBox!.Image?.Dispose();
            graphPictureBox!.Image = graphBitmap;
        }

        #endregion
    }
}