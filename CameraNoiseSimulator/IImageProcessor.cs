namespace NoiseSimulator;

/// <summary>
/// Interface for processing astronomical image data
/// </summary>
public interface IImageProcessor
{
    /// <summary>
    /// Generates a complete image with background, signal, and noise
    /// </summary>
    uint[,] GenerateImage(
        double backgroundFlux, 
        double signalFlux, 
        string signalPattern, 
        double exposureTime, 
        double readNoise, 
        int seed, 
        int squareSize = 20, 
        bool useVerticalLines = false);
    
    /// <summary>
    /// Generates multiple exposures and averages them
    /// </summary>
    uint[,] GenerateAveragedImage(
        int numExposures,
        double backgroundFlux, 
        double signalFlux, 
        string signalPattern, 
        double exposureTime, 
        double readNoise, 
        int seed, 
        int squareSize = 20, 
        bool useVerticalLines = false);
} 