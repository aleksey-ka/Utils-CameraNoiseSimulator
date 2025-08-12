using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class StatisticsServiceTests
{
    [Fact]
    public void StatisticsService_Constructor_ShouldUseDefaultConfig()
    {
        // Act
        var service = new StatisticsService();
        
        // Assert
        Assert.NotNull(service);
    }
    
    [Fact]
    public void StatisticsService_Constructor_ShouldUseCustomConfig()
    {
        // Arrange
        var config = SimulationConfig.CreateCustom(512, 512);
        
        // Act
        var service = new StatisticsService(config);
        
        // Assert
        Assert.NotNull(service);
    }
    
    [Fact]
    public void StatisticsService_CalculateImageStatistics_ShouldCalculateCorrectStatistics()
    {
        // Arrange
        var service = new StatisticsService();
        uint[,] imageData = CreateTestImage(256, 256);
        
        // Act
        var statistics = service.CalculateImageStatistics(
            imageData, "Square", 20, false, 100.0f);
        
        // Assert
        Assert.NotNull(statistics);
        Assert.Equal(256, statistics.ImageWidth);
        Assert.Equal(256, statistics.ImageHeight);
        Assert.True(statistics.Background.count > 0);
        Assert.True(statistics.Signal.count > 0);
        Assert.True(statistics.SignalToNoiseRatio > 0);
    }
    
    [Fact]
    public void StatisticsService_CalculateImageStatistics_ShouldHandleNoSignalPattern()
    {
        // Arrange
        var service = new StatisticsService();
        uint[,] imageData = CreateTestImage(128, 128);
        
        // Act
        var statistics = service.CalculateImageStatistics(
            imageData, "No Signal", 20, false, 0.0f);
        
        // Assert
        Assert.NotNull(statistics);
        Assert.Equal(128, statistics.ImageWidth);
        Assert.Equal(128, statistics.ImageHeight);
        Assert.Equal(128 * 128, statistics.Background.count); // All pixels are background
        Assert.Equal(0, statistics.Signal.count); // No signal pattern
        Assert.Equal(0.0f, statistics.Signal.mean);
        Assert.Equal(0.0f, statistics.Signal.std);
    }
    
    [Fact]
    public void StatisticsService_CalculateAveragedImageStatistics_ShouldHandleMultipleExposures()
    {
        // Arrange
        var service = new StatisticsService();
        uint[][,] exposures = new uint[3][,];
        for (int i = 0; i < 3; i++)
        {
            exposures[i] = CreateTestImage(64, 64);
        }
        
        // Act
        var statistics = service.CalculateAveragedImageStatistics(
            exposures, "Square", 20, false, 50.0f);
        
        // Assert
        Assert.NotNull(statistics);
        Assert.Equal(64, statistics.ImageWidth);
        Assert.Equal(64, statistics.ImageHeight);
        Assert.True(statistics.Background.count > 0);
        Assert.True(statistics.Signal.count > 0);
    }
    
    [Fact]
    public void StatisticsService_CalculateAveragedImageStatistics_ShouldThrowOnNullExposures()
    {
        // Arrange
        var service = new StatisticsService();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            service.CalculateAveragedImageStatistics(null!, "Square", 20, false, 50.0f));
    }
    
    [Fact]
    public void StatisticsService_CalculateAveragedImageStatistics_ShouldThrowOnEmptyExposures()
    {
        // Arrange
        var service = new StatisticsService();
        uint[][,] emptyExposures = new uint[0][,];
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            service.CalculateAveragedImageStatistics(emptyExposures, "Square", 20, false, 50.0f));
    }
    
    [Fact]
    public void ImageStatistics_ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var backgroundStats = (100.0f, 10.0f, 1000);
        var signalStats = (200.0f, 20.0f, 100);
        
        var statistics = new ImageStatistics
        {
            Background = backgroundStats,
            Signal = signalStats,
            SignalToNoiseRatio = 5.0f,
            ImageWidth = 256,
            ImageHeight = 256
        };
        
        // Verify the object is properly initialized
        Assert.Equal(100.0f, statistics.Background.mean);
        Assert.Equal(200.0f, statistics.Signal.mean);
        
        // Act
        string result = statistics.ToString();
        
        // Debug: Output the actual string to see the exact format
        Console.WriteLine($"DEBUG - Actual ToString output: '{result}'");
        Console.WriteLine($"DEBUG - String length: {result.Length}");
        Console.WriteLine($"DEBUG - Background.mean = {statistics.Background.mean}");
        Console.WriteLine($"DEBUG - Signal.mean = {statistics.Signal.mean}");
        Console.WriteLine($"DEBUG - Contains 'Background:': {result.Contains("Background:")}");
        Console.WriteLine($"DEBUG - Contains 'mean=100.00': {result.Contains("mean=100.00")}");
        
        // Let's test the exact expected output
        string expectedOutput = $"Image Statistics (256x256):\n" +
                               $"  Background: mean=100.00, std=10.00, count=1000\n" +
                               $"  Signal: mean=200.00, std=20.00, count=100\n" +
                               $"  Signal-to-Noise Ratio: 5.00";
        
        Console.WriteLine($"DEBUG - Expected output: '{expectedOutput}'");
        Console.WriteLine($"DEBUG - Expected length: {expectedOutput.Length}");
        Console.WriteLine($"DEBUG - Outputs match: {result == expectedOutput}");
        
        // Assert
        Assert.Equal(expectedOutput, result);
    }
    
    private uint[,] CreateTestImage(int width, int height)
    {
        uint[,] image = new uint[height, width];
        var random = new Random(42);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a simple test pattern
                if (x >= width / 4 && x < 3 * width / 4 && 
                    y >= height / 4 && y < 3 * height / 4)
                {
                    image[y, x] = (uint)(100 + random.Next(50)); // Signal region
                }
                else
                {
                    image[y, x] = (uint)(10 + random.Next(20)); // Background region
                }
            }
        }
        
        return image;
    }
} 