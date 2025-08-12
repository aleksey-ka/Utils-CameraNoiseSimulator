using System;
using System.Collections.Generic;
using System.Linq;

namespace NoiseSimulator;

/// <summary>
/// Handles calculation of statistics for photon detection simulation
/// </summary>
public class StatisticsCalculator
{
    /// <summary>
    /// Calculates statistics for background pixels (where signal flux is zero)
    /// </summary>
    /// <param name="detectedArrayFloats">Array of detected photon values (corrected for offset)</param>
    /// <param name="signalGenerator">SignalGenerator instance for pattern detection</param>
    /// <param name="pattern">Current pattern name</param>
    /// <param name="squareSize">Size of squares in pixels</param>
    /// <param name="useVerticalLines">Whether to use vertical lines pattern</param>
    /// <param name="baseSignalFlux">Base signal flux value</param>
    /// <param name="imageWidth">Width of the image</param>
    /// <param name="imageHeight">Height of the image</param>
    /// <returns>Tuple of (mean, std, count) for background pixels only</returns>
    public (float mean, float std, int count) CalculateBackgroundStatistics(
        float[] detectedArrayFloats, 
        SignalGenerator signalGenerator,
        string pattern,
        int squareSize,
        bool useVerticalLines,
        float baseSignalFlux,
        int imageWidth = 1024,
        int imageHeight = 1024)
    {
        // Collect background pixel values (where signal flux is zero)
        var backgroundValues = new List<float>();
        
        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                int index = y * imageWidth + x;
                
                // Check if this pixel has zero signal flux
                float signalFlux = signalGenerator.GetPatternSignalFlux(x, y, baseSignalFlux, pattern, squareSize, useVerticalLines);
                if (signalFlux == 0)
                {
                    // This is a background pixel, include it in statistics
                    backgroundValues.Add(detectedArrayFloats[index]);
                }
            }
        }
        
        // Calculate statistics for background pixels only
        if (backgroundValues.Count == 0)
        {
            return (0, 0, 0); // No background pixels found
        }
        
        float mean = backgroundValues.Average();
        float variance = backgroundValues.Average(v => (v - mean) * (v - mean));
        float std = MathF.Sqrt(variance);
        
        return (mean, std, backgroundValues.Count);
    }

    /// <summary>
    /// Calculates statistics for the selected square signal area
    /// </summary>
    /// <param name="detectedArrayFloats">Array of detected photon values (corrected for offset)</param>
    /// <param name="signalGenerator">SignalGenerator instance for pattern detection</param>
    /// <param name="pattern">Current pattern name</param>
    /// <param name="selectedSquareIndex">Index of the selected square</param>
    /// <param name="squareSize">Size of squares in pixels</param>
    /// <param name="imageWidth">Width of the image</param>
    /// <param name="imageHeight">Height of the image</param>
    /// <returns>Tuple of (mean, std, count) for selected square pixels only</returns>
    public (float mean, float std, int count) CalculateCentralSquareStatistics(
        float[] detectedArrayFloats,
        SignalGenerator signalGenerator,
        string pattern,
        int selectedSquareIndex,
        int squareSize,
        int imageWidth = 1024,
        int imageHeight = 1024)
    {
        // Check if "No Signal" pattern is selected
        if (pattern == "No Signal")
        {
            // For "No Signal" pattern, return zero statistics
            return (0, 0, 0);
        }
        
        // Determine the maximum valid square index for the current pattern
        int maxValidIndex = signalGenerator.GetMaxValidIndex(pattern);
        
        // Check if the selected square index is valid for the current pattern
        if (selectedSquareIndex > maxValidIndex)
        {
            return (0, 0, 0); // Invalid square index - return zero statistics
        }
        
        // Define square parameters - center the pattern in the image
        int centerX = imageWidth / 2;
        int centerY = imageHeight / 2;
        const int gap = 30;
        int halfSquare = squareSize / 2;
        
        // Calculate the position of the selected square
        int squareX = centerX + signalGenerator.SpiralPatternDx[selectedSquareIndex] * (squareSize + gap);
        int squareY = centerY + signalGenerator.SpiralPatternDy[selectedSquareIndex] * (squareSize + gap);
        
        // Collect selected square pixel values
        var signalValues = new List<float>();
        
        for (int y = squareY - halfSquare; y < squareY + halfSquare; y++)
        {
            for (int x = squareX - halfSquare; x < squareX + halfSquare; x++)
            {
                if (x >= 0 && x < imageWidth && y >= 0 && y < imageHeight)
                {
                    int index = y * imageWidth + x;
                    signalValues.Add(detectedArrayFloats[index]);
                }
            }
        }
        
        // Calculate statistics for selected square pixels only
        if (signalValues.Count == 0)
        {
            return (0, 0, 0); // No signal pixels found
        }
        
        float mean = signalValues.Average();
        float variance = signalValues.Average(v => (v - mean) * (v - mean));
        float std = MathF.Sqrt(variance);
        
        return (mean, std, signalValues.Count);
    }

    /// <summary>
    /// Calculates signal-to-noise ratio
    /// </summary>
    /// <param name="signalStats">Signal statistics (mean, std, count)</param>
    /// <param name="backgroundStats">Background statistics (mean, std, count)</param>
    /// <returns>Signal-to-noise ratio</returns>
    public float CalculateSignalToNoiseRatio((float mean, float std, int count) signalStats, (float mean, float std, int count) backgroundStats)
    {
        if (backgroundStats.std > 0)
        {
            return (signalStats.mean - backgroundStats.mean) / backgroundStats.std;
        }
        return 0;
    }

    /// <summary>
    /// Gets the source signal flux for a selected square
    /// </summary>
    /// <param name="selectedSquareIndex">Index of the selected square</param>
    /// <param name="pattern">Pattern name</param>
    /// <param name="baseSignalFlux">Base signal flux value</param>
    /// <param name="signalGenerator">SignalGenerator instance</param>
    /// <returns>Source signal flux for the selected square</returns>
    public float GetSourceSignalFlux(int selectedSquareIndex, string pattern, float baseSignalFlux, SignalGenerator signalGenerator)
    {
        return signalGenerator.GetSourceSignalFlux(selectedSquareIndex, pattern, baseSignalFlux);
    }
} 