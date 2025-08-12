namespace NoiseSimulator;

/// <summary>
/// Service for calculating astronomical image statistics
/// </summary>
public class StatisticsService
{
    private readonly StatisticsCalculator _calculator;
    private readonly SimulationConfig _config;
    
    public StatisticsService(SimulationConfig? config = null)
    {
        _calculator = new StatisticsCalculator();
        _config = config ?? SimulationConfig.Default;
    }
    
    /// <summary>
    /// Calculates comprehensive statistics for an image
    /// </summary>
    public ImageStatistics CalculateImageStatistics(
        uint[,] imageData, 
        string signalPattern, 
        int squareSize = 20, 
        bool useVerticalLines = false, 
        float signalFlux = 0.0f)
    {
        // Convert to float array for statistics calculation
        float[] imageDataFloat = ConvertToFloatArray(imageData);
        
        var backgroundStats = _calculator.CalculateBackgroundStatistics(
            imageDataFloat, new SignalGenerator(), signalPattern, squareSize, useVerticalLines, signalFlux,
            imageData.GetLength(1), imageData.GetLength(0));
        
        var signalStats = _calculator.CalculateCentralSquareStatistics(
            imageDataFloat, new SignalGenerator(), signalPattern, 0, squareSize,
            imageData.GetLength(1), imageData.GetLength(0));
        
        float snr = _calculator.CalculateSignalToNoiseRatio(signalStats, backgroundStats);
        
        return new ImageStatistics
        {
            Background = backgroundStats,
            Signal = signalStats,
            SignalToNoiseRatio = snr,
            ImageWidth = imageData.GetLength(1),
            ImageHeight = imageData.GetLength(0)
        };
    }
    
    /// <summary>
    /// Calculates statistics for multiple exposures
    /// </summary>
    public ImageStatistics CalculateAveragedImageStatistics(
        uint[][,] exposures, 
        string signalPattern, 
        int squareSize = 20, 
        bool useVerticalLines = false, 
        float signalFlux = 0.0f)
    {
        if (exposures == null || exposures.Length == 0)
            throw new ArgumentException("Exposures array cannot be null or empty");
        
        // Average the exposures
        uint[,] averagedImage = AverageExposures(exposures);
        
        return CalculateImageStatistics(averagedImage, signalPattern, squareSize, useVerticalLines, signalFlux);
    }
    
    private float[] ConvertToFloatArray(uint[,] imageData)
    {
        int height = imageData.GetLength(0);
        int width = imageData.GetLength(1);
        
        float[] result = new float[height * width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                result[y * width + x] = imageData[y, x];
            }
        }
        return result;
    }
    
    private uint[,] AverageExposures(uint[][,] exposures)
    {
        int height = exposures[0].GetLength(0);
        int width = exposures[0].GetLength(1);
        int numExposures = exposures.Length;
        
        uint[,] averagedImage = new uint[height, width];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                long sum = 0;
                for (int exp = 0; exp < numExposures; exp++)
                {
                    sum += exposures[exp][y, x];
                }
                averagedImage[y, x] = (uint)(sum / numExposures);
            }
        }
        
        return averagedImage;
    }
}

/// <summary>
/// Comprehensive statistics for an astronomical image
/// </summary>
public class ImageStatistics
{
    public (float mean, float std, int count) Background { get; set; }
    public (float mean, float std, int count) Signal { get; set; }
    public float SignalToNoiseRatio { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    
    public override string ToString()
    {
        return $"Image Statistics ({ImageWidth}x{ImageHeight}):\n" +
               $"  Background: mean={Background.mean.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}, std={Background.std.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}, count={Background.count}\n" +
               $"  Signal: mean={Signal.mean.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}, std={Signal.std.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}, count={Signal.count}\n" +
               $"  Signal-to-Noise Ratio: {SignalToNoiseRatio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}";
    }
} 