namespace NoiseSimulator;

/// <summary>
/// Factory for creating image exporters based on format
/// </summary>
public class ImageExporterFactory
{
    private readonly SimulationConfig _config;
    
    public ImageExporterFactory(SimulationConfig? config = null)
    {
        _config = config ?? SimulationConfig.Default;
    }
    
    /// <summary>
    /// Creates an image exporter for the specified format
    /// </summary>
    public IImageExporter CreateExporter(string format)
    {
        return format.ToUpper() switch
        {
            "FITS" => new FitsWriter(_config),
            "fits" => new FitsWriter(_config),
            _ => throw new ArgumentException($"Unsupported export format: {format}")
        };
    }
    
    /// <summary>
    /// Gets all supported export formats
    /// </summary>
    public string[] GetSupportedFormats()
    {
        return new[] { "FITS", "fits" };
    }
    
    /// <summary>
    /// Checks if a format is supported
    /// </summary>
    public bool IsFormatSupported(string format)
    {
        return GetSupportedFormats().Contains(format.ToUpper());
    }
    
    /// <summary>
    /// Gets the file extension for a given format
    /// </summary>
    public string GetFileExtension(string format)
    {
        return ".fits";
    }
} 