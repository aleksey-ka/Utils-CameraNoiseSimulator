using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class ConfigurationTests
{
    [Fact]
    public void SimulationConfig_Constructor_ShouldCreateInstance()
    {
        // Act
        var config = new SimulationConfig();
        
        // Assert
        Assert.NotNull(config);
    }
    
    [Fact]
    public void SimulationConfig_Instances_ShouldBeIndependent()
    {
        // Act
        var config1 = new SimulationConfig();
        var config2 = new SimulationConfig();
        
        // Assert
        Assert.NotSame(config1, config2);
    }
} 