using System;

namespace NoiseSimulator;

/// <summary>
/// Handles generation of signal patterns for photon detection simulation
/// </summary>
public class SignalGenerator
{
    public int[] SpiralPatternDx { get; } = { 0,  // 1 square
         0, -1, -1, -1, 0,  1,  1,  1, // 9 squares (3x3)
         1,  0, -1, -2, -2, -2, -2, -2, -1,  0,  1,  2,  2,  2,  2,  2, // 25 squares (5x5)
         2,  1,  0, -1, -2, -3, -3, -3, -3, -3, -3, -3, -2, -1,  0,  1,  2,  3,  3,  3,  3,  3,  3,  3, // 49 squares (7x7)
         3,  2,  1,  0, -1, -2, -3, -4, -4, -4, -4, -4, -4, -4, -4, -4, -3, -2, -1,  0,  1,  2,  3,  4,  4,  4,  4,  4,  4,  4,  4,  4 }; // 81 squares (9x9)
    public int[] SpiralPatternDy { get; } = { 0, // 1 square
        -1, -1,  0,  1,  1,  1,  0, -1, // 9 squares (3x3)
        -2, -2, -2, -2, -1,  0,  1,  2,  2,  2,  2,  2,  1,  0, -1, -2, // 25 squares (5x5)
        -3,  -3,  -3,  -3,  -3,  -3,  -2,  -1,  0,  1,  2,  3,  3,  3,  3,  3,  3,  3,  2,  1, 0, -1, -2, -3, // 49 squares (7x7)
        -4, -4, -4, -4, -4, -4, -4, -4, -3, -2, -1,  0,  1,  2,  3,  4,  4,  4,  4,  4,  4,  4,  4,  4,  3,  2,  1,  0, -1, -2, -3, -4 }; // 81 squares (9x9)
    
    public double brightnessDecay = 0.891220943978059; // 1 magnitude every 8 steps of decaying brightness

    /// <summary>
    /// Gets the signal flux for a pixel based on the selected pattern
    /// </summary>
    /// <param name="x">Pixel x coordinate</param>
    /// <param name="y">Pixel y coordinate</param>
    /// <param name="baseSignalFlux">Base signal flux value</param>
    /// <param name="pattern">Pattern name</param>
    /// <param name="squareSize">Size of squares in pixels</param>
    /// <param name="useVerticalLines">Whether to use vertical lines instead of solid squares</param>
    /// <returns>Signal flux value for this pixel (0 if not in pattern)</returns>
    public float GetPatternSignalFlux(int x, int y, float baseSignalFlux, string pattern, int squareSize, bool useVerticalLines)
    {
        switch (pattern)
        {
            case "Square":
                return GetNSquaresSignalFlux(x, y, baseSignalFlux, 1, squareSize, useVerticalLines);
            case "3x3 Squares":
                return GetNSquaresSignalFlux(x, y, baseSignalFlux, 9, squareSize, useVerticalLines);
            case "5x5 Squares":
                return GetNSquaresSignalFlux(x, y, baseSignalFlux, 25, squareSize, useVerticalLines);
            case "7x7 Squares":
                return GetNSquaresSignalFlux(x, y, baseSignalFlux, 49, squareSize, useVerticalLines);
            case "9x9 Squares":
                return GetNSquaresSignalFlux(x, y, baseSignalFlux, 81, squareSize, useVerticalLines);
            case "Gaussian Spots":
                return GetGaussianSpotsSignalFlux(x, y, baseSignalFlux, squareSize);
            case "Continuous lines":
                return GetContinuousLinesSignalFlux(x, y, baseSignalFlux, squareSize, useVerticalLines);
            default:
                return 0.0f;
        }
    }

    /// <summary>
    /// Gets the signal flux for a pixel in an N-squares pattern with spiral diminishing brightness
    /// </summary>
    /// <param name="x">Pixel x coordinate</param>
    /// <param name="y">Pixel y coordinate</param>
    /// <param name="baseSignalFlux">Base signal flux value</param>
    /// <param name="n">Number of squares in the pattern (1, 9, 25, 49, or 81)</param>
    /// <param name="squareSize">Size of squares in pixels</param>
    /// <param name="useVerticalLines">Whether to use vertical lines instead of solid squares</param>
    /// <returns>Signal flux value for this pixel (0 if not in pattern)</returns>
    private float GetNSquaresSignalFlux(int x, int y, float baseSignalFlux, int n, int squareSize, bool useVerticalLines)
    {
        // Pre-calculate constants for better performance
        const int centerX = 1024 / 2;
        const int centerY = 1024 / 2;
        const int gap = 30;
        int halfSquare = squareSize / 2;
        
        double brightnessFactor = 1.0f;
        for (int i = 0; i < n; i++)
        {
            int squareX = centerX + SpiralPatternDx[i] * (squareSize + gap);
            int squareY = centerY + SpiralPatternDy[i] * (squareSize + gap);
            
            if (useVerticalLines)
            {
                // Vertical lines pattern: one pixel wide lines with one pixel gap
                int relativeX = x - (squareX - halfSquare);
                int relativeY = y - (squareY - halfSquare);
                
                // Check if within the square bounds
                if (relativeX >= 0 && relativeX < squareSize && 
                    relativeY >= 0 && relativeY < squareSize)
                {
                    // Vertical lines: every other pixel column has signal
                    if (relativeX % 2 == 0)
                    {
                        return (float)(baseSignalFlux * brightnessFactor);
                    }
                }
            }
            else
            {
                // Solid square pattern
                if (x >= squareX - halfSquare && x < squareX + halfSquare &&
                    y >= squareY - halfSquare && y < squareY + halfSquare)
                {
                    return (float)(baseSignalFlux * brightnessFactor);
                }
            }
            brightnessFactor *= brightnessDecay;
        }
        
        return 0; // Not in any square
    }

    /// <summary>
    /// Gets the signal flux for Gaussian spots pattern (astronomical objects)
    /// </summary>
    /// <param name="x">Pixel x coordinate</param>
    /// <param name="y">Pixel y coordinate</param>
    /// <param name="baseSignalFlux">Base signal flux value</param>
    /// <param name="squareSize">Size parameter (used as spot size multiplier)</param>
    /// <returns>Signal flux value for this pixel (0 if not in any spot)</returns>
    private float GetGaussianSpotsSignalFlux(int x, int y, float baseSignalFlux, int squareSize)
    {
        // Use the same placement and decay rules as 9x9 pattern
        const int centerX = 1024 / 2;
        const int centerY = 1024 / 2;
        const int gap = 30;
        const int n = 81; // Same as 9x9 pattern
        
        float totalFlux = 0.0f;
        double brightnessFactor = 1.0f;
        
        for (int i = 0; i < n; i++)
        {
            // Use the same placement as 9x9 pattern
            int spotX = centerX + SpiralPatternDx[i] * (squareSize + gap);
            int spotY = centerY + SpiralPatternDy[i] * (squareSize + gap);
            
            // Calculate distance from spot center
            float dx = x - spotX;
            float dy = y - spotY;
            float distanceSquared = dx * dx + dy * dy;
            
            // Gaussian function: A * exp(-(r^2) / (2 * sigma^2))
            // Use squareSize as sigma multiplier for size adjustment
            float sigma = squareSize * 0.1f; // Scale sigma with squareSize
            float sigmaSquared = sigma * sigma;
            double gaussian = brightnessFactor * MathF.Exp(-distanceSquared / (2.0f * sigmaSquared));
            
            totalFlux += (float)(gaussian * baseSignalFlux);
            
            // Apply the same decay as 9x9 pattern
            brightnessFactor *= brightnessDecay;
        }
        
        return totalFlux;
    }

    /// <summary>
    /// Gets the signal flux for continuous horizontal lines pattern
    /// 5 horizontal lines spanning the full width of the image
    /// Intensity decays from left to right, spanning 10 magnitudes total
    /// Each line continues from where the previous line ended
    /// </summary>
    /// <param name="x">Pixel x coordinate</param>
    /// <param name="y">Pixel y coordinate</param>
    /// <param name="baseSignalFlux">Base signal flux value</param>
    /// <param name="lineWidth">Width of each line in pixels</param>
    /// <param name="useVerticalLines">Whether to use vertical lines instead of solid squares</param>
    /// <returns>Signal flux value for this pixel (0 if not in any line)</returns>
    private float GetContinuousLinesSignalFlux(int x, int y, float baseSignalFlux, int lineWidth, bool useVerticalLines) 
    {
        const int imageWidth = 1024;
        const int imageHeight = 1024;
        const int centerY = imageHeight / 2;
        const int gap = 30; // Same gap as squares pattern
        
        // 5 horizontal lines: top to bottom (brightest to dimmest)
        int[] lineYPositions = {
            centerY - 3 * (lineWidth + gap), // Top line (brightest)
            centerY - 2 * (lineWidth + gap),
            centerY - 1 * (lineWidth + gap),
            centerY,                          // Center line
            centerY + 1 * (lineWidth + gap),
            centerY + 2 * (lineWidth + gap),
            centerY + 3 * (lineWidth + gap)  // Bottom line (dimmest)
        };

        double brightnessFactor = 1.0f;
        
        // Check if pixel is in any of the lines
        for (int lineIndex = 0; lineIndex < 7; lineIndex++)
        {
            int lineY = lineYPositions[lineIndex];
            
            // Check if pixel is within this line's vertical bounds
            if (y >= lineY && y < lineY + lineWidth)
            {
                // Calculate horizontal position within the line (0 to imageWidth-1)
                float relativeX = (float)x / (imageWidth - 1); // 0.0 to 1.0
                
                if(useVerticalLines && x % 2 != 0)
                {
                    return 0.0f;
                }
                else
                {
                    return (float)(baseSignalFlux * brightnessFactor * (1 - relativeX ));
                }
            }

            // Each next line starts 2 magnitudes dimmer than the previous line
            brightnessFactor *= Math.Pow( brightnessDecay, 8.0 );
        }
        
        return 0.0f; // Not in any line
    }

    /// <summary>
    /// Gets the maximum valid square index for a given pattern
    /// </summary>
    /// <param name="pattern">Pattern name</param>
    /// <returns>Maximum valid square index (0-based)</returns>
    public int GetMaxValidIndex(string pattern)
    {
        return pattern switch
        {
            "Square" => 0,
            "3x3 Squares" => 8,
            "5x5 Squares" => 24,
            "7x7 Squares" => 48,
            "9x9 Squares" => 80,
            "Gaussian Spots" => 80, // Same as 9x9 pattern (81 spots, 0-80)
            "Continuous lines" => 4, // 5 lines (0-4), each spanning 2 magnitudes
            _ => 0
        };
    }

    /// <summary>
    /// Gets the source signal flux for a selected square
    /// </summary>
    /// <param name="selectedSquareIndex">Index of the selected square</param>
    /// <param name="pattern">Pattern name</param>
    /// <param name="baseSignalFlux">Base signal flux value</param>
    /// <returns>Source signal flux for the selected square</returns>
    public float GetSourceSignalFlux(int selectedSquareIndex, string pattern, float baseSignalFlux)
    {
        if (pattern == "No Signal")
        {
            return 0.0f;
        }

        int maxValidIndex = this.GetMaxValidIndex(pattern);
        
        if (selectedSquareIndex <= maxValidIndex)
        {
            // Special handling for continuous lines pattern
            if (pattern == "Continuous lines")
            {
                // Each line represents 2 magnitudes of decay
                // Line 0 (top): 0-2 magnitudes, Line 1: 2-4 magnitudes, etc.
                // The selected line's average intensity (middle of the line)
                double brightnessFactor = Math.Pow(brightnessDecay, selectedSquareIndex * 2.0 + 1.0);
                return (float)(baseSignalFlux * brightnessFactor);
            }
            else
            {
                // Calculate the brightness factor for the selected square
                float brightnessFactor = 1.0f;
                for (int i = 0; i < selectedSquareIndex; i++)
                {
                    brightnessFactor *= (float)brightnessDecay;
                }
                return baseSignalFlux * brightnessFactor;
            }
        }
        
        return 0.0f;
    }
} 