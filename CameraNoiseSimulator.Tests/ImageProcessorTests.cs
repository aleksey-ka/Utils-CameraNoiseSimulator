using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class ImageProcessorTests
{
    [Fact]
    public void ImageProcessor_Constructor_ShouldUseDefaultConfig()
    {
        // Act
        var processor = new ImageProcessor();
        
        // Assert
        Assert.NotNull(processor);
    }
    
    [Fact]
    public void ImageProcessor_Constructor_ShouldUseCustomConfig()
    {
        // Arrange
        var config = SimulationConfig.CreateCustom(512, 512);
        
        // Act
        var processor = new ImageProcessor(config);
        
        // Assert
        Assert.NotNull(processor);
    }
    
    [Fact]
    public void ImageProcessor_GenerateImage_ShouldCreateImageWithCorrectDimensions()
    {
        // Arrange
        var config = SimulationConfig.CreateCustom(256, 256);
        var processor = new ImageProcessor(config);
        
        // Act
        var image = processor.GenerateImage(
            backgroundFlux: 5.0,
            signalFlux: 50.0,
            signalPattern: "Square",
            exposureTime: 1.0,
            readNoise: 1.0,
            seed: 42,
            squareSize: 20);
        
        // Assert
        Assert.Equal(256, image.GetLength(0)); // Height
        Assert.Equal(256, image.GetLength(1)); // Width
    }
    
    [Fact]
    public void ImageProcessor_GenerateImage_ShouldHandleNoSignal()
    {
        // Arrange
        var processor = new ImageProcessor();
        
        // Act
        var image = processor.GenerateImage(
            backgroundFlux: 10.0,
            signalFlux: 0.0,
            signalPattern: "No Signal",
            exposureTime: 1.0,
            readNoise: 0.5,
            seed: 42);
        
        // Assert
        Assert.NotNull(image);
        Assert.Equal(1024, image.GetLength(0));
        Assert.Equal(1024, image.GetLength(1));
        
        // All pixels should have some value due to background + offset
        for (int y = 0; y < 10; y += 100) // Sample some pixels
        {
            for (int x = 0; x < 10; x += 100)
            {
                Assert.True(image[y, x] >= 0, "Pixel values should be non-negative");
            }
        }
    }
    
    [Fact]
    public void ImageProcessor_GenerateAveragedImage_ShouldHandleSingleExposure()
    {
        // Arrange
        var processor = new ImageProcessor();
        
        // Act
        var image = processor.GenerateAveragedImage(
            numExposures: 1,
            backgroundFlux: 5.0,
            signalFlux: 50.0,
            signalPattern: "Square",
            exposureTime: 1.0,
            readNoise: 1.0,
            seed: 42);
        
        // Assert
        Assert.NotNull(image);
        Assert.Equal(1024, image.GetLength(0));
        Assert.Equal(1024, image.GetLength(1));
    }
    
    [Fact]
    public void ImageProcessor_GenerateAveragedImage_ShouldAverageMultipleExposures()
    {
        // Arrange
        var processor = new ImageProcessor();
        
        // Act
        var image = processor.GenerateAveragedImage(
            numExposures: 4,
            backgroundFlux: 5.0,
            signalFlux: 50.0,
            signalPattern: "Square",
            exposureTime: 1.0,
            readNoise: 1.0,
            seed: 42);
        
        // Assert
        Assert.NotNull(image);
        Assert.Equal(1024, image.GetLength(0));
        Assert.Equal(1024, image.GetLength(1));
        
        // Averaged image should have reasonable values
        for (int y = 0; y < 10; y += 100) // Sample some pixels
        {
            for (int x = 0; x < 10; x += 100)
            {
                Assert.True(image[y, x] >= 0, "Pixel values should be non-negative");
            }
        }
    }
    
    [Fact]
    public void ImageProcessor_GenerateImage_ShouldRespectSeedForReproducibility()
    {
        // Arrange
        var processor = new ImageProcessor();
        
        // Act
        var image1 = processor.GenerateImage(5.0, 50.0, "Square", 1.0, 1.0, 42);
        var image2 = processor.GenerateImage(5.0, 50.0, "Square", 1.0, 1.0, 42);
        
        // Assert
        Assert.Equal(1024, image1.GetLength(0));
        Assert.Equal(1024, image1.GetLength(1));
        Assert.Equal(1024, image2.GetLength(0));
        Assert.Equal(1024, image2.GetLength(1));
        
        // With same seed, images should be identical
        for (int y = 0; y < 100; y += 50) // Sample some pixels
        {
            for (int x = 0; x < 100; x += 50)
            {
                Assert.Equal(image1[y, x], image2[y, x]);
            }
        }
    }
} 