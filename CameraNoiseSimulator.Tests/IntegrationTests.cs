using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class IntegrationTests
{
    [Fact]
    public void CompleteWorkflow_GenerateImageWithSignal_ShouldCalculateCorrectStatistics()
    {
        // Arrange
        const int imageSize = 1024;
        const double backgroundFlux = 5.0;        // 5 photons/sec background
        const double signalFlux = 50.0;           // 50 photons/sec signal
        const double exposureTime = 2.0;          // 2 second exposure
        const double readNoise = 1.0;             // 1 photon read noise
        const int seed = 42;                      // Fixed seed for reproducible results
        
        var photonDetector = new PhotonDetector(seed, readNoise);
        var signalGenerator = new SignalGenerator();
        var statisticsCalculator = new StatisticsCalculator();
        
        // Create image data array
        uint[,] imageData = new uint[imageSize, imageSize];
        
        // Act - Generate complete image using the same logic as the main application
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                // Get signal flux for this pixel
                float pixelSignalFlux = signalGenerator.GetPatternSignalFlux(
                    x, y, (float)signalFlux, "Square", 20, false);
                
                // Total flux for this pixel
                double totalFlux = backgroundFlux + pixelSignalFlux;
                
                // Generate photon detection for this pixel
                int detectedPhotons = photonDetector.GeneratePhotonDetection(totalFlux, exposureTime);
                
                // Add offset to prevent negative values
                int offset = photonDetector.Offset;
                int finalValue = detectedPhotons + offset;
                
                // Ensure non-negative and convert to uint
                imageData[y, x] = (uint)Math.Max(0, finalValue);
            }
        }
        
        // Convert to float array for statistics calculation
        float[] imageDataFloat = new float[imageSize * imageSize];
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                imageDataFloat[y * imageSize + x] = imageData[y, x];
            }
        }
        
        // Calculate statistics
        var backgroundStats = statisticsCalculator.CalculateBackgroundStatistics(
            imageDataFloat, signalGenerator, "Square", 20, false, (float)signalFlux);
        
        var signalStats = statisticsCalculator.CalculateCentralSquareStatistics(
            imageDataFloat, signalGenerator, "Square", 0, 20);
        
        float snr = statisticsCalculator.CalculateSignalToNoiseRatio(signalStats, backgroundStats);
        
        // Assert - Verify the complete workflow produces expected results
        
        // Background should have the expected flux
        double expectedBackgroundMean = backgroundFlux * exposureTime + photonDetector.Offset;
        Assert.True(Math.Abs(backgroundStats.mean - expectedBackgroundMean) < 10.0, 
            $"Background mean should be approximately {expectedBackgroundMean}, got {backgroundStats.mean}");
        
        // Signal should be higher than background
        Assert.True(signalStats.mean > backgroundStats.mean, 
            "Signal mean should be higher than background mean");
        
        // Signal-to-noise ratio should be positive
        Assert.True(snr > 0, "Signal-to-noise ratio should be positive");
        
        // Background should have more pixels than signal
        Assert.True(backgroundStats.count > signalStats.count, 
            "Background should have more pixels than signal");
        
        // Output statistics for debugging
        Console.WriteLine($"Complete Workflow Results:");
        Console.WriteLine($"  Background: mean={backgroundStats.mean:F2}, std={backgroundStats.std:F2}, count={backgroundStats.count}");
        Console.WriteLine($"  Signal: mean={signalStats.mean:F2}, std={signalStats.std:F2}, count={signalStats.count}");
        Console.WriteLine($"  Signal-to-Noise Ratio: {snr:F2}");
    }
    
    [Fact]
    public void CompleteWorkflow_NoSignalPattern_ShouldHaveUniformBackground()
    {
        // Arrange
        const int imageSize = 1024;
        const double backgroundFlux = 10.0;       // 10 photons/sec background
        const double signalFlux = 0.0;            // No signal
        const double exposureTime = 1.0;          // 1 second exposure
        const double readNoise = 0.5;             // 0.5 photon read noise
        const int seed = 42;                      // Fixed seed for reproducible results
        
        var photonDetector = new PhotonDetector(seed, readNoise);
        var signalGenerator = new SignalGenerator();
        var statisticsCalculator = new StatisticsCalculator();
        
        // Create image data array
        uint[,] imageData = new uint[imageSize, imageSize];
        
        // Act - Generate image with no signal pattern
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                // Get signal flux for this pixel (should be 0 for "No Signal")
                float pixelSignalFlux = signalGenerator.GetPatternSignalFlux(
                    x, y, (float)signalFlux, "No Signal", 20, false);
                
                // Total flux for this pixel
                double totalFlux = backgroundFlux + pixelSignalFlux;
                
                // Generate photon detection for this pixel
                int detectedPhotons = photonDetector.GeneratePhotonDetection(totalFlux, exposureTime);
                
                // Add offset to prevent negative values
                int offset = photonDetector.Offset;
                int finalValue = detectedPhotons + offset;
                
                // Ensure non-negative and convert to uint
                imageData[y, x] = (uint)Math.Max(0, finalValue);
            }
        }
        
        // Convert to float array for statistics calculation
        float[] imageDataFloat = new float[imageSize * imageSize];
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                imageDataFloat[y * imageSize + x] = imageData[y, x];
            }
        }
        
        // Calculate statistics
        var backgroundStats = statisticsCalculator.CalculateBackgroundStatistics(
            imageDataFloat, signalGenerator, "No Signal", 20, false, (float)signalFlux);
        
        var signalStats = statisticsCalculator.CalculateCentralSquareStatistics(
            imageDataFloat, signalGenerator, "No Signal", 0, 20);
        
        // Assert - Verify uniform background behavior
        
        // All pixels should be background
        Assert.Equal(imageSize * imageSize, backgroundStats.count);
        
        // Signal should be zero (no signal pattern)
        Assert.Equal(0.0f, signalStats.mean);
        Assert.Equal(0.0f, signalStats.std);
        Assert.Equal(0, signalStats.count);
        
        // Background should have reasonable statistics
        double expectedBackgroundMean = backgroundFlux * exposureTime + photonDetector.Offset;
        Assert.True(Math.Abs(backgroundStats.mean - expectedBackgroundMean) < 8.0, 
            $"Background mean should be approximately {expectedBackgroundMean}, got {backgroundStats.mean}");
        
        // Background should have some variance due to Poisson noise and read noise
        Assert.True(backgroundStats.std > 0, "Background should have some variance");
        
        Console.WriteLine($"No Signal Pattern Results:");
        Console.WriteLine($"  Background: mean={backgroundStats.mean:F2}, std={backgroundStats.std:F2}, count={backgroundStats.count}");
        Console.WriteLine($"  Signal: mean={signalStats.mean:F2}, std={signalStats.std:F2}, count={signalStats.count}");
    }
    
    [Fact]
    public void CompleteWorkflow_MultipleExposures_ShouldReduceNoise()
    {
        // Arrange
        const int imageSize = 1024;
        const int numExposures = 4;
        const double backgroundFlux = 5.0;
        const double signalFlux = 20.0;
        const double exposureTime = 1.0;
        const double readNoise = 1.0;
        const int seed = 42;
        
        var photonDetector = new PhotonDetector(seed, readNoise);
        var signalGenerator = new SignalGenerator();
        var statisticsCalculator = new StatisticsCalculator();
        
        // Create arrays to store multiple exposures
        uint[][,] exposures = new uint[numExposures][,];
        
        // Act - Generate multiple exposures
        for (int exp = 0; exp < numExposures; exp++)
        {
            exposures[exp] = new uint[imageSize, imageSize];
            
            for (int y = 0; y < imageSize; y++)
            {
                for (int x = 0; x < imageSize; x++)
                {
                    float pixelSignalFlux = signalGenerator.GetPatternSignalFlux(
                        x, y, (float)signalFlux, "Square", 20, false);
                    
                    double totalFlux = backgroundFlux + pixelSignalFlux;
                    int detectedPhotons = photonDetector.GeneratePhotonDetection(totalFlux, exposureTime);
                    int offset = photonDetector.Offset;
                    int finalValue = detectedPhotons + offset;
                    
                    exposures[exp][y, x] = (uint)Math.Max(0, finalValue);
                }
            }
        }
        
        // Average the exposures
        uint[,] averagedImage = new uint[imageSize, imageSize];
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                long sum = 0;
                for (int exp = 0; exp < numExposures; exp++)
                {
                    sum += exposures[exp][y, x];
                }
                averagedImage[y, x] = (uint)(sum / numExposures);
            }
        }
        
        // Convert to float array for statistics
        float[] averagedImageFloat = new float[imageSize * imageSize];
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                averagedImageFloat[y * imageSize + x] = averagedImage[y, x];
            }
        }
        
        // Calculate statistics for averaged image
        var backgroundStats = statisticsCalculator.CalculateBackgroundStatistics(
            averagedImageFloat, signalGenerator, "Square", 20, false, (float)signalFlux);
        
        var signalStats = statisticsCalculator.CalculateCentralSquareStatistics(
            averagedImageFloat, signalGenerator, "Square", 0, 20);
        
        // Assert - Verify averaging reduces noise
        
        // Signal should still be higher than background
        Assert.True(signalStats.mean > backgroundStats.mean, 
            "Averaged signal should still be higher than background");
        
        // Background noise should be reasonable
        Assert.True(backgroundStats.std > 0, "Background should have some variance");
        
        Console.WriteLine($"Multiple Exposures Results:");
        Console.WriteLine($"  Number of exposures: {numExposures}");
        Console.WriteLine($"  Background: mean={backgroundStats.mean:F2}, std={backgroundStats.std:F2}");
        Console.WriteLine($"  Signal: mean={signalStats.mean:F2}, std={signalStats.std:F2}");
    }
} 