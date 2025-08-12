using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class SignalGeneratorAndPhotonDetectorTests
{
    [Fact]
    public void GenerateImage_NoBackgroundNoReadNoiseNoSignal_ShouldHaveExpectedStatistics()
    {
        // Arrange
        const int imageSize = 1024;
        const double backgroundFlux = 0.0;        // No background
        const double signalFlux = 0.0;            // No signal
        const double exposureTime = 1.0;          // 1 second exposure
        const double readNoise = 0.0;             // No read noise
        const int seed = 42;                      // Fixed seed for reproducible results
        
        var photonDetector = new PhotonDetector(seed, readNoise);
        var signalGenerator = new SignalGenerator();
        
        // Create image data array
        uint[,] imageData = new uint[imageSize, imageSize];
        
        // Act - Generate image using the same logic as the main application
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                // Get signal flux for this pixel (should be 0 for no signal)
                float pixelSignalFlux = signalGenerator.GetPatternSignalFlux(
                    x, y, (float)signalFlux, "Square", 10, false);
                
                // Total flux for this pixel
                double totalFlux = backgroundFlux + pixelSignalFlux;
                
                // Generate photon detection for this pixel
                int detectedPhotons = photonDetector.GeneratePhotonDetection(totalFlux, exposureTime);
                
                // Add offset to prevent negative values (this is how the main app does it)
                int offset = photonDetector.Offset;
                int finalValue = detectedPhotons + offset;
                
                // Ensure non-negative and convert to uint
                imageData[y, x] = (uint)Math.Max(0, finalValue);
            }
        }
        
        // Assert - Calculate statistics and verify expectations
        
        // Calculate mean and standard deviation
        double sum = 0.0;
        double sumSquares = 0.0;
        int totalPixels = imageSize * imageSize;
        
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                sum += imageData[y, x];
                sumSquares += (double)imageData[y, x] * imageData[y, x];
            }
        }
        
        double mean = sum / totalPixels;
        double variance = (sumSquares / totalPixels) - (mean * mean);
        double stdDev = Math.Sqrt(variance);
        
        // Expected values:
        // - With no background flux, no signal flux, and no read noise:
        //   - Poisson(λ=0) = 0 (always)
        //   - Read noise = 0
        //   - Only offset remains
        double expectedMean = photonDetector.Offset;
        double expectedStdDev = 0.0; // No variance when all values are the same
        
        // Verify mean is exactly the offset value
        Assert.Equal(expectedMean, mean, precision: 0);
        
        // Verify standard deviation is 0 (all values should be identical)
        Assert.Equal(expectedStdDev, stdDev, precision: 6);
        
        // Verify all pixels have the same value (the offset)
        uint expectedPixelValue = (uint)photonDetector.Offset;
        for (int y = 0; y < imageSize; y++)
        {
            for (int x = 0; x < imageSize; x++)
            {
                Assert.Equal(expectedPixelValue, imageData[y, x]);
            }
        }
        
        // Additional verification: Check that offset is reasonable
        Assert.True(photonDetector.Offset >= 0, "Offset should be non-negative");
        
        // Output statistics for debugging
        Console.WriteLine($"Generated {imageSize}x{imageSize} image with:");
        Console.WriteLine($"  Background flux: {backgroundFlux} photons/sec");
        Console.WriteLine($"  Signal flux: {signalFlux} photons/sec");
        Console.WriteLine($"  Read noise: {readNoise}");
        Console.WriteLine($"  Exposure time: {exposureTime} seconds");
        Console.WriteLine($"  Detector offset: {photonDetector.Offset}");
        Console.WriteLine($"  Actual mean: {mean:F6}");
        Console.WriteLine($"  Actual std dev: {stdDev:F6}");
        Console.WriteLine($"  Expected mean: {expectedMean:F6}");
        Console.WriteLine($"  Expected std dev: {expectedStdDev:F6}");
        Console.WriteLine($"  All pixels have value: {expectedPixelValue}");
    }
    
    [Fact]
    public void PhotonDetector_ZeroFluxZeroReadNoise_ShouldReturnOnlyOffset()
    {
        // Arrange
        const int seed = 42;
        const double readNoise = 0.0;
        const double flux = 0.0;
        const double exposureTime = 1.0;
        
        var detector = new PhotonDetector(seed, readNoise);
        
        // Act
        int result = detector.GeneratePhotonDetection(flux, exposureTime);
        
        // Assert
        Assert.Equal(0, result); // Poisson(0) = 0
        Assert.Equal(0.0, detector.ReadNoise);
        Assert.True(detector.Offset >= 0);
    }
    
    [Fact]
    public void SignalGenerator_NoSignalPattern_ShouldReturnZeroFlux()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 10;
        
        // Act - Test a pixel that should have no signal
        float flux = generator.GetPatternSignalFlux(0, 0, baseSignalFlux, "Square", squareSize, false);
        
        // Assert - This pixel should have no signal (outside the square pattern)
        Assert.Equal(0.0f, flux);
    }
}