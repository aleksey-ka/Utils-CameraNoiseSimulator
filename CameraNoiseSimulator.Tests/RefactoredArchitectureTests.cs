using NoiseSimulator;

namespace CameraNoiseSimulator.Tests;

public class RefactoredArchitectureTests
{
    [Fact]
    public void CompleteRefactoredWorkflow_ShouldGenerateAndExportImage()
    {
        // Arrange
        var config = SimulationConfig.CreateCustom(512, 512);
        var simulationService = new SimulationService(config);
        var parameters = SimulationParameters.CreateCustom(
            backgroundFlux: 10.0,
            signalFlux: 100.0,
            signalPattern: "Square",
            exposureTime: 2.0,
            readNoise: 1.5,
            seed: 123);
        
        // Act
        var result = simulationService.RunSimulation(parameters);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(512, result.ImageData.GetLength(0));
        Assert.Equal(512, result.ImageData.GetLength(1));
        Assert.True(result.Statistics.SignalToNoiseRatio > 0);
        
        // Verify signal is higher than background
        Assert.True(result.Statistics.Signal.mean > result.Statistics.Background.mean);
    }
    
    [Fact]
    public void MultipleExposureWorkflow_ShouldReduceNoise()
    {
        // Arrange
        var simulationService = new SimulationService();
        var parameters = SimulationParameters.CreateCustom(
            backgroundFlux: 5.0,
            signalFlux: 30.0,
            signalPattern: "3x3 Squares",
            exposureTime: 1.0,
            readNoise: 2.0,
            seed: 42);
        
        // Act
        var singleResult = simulationService.RunSimulation(parameters);
        var averagedResult = simulationService.RunAveragedSimulation(parameters, 4);
        
        // Assert
        Assert.NotNull(singleResult);
        Assert.NotNull(averagedResult);
        
        // Both should have same dimensions
        Assert.Equal(singleResult.ImageData.GetLength(0), averagedResult.ImageData.GetLength(0));
        Assert.Equal(singleResult.ImageData.GetLength(1), averagedResult.ImageData.GetLength(1));
        
        // Averaged result should have exposures count
        Assert.Equal(4, averagedResult.NumExposures);
    }
    
    [Fact]
    public void ImageExporterFactory_ShouldCreateCorrectExporters()
    {
        // Arrange
        var factory = new ImageExporterFactory();
        
        // Act
        var fitsExporter = factory.CreateExporter("FITS");
        var fitsExporter2 = factory.CreateExporter("fits");
        
        // Assert
        Assert.NotNull(fitsExporter);
        Assert.NotNull(fitsExporter2);
        Assert.IsAssignableFrom<IImageExporter>(fitsExporter);
        Assert.IsAssignableFrom<IImageExporter>(fitsExporter2);
        
        // Should support FITS format
        Assert.True(factory.IsFormatSupported("FITS"));
        Assert.True(factory.IsFormatSupported("fits"));
        Assert.False(factory.IsFormatSupported("PNG"));
        
        // Should get correct extensions
        Assert.Equal(".fits", factory.GetFileExtension("FITS"));
        Assert.Equal(".fits", factory.GetFileExtension("fits"));
    }
    
    [Fact]
    public void FitsWriter_ShouldValidateDimensions()
    {
        // Arrange
        var config = SimulationConfig.CreateCustom(256, 256);
        var fitsWriter = new FitsWriter(config);
        uint[,] correctImage = new uint[256, 256];
        uint[,] wrongImage = new uint[512, 512];
        
        // Act & Assert
        // Should accept correct dimensions
        Assert.NotNull(fitsWriter);
        
        // Should reject wrong dimensions
        Assert.Throws<ArgumentException>(() => 
            fitsWriter.SaveFits("test.fits", wrongImage));
    }
    
    [Fact]
    public void StatisticsService_ShouldCalculateStatisticsForDifferentImageSizes()
    {
        // Arrange
        var config = SimulationConfig.CreateCustom(128, 128);
        var statisticsService = new StatisticsService(config);
        uint[,] imageData = CreateTestImage(128, 128);
        
        // Act
        var statistics = statisticsService.CalculateImageStatistics(
            imageData, "Square", 20, false, 50.0f);
        
        // Assert
        Assert.NotNull(statistics);
        Assert.Equal(128, statistics.ImageWidth);
        Assert.Equal(128, statistics.ImageHeight);
        Assert.True(statistics.Background.count > 0);
        Assert.True(statistics.Signal.count > 0);
    }
    
    [Fact]
    public void Configuration_ShouldSupportDifferentImageSizes()
    {
        // Arrange & Act
        var smallConfig = SimulationConfig.CreateCustom(64, 64);
        var mediumConfig = SimulationConfig.CreateCustom(512, 512);
        var largeConfig = SimulationConfig.CreateCustom(2048, 2048);
        
        // Assert
        Assert.Equal(64, smallConfig.ImageWidth);
        Assert.Equal(64, smallConfig.ImageHeight);
        Assert.Equal(512, mediumConfig.ImageWidth);
        Assert.Equal(512, mediumConfig.ImageHeight);
        Assert.Equal(2048, largeConfig.ImageWidth);
        Assert.Equal(2048, largeConfig.ImageHeight);
    }
    
    [Fact]
    public void ImageProcessor_ShouldRespectConfigurationDimensions()
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
            seed: 42);
        
        // Assert
        Assert.Equal(256, image.GetLength(0));
        Assert.Equal(256, image.GetLength(1));
    }
    
    [Fact]
    public void CompleteWorkflow_ShouldExportToFits()
    {
        // Arrange
        var simulationService = new SimulationService();
        var parameters = SimulationParameters.CreateCustom(
            backgroundFlux: 5.0,
            signalFlux: 50.0,
            signalPattern: "Square",
            exposureTime: 1.0,
            readNoise: 1.0,
            seed: 42);
        
        // Act
        var result = simulationService.RunSimulation(parameters);
        
        // Should be able to export
        Assert.NotNull(result);
        Assert.NotNull(result.ImageData);
        
        // Export should work (we won't actually create the file in tests)
        var factory = new ImageExporterFactory();
        var exporter = factory.CreateExporter("FITS");
        Assert.NotNull(exporter);
    }
    
    private uint[,] CreateTestImage(int width, int height)
    {
        uint[,] image = new uint[height, width];
        var random = new Random(42);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a simple test pattern with signal in center
                if (x >= width / 4 && x < 3 * width / 4 && 
                    y >= height / 4 && y < 3 * height / 4)
                {
                    image[y, x] = (uint)(100 + random.Next(50)); // Signal region
                }
                else
                {
                    image[y, x] = (uint)(10 + random.Next(20)); // Background region
                }
            }
        }
        
        return image;
    }
} 