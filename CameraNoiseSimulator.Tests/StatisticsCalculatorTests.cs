using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class StatisticsCalculatorTests
{
    [Fact]
    public void CalculateBackgroundStatistics_UniformImage_ShouldHaveExpectedStatistics()
    {
        // Arrange
        var calculator = new StatisticsCalculator();
        var signalGenerator = new SignalGenerator();
        const int imageSize = 1024; // Must be 1024 as expected by StatisticsCalculator
        const float uniformValue = 100.0f;
        
        float[] imageData = new float[imageSize * imageSize];
        for (int i = 0; i < imageData.Length; i++)
        {
            imageData[i] = uniformValue;
        }
        
        // Act
        var stats = calculator.CalculateBackgroundStatistics(
            imageData, signalGenerator, "No Signal", 10, false, 0.0f);
        
        // Assert
        Assert.Equal(uniformValue, stats.mean, precision: 6);
        Assert.Equal(0.0f, stats.std, precision: 6); // No variance in uniform image
        Assert.Equal(imageData.Length, stats.count); // All pixels are background
    }
    
    [Fact]
    public void CalculateBackgroundStatistics_ImageWithSignal_ShouldExcludeSignalPixels()
    {
        // Arrange
        var calculator = new StatisticsCalculator();
        var signalGenerator = new SignalGenerator();
        const int imageSize = 1024; // Must be 1024 as expected by StatisticsCalculator
        const float backgroundValue = 100.0f;
        const float signalValue = 200.0f;
        
        float[] imageData = new float[imageSize * imageSize];
        
        // Set background
        for (int i = 0; i < imageData.Length; i++)
        {
            imageData[i] = backgroundValue;
        }
        
        // Set signal in center region (20x20 pixels)
        int signalStart = (imageSize - 20) / 2;
        int signalEnd = signalStart + 20;
        for (int y = signalStart; y < signalEnd; y++)
        {
            for (int x = signalStart; x < signalEnd; x++)
            {
                int index = y * imageSize + x;
                imageData[index] = signalValue;
            }
        }
        
        // Act
        var stats = calculator.CalculateBackgroundStatistics(
            imageData, signalGenerator, "Square", 20, false, 100.0f);
        
        // Assert
        Assert.Equal(backgroundValue, stats.mean, precision: 6);
        Assert.True(stats.count < imageData.Length, "Background count should be less than total pixels");
        
        // Verify signal region was excluded from background calculation
        int expectedBackgroundPixels = imageData.Length - (20 * 20);
        Assert.Equal(expectedBackgroundPixels, stats.count);
    }
    
    [Fact]
    public void CalculateCentralSquareStatistics_ValidSquare_ShouldCalculateCorrectly()
    {
        // Arrange
        var calculator = new StatisticsCalculator();
        var signalGenerator = new SignalGenerator();
        const int imageSize = 1024; // Must be 1024 as expected by StatisticsCalculator
        const float signalValue = 200.0f;
        
        float[] imageData = new float[imageSize * imageSize];
        
        // Set background
        for (int i = 0; i < imageData.Length; i++)
        {
            imageData[i] = 100.0f;
        }
        
        // Set signal in center square (index 0)
        int centerX = imageSize / 2;
        int centerY = imageSize / 2;
        int squareSize = 20;
        int halfSquare = squareSize / 2;
        
        for (int y = centerY - halfSquare; y < centerY + halfSquare; y++)
        {
            for (int x = centerX - halfSquare; x < centerX + halfSquare; x++)
            {
                if (x >= 0 && x < imageSize && y >= 0 && y < imageSize)
                {
                    int index = y * imageSize + x;
                    imageData[index] = signalValue;
                }
            }
        }
        
        // Act
        var stats = calculator.CalculateCentralSquareStatistics(
            imageData, signalGenerator, "Square", 0, squareSize);
        
        // Assert
        Assert.Equal(signalValue, stats.mean, precision: 6);
        Assert.Equal(0.0f, stats.std, precision: 6); // No variance in uniform signal
        Assert.True(stats.count > 0, "Should have signal pixels");
    }
    
    [Fact]
    public void CalculateCentralSquareStatistics_NoSignalPattern_ShouldReturnZero()
    {
        // Arrange
        var calculator = new StatisticsCalculator();
        var signalGenerator = new SignalGenerator();
        float[] imageData = new float[1024 * 1024]; // Must be 1024x1024
        
        // Act
        var stats = calculator.CalculateCentralSquareStatistics(
            imageData, signalGenerator, "No Signal", 0, 10);
        
        // Assert
        Assert.Equal(0.0f, stats.mean);
        Assert.Equal(0.0f, stats.std);
        Assert.Equal(0, stats.count);
    }
    
    [Fact]
    public void CalculateSignalToNoiseRatio_ValidData_ShouldCalculateCorrectly()
    {
        // Arrange
        var calculator = new StatisticsCalculator();
        var signalStats = (mean: 200.0f, std: 10.0f, count: 100);
        var backgroundStats = (mean: 100.0f, std: 5.0f, count: 1000);
        
        // Act
        float snr = calculator.CalculateSignalToNoiseRatio(signalStats, backgroundStats);
        
        // Assert
        float expectedSNR = (200.0f - 100.0f) / 5.0f; // (signal - background) / background_std
        Assert.Equal(expectedSNR, snr, precision: 6);
    }
    
    [Fact]
    public void CalculateSignalToNoiseRatio_ZeroBackgroundStd_ShouldReturnZero()
    {
        // Arrange
        var calculator = new StatisticsCalculator();
        var signalStats = (mean: 200.0f, std: 10.0f, count: 100);
        var backgroundStats = (mean: 100.0f, std: 0.0f, count: 1000);
        
        // Act
        float snr = calculator.CalculateSignalToNoiseRatio(signalStats, backgroundStats);
        
        // Assert
        Assert.Equal(0.0f, snr);
    }
    
    [Fact]
    public void GetSourceSignalFlux_ValidSquare_ShouldReturnCorrectValue()
    {
        // Arrange
        var calculator = new StatisticsCalculator();
        var signalGenerator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        
        // Act
        float flux = calculator.GetSourceSignalFlux(0, "Square", baseSignalFlux, signalGenerator);
        
        // Assert
        Assert.Equal(baseSignalFlux, flux, precision: 6);
    }
} 