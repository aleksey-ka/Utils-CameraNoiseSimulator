using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class SignalGeneratorTests
{
    [Fact]
    public void GetPatternSignalFlux_SquarePattern_ShouldReturnCorrectSignal()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test center pixel of square pattern
        float flux = generator.GetPatternSignalFlux(512, 512, baseSignalFlux, "Square", squareSize, false);
        
        // Assert
        Assert.Equal(baseSignalFlux, flux, precision: 6);
    }
    
    [Fact]
    public void GetPatternSignalFlux_OutsideSquarePattern_ShouldReturnZero()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test pixel outside the square pattern
        float flux = generator.GetPatternSignalFlux(0, 0, baseSignalFlux, "Square", squareSize, false);
        
        // Assert
        Assert.Equal(0.0f, flux);
    }
    
    [Fact]
    public void GetPatternSignalFlux_3x3Squares_ShouldReturnCorrectSignal()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test center square (index 0) - gets full brightness
        float centerFlux = generator.GetPatternSignalFlux(512, 512, baseSignalFlux, "3x3 Squares", squareSize, false);
        
        // Test that we can find some other squares with signal (brightness will vary)
        bool foundOtherSquares = false;
        for (int offset = 40; offset <= 80; offset += 20)
        {
            float testFlux = generator.GetPatternSignalFlux(512 + offset, 512, baseSignalFlux, "3x3 Squares", squareSize, false);
            if (testFlux > 0 && testFlux < baseSignalFlux)
            {
                foundOtherSquares = true;
                break;
            }
        }
        
        // Assert
        Assert.Equal(baseSignalFlux, centerFlux, precision: 6);
        Assert.True(foundOtherSquares, "Should find other squares with signal in 3x3 pattern");
    }
    
    [Fact]
    public void GetPatternSignalFlux_5x5Squares_ShouldReturnCorrectSignal()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test center square (index 0) - gets full brightness
        float centerFlux = generator.GetPatternSignalFlux(512, 512, baseSignalFlux, "5x5 Squares", squareSize, false);
        
        // Test that we can find some other squares with signal (brightness will vary)
        bool foundOtherSquares = false;
        for (int offset = 40; offset <= 120; offset += 40)
        {
            float testFlux = generator.GetPatternSignalFlux(512 + offset, 512, baseSignalFlux, "5x5 Squares", squareSize, false);
            if (testFlux > 0 && testFlux < baseSignalFlux)
            {
                foundOtherSquares = true;
                break;
            }
        }
        
        // Assert
        Assert.Equal(baseSignalFlux, centerFlux, precision: 6);
        Assert.True(foundOtherSquares, "Should find other squares with signal in 5x5 pattern");
    }
    
    [Fact]
    public void GetPatternSignalFlux_GaussianSpots_ShouldReturnCorrectSignal()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test center of Gaussian spot
        float centerFlux = generator.GetPatternSignalFlux(512, 512, baseSignalFlux, "Gaussian Spots", squareSize, false);
        
        // Test edge of Gaussian spot
        float edgeFlux = generator.GetPatternSignalFlux(512 + 10, 512, baseSignalFlux, "Gaussian Spots", squareSize, false);
        
        // Assert
        Assert.True(centerFlux > 0, "Center should have signal");
        Assert.True(edgeFlux < centerFlux, "Edge should have less signal than center");
        Assert.True(edgeFlux >= 0, "Edge should have non-negative signal");
    }
    
    [Fact]
    public void GetPatternSignalFlux_VerticalLines_ShouldReturnAlternatingSignal()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test alternating columns in vertical lines pattern
        float evenColumnFlux = generator.GetPatternSignalFlux(512, 512, baseSignalFlux, "Square", squareSize, true);
        float oddColumnFlux = generator.GetPatternSignalFlux(513, 512, baseSignalFlux, "Square", squareSize, true);
        
        // Assert
        Assert.Equal(baseSignalFlux, evenColumnFlux, precision: 6);
        Assert.Equal(0.0f, oddColumnFlux);
    }
    
    [Fact]
    public void GetPatternSignalFlux_ContinuousLines_ShouldReturnCorrectSignal()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test continuous lines pattern
        float flux = generator.GetPatternSignalFlux(512, 512, baseSignalFlux, "Continuous lines", squareSize, false);
        
        // Assert
        Assert.True(flux >= 0, "Continuous lines should have non-negative signal");
    }
    
    [Fact]
    public void GetPatternSignalFlux_InvalidPattern_ShouldReturnZero()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test invalid pattern name
        float flux = generator.GetPatternSignalFlux(512, 512, baseSignalFlux, "Invalid Pattern", squareSize, false);
        
        // Assert
        Assert.Equal(0.0f, flux);
    }
    
    [Theory]
    [InlineData("Square", 1)]
    [InlineData("3x3 Squares", 9)]
    [InlineData("5x5 Squares", 25)]
    [InlineData("7x7 Squares", 49)]
    [InlineData("9x9 Squares", 81)]
    public void GetPatternSignalFlux_DifferentPatterns_ShouldHaveCorrectNumberOfSquares(string pattern, int expectedSquares)
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Count pixels with signal
        int signalPixels = 0;
        for (int y = 0; y < 1024; y += 10) // Sample every 10th pixel for performance
        {
            for (int x = 0; x < 1024; x += 10)
            {
                if (generator.GetPatternSignalFlux(x, y, baseSignalFlux, pattern, squareSize, false) > 0)
                {
                    signalPixels++;
                }
            }
        }
        
        // Assert - Should have approximately the expected number of squares
        // We multiply by squareSize^2 since each square has multiple pixels
        int expectedPixels = expectedSquares * squareSize * squareSize;
        int tolerance = expectedPixels / 10; // 10% tolerance for sampling
        
        Assert.True(Math.Abs(signalPixels * 100 - expectedPixels) < tolerance, 
            $"Expected approximately {expectedPixels} signal pixels, got {signalPixels * 100}");
    }
    
    [Fact]
    public void GetPatternSignalFlux_BrightnessDecay_ShouldFollowExponentialPattern()
    {
        // Arrange
        var generator = new SignalGenerator();
        const float baseSignalFlux = 100.0f;
        const int squareSize = 20;
        
        // Act - Test brightness decay in 3x3 pattern
        float centerFlux = generator.GetPatternSignalFlux(512, 512, baseSignalFlux, "3x3 Squares", squareSize, false);
        
        // Find a pixel in another square to verify decay exists
        bool foundDecay = false;
        for (int offset = 40; offset <= 80; offset += 20)
        {
            float testFlux = generator.GetPatternSignalFlux(512 + offset, 512, baseSignalFlux, "3x3 Squares", squareSize, false);
            if (testFlux > 0 && testFlux < baseSignalFlux)
            {
                foundDecay = true;
                break;
            }
        }
        
        // Assert
        Assert.Equal(baseSignalFlux, centerFlux, precision: 6);
        Assert.True(foundDecay, "Should find squares with reduced brightness due to decay");
        
        // Verify decay factor is reasonable (should be less than 1.0)
        Assert.True(generator.brightnessDecay < 1.0, "Brightness decay should be less than 1.0");
        Assert.True(generator.brightnessDecay > 0.5, "Brightness decay should be reasonable (> 0.5)");
    }
} 