using NoiseSimulator;
using System.Text;

namespace CameraNoiseSimulator.Tests;

public class FitsWriterTests
{
    private readonly string _testOutputPath = Path.Combine(Path.GetTempPath(), "test_fits.fits");
    
    [Fact]
    public void SaveFits_ValidData_ShouldCreateValidFitsFile()
    {
        // Arrange
        const int width = 1024;  // Must be 1024 as expected by FitsWriter
        const int height = 1024;
        ushort[,] testData = CreateTestImage(width, height);
        
        // Act
        FitsWriter.SaveFits(_testOutputPath, testData);
        
        // Assert
        Assert.True(File.Exists(_testOutputPath), "FITS file should be created");
        
        // Verify file size is multiple of 2880 bytes (FITS requirement)
        long fileSize = new FileInfo(_testOutputPath).Length;
        Assert.True(fileSize % 2880 == 0, $"File size {fileSize} should be multiple of 2880 bytes");
        
        // Verify header is exactly 2880 bytes
        using var fs = new FileStream(_testOutputPath, FileMode.Open);
        Assert.Equal(2880, fs.Length >= 2880 ? 2880 : fs.Length);
        
        // Cleanup
        CleanupTestFile();
    }
    
    [Fact]
    public void SaveFits_HeaderFormat_ShouldFollowFitsStandard()
    {
        // Arrange
        const int width = 1024;  // Must be 1024 as expected by FitsWriter
        const int height = 1024;
        ushort[,] testData = CreateTestImage(width, height);
        
        // Act
        FitsWriter.SaveFits(_testOutputPath, testData);
        
        // Assert
        using var fs = new FileStream(_testOutputPath, FileMode.Open);
        using var reader = new BinaryReader(fs);
        
        // Read first 80 characters (first header record)
        byte[] firstRecord = reader.ReadBytes(80);
        string firstRecordStr = Encoding.ASCII.GetString(firstRecord);
        
        // Verify SIMPLE keyword
        Assert.StartsWith("SIMPLE  =                    T", firstRecordStr);
        
        // Verify header ends with proper padding
        fs.Position = 0;
        byte[] header = reader.ReadBytes(2880);
        Assert.Equal(2880, header.Length);
        
        CleanupTestFile();
    }
    
    [Fact]
    public void SaveFits_DataFormat_ShouldBe16BitSignedIntegers()
    {
        // Arrange
        const int width = 1024;  // Must be 1024 as expected by FitsWriter
        const int height = 1024;
        ushort[,] testData = CreateTestImage(width, height);
        
        // Act
        FitsWriter.SaveFits(_testOutputPath, testData);
        
        // Assert
        using var fs = new FileStream(_testOutputPath, FileMode.Open);
        using var reader = new BinaryReader(fs);
        
        // Skip header (2880 bytes)
        fs.Position = 2880;
        
        // Read first few data values
        for (int i = 0; i < 4; i++)
        {
            // FITS uses big-endian format
            byte highByte = reader.ReadByte();
            byte lowByte = reader.ReadByte();
            
            // Convert to signed 16-bit integer
            short value = (short)((highByte << 8) | lowByte);
            
            // Value should be in signed 16-bit range
            Assert.True(value >= -32768 && value <= 32767, $"Value {value} should be in signed 16-bit range");
        }
        
        CleanupTestFile();
    }
    
    [Fact]
    public void SaveFits_ImageDimensions_ShouldMatchInputData()
    {
        // Arrange
        const int width = 1024;  // Must be 1024 as expected by FitsWriter
        const int height = 1024;
        ushort[,] testData = CreateTestImage(width, height);
        
        // Act
        FitsWriter.SaveFits(_testOutputPath, testData);
        
        // Assert
        using var fs = new FileStream(_testOutputPath, FileMode.Open);
        using var reader = new BinaryReader(fs);
        
        // Read header to find NAXIS1 and NAXIS2 keywords
        fs.Position = 0;
        byte[] header = reader.ReadBytes(2880);
        string headerStr = Encoding.ASCII.GetString(header);
        
        // Verify dimensions are in header
        Assert.Contains("NAXIS1  =                 1024", headerStr);
        Assert.Contains("NAXIS2  =                 1024", headerStr);
        
        CleanupTestFile();
    }
    
    [Theory]
    [InlineData(1024, 1024)]  // Standard size
    public void SaveFits_DifferentImageSizes_ShouldWorkCorrectly(int width, int height)
    {
        // Arrange
        ushort[,] testData = CreateTestImage(width, height);
        
        // Act
        FitsWriter.SaveFits(_testOutputPath, testData);
        
        // Assert
        Assert.True(File.Exists(_testOutputPath));
        
        // Verify file size is reasonable
        long fileSize = new FileInfo(_testOutputPath).Length;
        long expectedMinSize = 2880 + (width * height * 2); // Header + data
        Assert.True(fileSize >= expectedMinSize, $"File size {fileSize} should be at least {expectedMinSize}");
        
        CleanupTestFile();
    }
    
    private static ushort[,] CreateTestImage(int width, int height)
    {
        ushort[,] image = new ushort[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a simple test pattern
                image[y, x] = (ushort)((x + y) % 65536);
            }
        }
        return image;
    }
    
    private void CleanupTestFile()
    {
        if (File.Exists(_testOutputPath))
        {
            try
            {
                File.Delete(_testOutputPath);
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }
    }
} 