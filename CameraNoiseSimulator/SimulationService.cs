namespace NoiseSimulator;

/// <summary>
/// Main service for orchestrating camera noise simulations
/// </summary>
public class SimulationService
{
    private readonly SimulationConfig _config;
    private readonly IImageProcessor _imageProcessor;
    private readonly StatisticsService _statisticsService;
    private readonly ImageExporterFactory _exporterFactory;
    
    public SimulationService(SimulationConfig? config = null)
    {
        _config = config ?? SimulationConfig.Default;
        _imageProcessor = new ImageProcessor(_config);
        _statisticsService = new StatisticsService(_config);
        _exporterFactory = new ImageExporterFactory(_config);
    }
    
    /// <summary>
    /// Runs a complete simulation with the specified parameters
    /// </summary>
    public SimulationResult RunSimulation(SimulationParameters parameters)
    {
        // Generate the image
        uint[,] imageData = _imageProcessor.GenerateImage(
            parameters.BackgroundFlux,
            parameters.SignalFlux,
            parameters.SignalPattern,
            parameters.ExposureTime,
            parameters.ReadNoise,
            parameters.Seed,
            parameters.SquareSize,
            parameters.UseVerticalLines);
        
        // Calculate statistics
        var statistics = _statisticsService.CalculateImageStatistics(
            imageData,
            parameters.SignalPattern,
            parameters.SquareSize,
            parameters.UseVerticalLines,
            (float)parameters.SignalFlux);
        
        return new SimulationResult
        {
            ImageData = imageData,
            Statistics = statistics,
            Parameters = parameters,
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Runs a simulation with multiple exposures and averaging
    /// </summary>
    public SimulationResult RunAveragedSimulation(SimulationParameters parameters, int numExposures)
    {
        // Generate averaged image
        uint[,] imageData = _imageProcessor.GenerateAveragedImage(
            numExposures,
            parameters.BackgroundFlux,
            parameters.SignalFlux,
            parameters.SignalPattern,
            parameters.ExposureTime,
            parameters.ReadNoise,
            parameters.Seed,
            parameters.SquareSize,
            parameters.UseVerticalLines);
        
        // Calculate statistics
        var statistics = _statisticsService.CalculateImageStatistics(
            imageData,
            parameters.SignalPattern,
            parameters.SquareSize,
            parameters.UseVerticalLines,
            (float)parameters.SignalFlux);
        
        return new SimulationResult
        {
            ImageData = imageData,
            Statistics = statistics,
            Parameters = parameters,
            Timestamp = DateTime.UtcNow,
            NumExposures = numExposures
        };
    }
    
    /// <summary>
    /// Exports simulation results to the specified format
    /// </summary>
    public void ExportResults(SimulationResult result, string filePath, string format)
    {
        if (!_exporterFactory.IsFormatSupported(format))
            throw new ArgumentException($"Unsupported export format: {format}");
        
        var exporter = _exporterFactory.CreateExporter(format);
        exporter.ExportImage(filePath, result.ImageData, format);
    }
    
    /// <summary>
    /// Gets the current configuration
    /// </summary>
    public SimulationConfig GetConfiguration() => _config;
}

/// <summary>
/// Parameters for a camera noise simulation
/// </summary>
public class SimulationParameters
{
    public double BackgroundFlux { get; set; } = 5.0;
    public double SignalFlux { get; set; } = 50.0;
    public string SignalPattern { get; set; } = "Square";
    public double ExposureTime { get; set; } = 1.0;
    public double ReadNoise { get; set; } = 1.0;
    public int Seed { get; set; } = 42;
    public int SquareSize { get; set; } = 20;
    public bool UseVerticalLines { get; set; } = false;
    
    public static SimulationParameters Default => new();
    
    public static SimulationParameters CreateCustom(
        double backgroundFlux = 5.0,
        double signalFlux = 50.0,
        string signalPattern = "Square",
        double exposureTime = 1.0,
        double readNoise = 1.0,
        int seed = 42,
        int squareSize = 20,
        bool useVerticalLines = false)
    {
        return new SimulationParameters
        {
            BackgroundFlux = backgroundFlux,
            SignalFlux = signalFlux,
            SignalPattern = signalPattern,
            ExposureTime = exposureTime,
            ReadNoise = readNoise,
            Seed = seed,
            SquareSize = squareSize,
            UseVerticalLines = useVerticalLines
        };
    }
}

/// <summary>
/// Results of a camera noise simulation
/// </summary>
public class SimulationResult
{
    public uint[,] ImageData { get; set; } = new uint[0, 0];
    public ImageStatistics Statistics { get; set; } = new();
    public SimulationParameters Parameters { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public int? NumExposures { get; set; }
    
    public override string ToString()
    {
        string exposureInfo = NumExposures.HasValue ? $" ({NumExposures.Value} exposures)" : "";
        return $"Simulation Result{exposureInfo} - {Timestamp:yyyy-MM-dd HH:mm:ss}\n{Statistics}";
    }
} 