namespace CameraNoiseSimulator.Tests;

/// <summary>
/// Centralized configuration for all tests
/// </summary>
public static class TestConfiguration
{
    // Image dimensions
    public const int ImageSize = 1024;
    public const int ImageWidth = ImageSize;
    public const int ImageHeight = ImageSize;
    
    // Test parameters
    public const int FixedSeed = 42;           // Fixed seed for reproducible tests
    public const double ExposureTime = 1.0;    // 1 second exposure for tests
    
    // Test scenarios
    public static class Scenarios
    {
        public static class NoSignal
        {
            public const double BackgroundFlux = 0.0;
            public const double SignalFlux = 0.0;
            public const double ReadNoise = 0.0;
        }
        
        public static class LowSignal
        {
            public const double BackgroundFlux = 1.0;
            public const double SignalFlux = 5.0;
            public const double ReadNoise = 0.5;
        }
        
        public static class HighSignal
        {
            public const double BackgroundFlux = 10.0;
            public const double SignalFlux = 100.0;
            public const double ReadNoise = 2.0;
        }
    }
    
    // Pattern configurations
    public static class Patterns
    {
        public const int DefaultSquareSize = 10;
        public const string SquarePattern = "Square";
        public const string GaussianPattern = "Gaussian Spots";
    }
    
    // Statistical tolerance
    public const double StatisticalTolerance = 0.01; // 1% tolerance for statistical tests
} 