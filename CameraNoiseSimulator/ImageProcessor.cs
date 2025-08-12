namespace NoiseSimulator;

/// <summary>
/// Main processor for generating astronomical images with noise and signal
/// </summary>
public class ImageProcessor
{
    public ImageProcessor()
    {
    }
    
    public uint[,] GenerateImage(
        double backgroundFlux, 
        double signalFlux, 
        string signalPattern, 
        double exposureTime, 
        double readNoise, 
        int seed, 
        int squareSize = 20, 
        bool useVerticalLines = false)
    {
        var photonDetector = new PhotonDetector(seed, readNoise);
        var signalGenerator = new SignalGenerator();
        
        uint[,] imageData = new uint[1024, 1024];
        
        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                // Get signal flux for this pixel
                float pixelSignalFlux = signalGenerator.GetPatternSignalFlux(
                    x, y, (float)signalFlux, signalPattern, squareSize, useVerticalLines);
                
                // Total flux for this pixel
                double totalFlux = backgroundFlux + pixelSignalFlux;
                
                // Generate photon detection for this pixel
                int detectedPhotons = photonDetector.GeneratePhotonDetection(totalFlux, exposureTime);
                
                // Add offset to prevent negative values
                int offset = photonDetector.Offset;
                int finalValue = detectedPhotons + offset;
                
                // Ensure non-negative and convert to uint
                imageData[y, x] = (uint)Math.Max(0, finalValue);
            }
        }
        
        return imageData;
    }
    
    public uint[,] GenerateAveragedImage(
        int numExposures,
        double backgroundFlux, 
        double signalFlux, 
        string signalPattern, 
        double exposureTime, 
        double readNoise, 
        int seed, 
        int squareSize = 20, 
        bool useVerticalLines = false)
    {
        if (numExposures <= 1)
            return GenerateImage(backgroundFlux, signalFlux, signalPattern, exposureTime, readNoise, seed, squareSize, useVerticalLines);
        
        // Generate multiple exposures
        uint[][,] exposures = new uint[numExposures][,];
        for (int exp = 0; exp < numExposures; exp++)
        {
            exposures[exp] = GenerateImage(backgroundFlux, signalFlux, signalPattern, exposureTime, readNoise, seed + exp, squareSize, useVerticalLines);
        }
        
        // Average the exposures
        uint[,] averagedImage = new uint[1024, 1024];
        for (int y = 0; y < 1024; y++)
        {
            for (int x = 0; x < 1024; x++)
            {
                long sum = 0;
                for (int exp = 0; exp < numExposures; exp++)
                {
                    sum += exposures[exp][y, x];
                }
                averagedImage[y, x] = (uint)(sum / numExposures);
            }
        }
        
        return averagedImage;
    }
} 