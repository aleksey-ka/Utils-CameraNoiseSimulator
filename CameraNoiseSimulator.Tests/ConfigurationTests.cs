using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class ConfigurationTests
{
    [Fact]
    public void SimulationConfig_Default_ShouldHaveExpectedValues()
    {
        // Act
        var config = SimulationConfig.Default;
        
        // Assert
        Assert.Equal(1024, config.ImageWidth);
        Assert.Equal(1024, config.ImageHeight);
        Assert.Equal(5.0, config.DefaultBackgroundFlux);
        Assert.Equal(50.0, config.DefaultSignalFlux);
        Assert.Equal(1.0, config.DefaultExposureTime);
        Assert.Equal(1.0, config.DefaultReadNoise);
        Assert.Equal(42, config.DefaultSeed);
        Assert.Equal(20, config.DefaultSquareSize);
    }
    
    [Fact]
    public void SimulationConfig_CreateCustom_ShouldSetCustomValues()
    {
        // Arrange
        const int width = 2048;
        const int height = 2048;
        const double backgroundFlux = 10.0;
        const double signalFlux = 100.0;
        const double exposureTime = 5.0;
        const double readNoise = 2.0;
        const int seed = 123;
        const int squareSize = 40;
        
        // Act
        var config = SimulationConfig.CreateCustom(
            width, height, backgroundFlux, signalFlux, 
            exposureTime, readNoise, seed, squareSize);
        
        // Assert
        Assert.Equal(width, config.ImageWidth);
        Assert.Equal(height, config.ImageHeight);
        Assert.Equal(backgroundFlux, config.DefaultBackgroundFlux);
        Assert.Equal(signalFlux, config.DefaultSignalFlux);
        Assert.Equal(exposureTime, config.DefaultExposureTime);
        Assert.Equal(readNoise, config.DefaultReadNoise);
        Assert.Equal(seed, config.DefaultSeed);
        Assert.Equal(squareSize, config.DefaultSquareSize);
    }
    
    [Fact]
    public void SimulationConfig_CreateCustom_ShouldUseDefaultValuesForOptionalParameters()
    {
        // Act
        var config = SimulationConfig.CreateCustom(512, 512);
        
        // Assert
        Assert.Equal(512, config.ImageWidth);
        Assert.Equal(512, config.ImageHeight);
        Assert.Equal(5.0, config.DefaultBackgroundFlux); // Default
        Assert.Equal(50.0, config.DefaultSignalFlux); // Default
        Assert.Equal(1.0, config.DefaultExposureTime); // Default
        Assert.Equal(1.0, config.DefaultReadNoise); // Default
        Assert.Equal(42, config.DefaultSeed); // Default
        Assert.Equal(20, config.DefaultSquareSize); // Default
    }
} 