namespace NoiseSimulator;

/// <summary>
/// Centralized configuration for camera noise simulation parameters
/// </summary>
public class SimulationConfig
{
    public int ImageWidth { get; set; } = 1024;
    public int ImageHeight { get; set; } = 1024;
    public double DefaultBackgroundFlux { get; set; } = 5.0;
    public double DefaultSignalFlux { get; set; } = 50.0;
    public double DefaultExposureTime { get; set; } = 1.0;
    public double DefaultReadNoise { get; set; } = 1.0;
    public int DefaultSeed { get; set; } = 42;
    public int DefaultSquareSize { get; set; } = 20;
    
    public static SimulationConfig Default => new();
    
    public static SimulationConfig CreateCustom(
        int width, int height, 
        double backgroundFlux = 5.0, 
        double signalFlux = 50.0,
        double exposureTime = 1.0,
        double readNoise = 1.0,
        int seed = 42,
        int squareSize = 20)
    {
        return new SimulationConfig
        {
            ImageWidth = width,
            ImageHeight = height,
            DefaultBackgroundFlux = backgroundFlux,
            DefaultSignalFlux = signalFlux,
            DefaultExposureTime = exposureTime,
            DefaultReadNoise = readNoise,
            DefaultSeed = seed,
            DefaultSquareSize = squareSize
        };
    }
} 