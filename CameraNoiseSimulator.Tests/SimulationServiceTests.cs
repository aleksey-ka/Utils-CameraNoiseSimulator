using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class SimulationServiceTests
{
    [Fact]
    public void SimulationService_Constructor_ShouldUseDefaultConfig()
    {
        // Act
        var service = new SimulationService();
        
        // Assert
        Assert.NotNull(service);
    }
    
    [Fact]
    public void SimulationService_Constructor_ShouldUseCustomConfig()
    {
        // Arrange
        var config = SimulationConfig.CreateCustom(512, 512);
        
        // Act
        var service = new SimulationService(config);
        
        // Assert
        Assert.NotNull(service);
    }
    
    [Fact]
    public void SimulationService_RunSimulation_ShouldGenerateCompleteResult()
    {
        // Arrange
        var service = new SimulationService();
        var parameters = SimulationParameters.CreateCustom(
            backgroundFlux: 5.0,
            signalFlux: 50.0,
            signalPattern: "Square",
            exposureTime: 1.0,
            readNoise: 1.0,
            seed: 42);
        
        // Act
        var result = service.RunSimulation(parameters);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ImageData);
        Assert.NotNull(result.Statistics);
        Assert.Equal(parameters, result.Parameters);
        Assert.True(result.Timestamp > DateTime.UtcNow.AddMinutes(-1)); // Recent timestamp
        Assert.Null(result.NumExposures); // Single exposure
        
        // Image should have correct dimensions
        Assert.Equal(1024, result.ImageData.GetLength(0));
        Assert.Equal(1024, result.ImageData.GetLength(1));
        
        // Statistics should be calculated
        Assert.True(result.Statistics.Background.count > 0);
        Assert.True(result.Statistics.Signal.count > 0);
        Assert.True(result.Statistics.SignalToNoiseRatio > 0);
    }
    
    [Fact]
    public void SimulationService_RunAveragedSimulation_ShouldGenerateAveragedResult()
    {
        // Arrange
        var service = new SimulationService();
        var parameters = SimulationParameters.CreateCustom(
            backgroundFlux: 5.0,
            signalFlux: 50.0,
            signalPattern: "Square",
            exposureTime: 1.0,
            readNoise: 1.0,
            seed: 42);
        const int numExposures = 4;
        
        // Act
        var result = service.RunAveragedSimulation(parameters, numExposures);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ImageData);
        Assert.NotNull(result.Statistics);
        Assert.Equal(parameters, result.Parameters);
        Assert.Equal(numExposures, result.NumExposures);
        
        // Image should have correct dimensions
        Assert.Equal(1024, result.ImageData.GetLength(0));
        Assert.Equal(1024, result.ImageData.GetLength(1));
    }
    
    [Fact]
    public void SimulationService_GetConfiguration_ShouldReturnCurrentConfig()
    {
        // Arrange
        var config = SimulationConfig.CreateCustom(512, 512);
        var service = new SimulationService(config);
        
        // Act
        var returnedConfig = service.GetConfiguration();
        
        // Assert
        Assert.Equal(config, returnedConfig);
        Assert.Equal(512, returnedConfig.ImageWidth);
        Assert.Equal(512, returnedConfig.ImageHeight);
    }
    
    [Fact]
    public void SimulationParameters_Default_ShouldHaveExpectedValues()
    {
        // Act
        var parameters = SimulationParameters.Default;
        
        // Assert
        Assert.Equal(5.0, parameters.BackgroundFlux);
        Assert.Equal(50.0, parameters.SignalFlux);
        Assert.Equal("Square", parameters.SignalPattern);
        Assert.Equal(1.0, parameters.ExposureTime);
        Assert.Equal(1.0, parameters.ReadNoise);
        Assert.Equal(42, parameters.Seed);
        Assert.Equal(20, parameters.SquareSize);
        Assert.False(parameters.UseVerticalLines);
    }
    
    [Fact]
    public void SimulationParameters_CreateCustom_ShouldSetCustomValues()
    {
        // Arrange
        const double backgroundFlux = 10.0;
        const double signalFlux = 100.0;
        const string signalPattern = "3x3 Squares";
        const double exposureTime = 5.0;
        const double readNoise = 2.0;
        const int seed = 123;
        const int squareSize = 40;
        const bool useVerticalLines = true;
        
        // Act
        var parameters = SimulationParameters.CreateCustom(
            backgroundFlux, signalFlux, signalPattern, exposureTime, 
            readNoise, seed, squareSize, useVerticalLines);
        
        // Assert
        Assert.Equal(backgroundFlux, parameters.BackgroundFlux);
        Assert.Equal(signalFlux, parameters.SignalFlux);
        Assert.Equal(signalPattern, parameters.SignalPattern);
        Assert.Equal(exposureTime, parameters.ExposureTime);
        Assert.Equal(readNoise, parameters.ReadNoise);
        Assert.Equal(seed, parameters.Seed);
        Assert.Equal(squareSize, parameters.SquareSize);
        Assert.Equal(useVerticalLines, parameters.UseVerticalLines);
    }
    
    [Fact]
    public void SimulationParameters_CreateCustom_ShouldUseDefaultValuesForOptionalParameters()
    {
        // Act
        var parameters = SimulationParameters.CreateCustom(
            backgroundFlux: 15.0,
            signalFlux: 75.0);
        
        // Assert
        Assert.Equal(15.0, parameters.BackgroundFlux);
        Assert.Equal(75.0, parameters.SignalFlux);
        Assert.Equal("Square", parameters.SignalPattern); // Default
        Assert.Equal(1.0, parameters.ExposureTime); // Default
        Assert.Equal(1.0, parameters.ReadNoise); // Default
        Assert.Equal(42, parameters.Seed); // Default
        Assert.Equal(20, parameters.SquareSize); // Default
        Assert.False(parameters.UseVerticalLines); // Default
    }
    
    [Fact]
    public void SimulationResult_ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var backgroundStats = (100.0f, 10.0f, 1000);
        var signalStats = (200.0f, 20.0f, 100);
        
        var result = new SimulationResult
        {
            ImageData = new uint[256, 256],
            Statistics = new ImageStatistics
            {
                Background = backgroundStats,
                Signal = signalStats,
                SignalToNoiseRatio = 5.0f,
                ImageWidth = 256,
                ImageHeight = 256
            },
            Parameters = SimulationParameters.Default,
            Timestamp = new DateTime(2024, 1, 1, 12, 0, 0),
            NumExposures = 4
        };
        
        // Verify the object is properly initialized
        Assert.Equal(100.0f, result.Statistics.Background.mean);
        Assert.Equal(200.0f, result.Statistics.Signal.mean);
        
        // Act
        string resultString = result.ToString();
        
        // Debug: Output the actual string to see the exact format
        Console.WriteLine($"DEBUG - Actual ToString output: '{resultString}'");
        Console.WriteLine($"DEBUG - String length: {resultString.Length}");
        Console.WriteLine($"DEBUG - Background.mean = {result.Statistics.Background.mean}");
        Console.WriteLine($"DEBUG - Signal.mean = {result.Statistics.Signal.mean}");
        Console.WriteLine($"DEBUG - Contains 'Background:': {resultString.Contains("Background:")}");
        Console.WriteLine($"DEBUG - Contains 'mean=100.00': {resultString.Contains("mean=100.00")}");
        
        // Let's test the exact expected output
        string expectedOutput = $"Simulation Result (4 exposures) - 2024-01-01 12:00:00\n" +
                               $"Image Statistics (256x256):\n" +
                               $"  Background: mean=100.00, std=10.00, count=1000\n" +
                               $"  Signal: mean=200.00, std=20.00, count=100\n" +
                               $"  Signal-to-Noise Ratio: 5.00";
        
        Console.WriteLine($"DEBUG - Expected output: '{expectedOutput}'");
        Console.WriteLine($"DEBUG - Expected length: {expectedOutput.Length}");
        Console.WriteLine($"DEBUG - Outputs match: {resultString == expectedOutput}");
        
        // Assert
        Assert.Equal(expectedOutput, resultString);
    }
} 