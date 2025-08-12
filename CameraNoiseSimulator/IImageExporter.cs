namespace NoiseSimulator;

/// <summary>
/// Interface for exporting astronomical image data to various formats
/// </summary>
public interface IImageExporter
{
    /// <summary>
    /// Exports image data to the specified format
    /// </summary>
    void ExportImage(string filePath, uint[,] imageData, string format);
    
    /// <summary>
    /// Gets the supported export formats
    /// </summary>
    string[] GetSupportedFormats();
    
    /// <summary>
    /// Gets the file extension for a given format
    /// </summary>
    string GetFileExtension(string format);
} 