using System.Text;

namespace NoiseSimulator;

/// <summary>
/// FITS (Flexible Image Transport System) file writer for astronomical data
/// </summary>
public class FitsWriter
{
    public FitsWriter()
    {
    }
    

    
    /// <summary>
    /// Saves image data to a FITS file
    /// </summary>
    public void SaveFits(string filePath, uint[,] imageData)
    {
        int height = imageData.GetLength(0);
        int width = imageData.GetLength(1);
        
        using (var writer = new BinaryWriter(File.Create(filePath)))
        {
            // Write FITS header
            WriteFitsHeader(writer, width, height);
            
            // Write image data
            WriteImageData(writer, imageData);
            
            // Pad to 2880-byte boundary if necessary
            long currentPosition = writer.BaseStream.Position;
            long remainder = currentPosition % 2880;
            if (remainder > 0)
            {
                int padding = (int)(2880 - remainder);
                byte[] paddingBytes = new byte[padding];
                writer.Write(paddingBytes);
            }
        }
    }
    
    private void WriteFitsHeader(BinaryWriter writer, int width, int height)
    {
        // Create header cards
        var headerCards = new List<string>
        {
            "SIMPLE  =                    T / FITS standard",
            $"BITPIX  =                   16 / Number of bits per data pixel",
            $"NAXIS   =                    2 / Number of data axes",
            $"NAXIS1  = {width,20} / Length of data axis 1",
            $"NAXIS2  = {height,20} / Length of data axis 2",
            "EXTEND  =                    T / FITS dataset may contain extensions",
            "ORIGIN  = 'Camera Noise Simulator' / Origin of the FITS file",
            $"DATE    = '{DateTime.Now:yyyy-MM-dd}' / Date of file creation",
            "COMMENT = 'Simulated astronomical image with noise'",
            "END"
        };
        
        // Write header cards (each card is 80 characters)
        foreach (string card in headerCards)
        {
            string paddedCard = card.PadRight(80);
            byte[] cardBytes = Encoding.ASCII.GetBytes(paddedCard);
            writer.Write(cardBytes);
        }
        
        // Pad header to 2880-byte boundary
        long headerSize = headerCards.Count * 80;
        long padding = 2880 - (headerSize % 2880);
        if (padding < 2880)
        {
            byte[] paddingBytes = new byte[padding];
            writer.Write(paddingBytes);
        }
    }
    
    private void WriteImageData(BinaryWriter writer, uint[,] imageData)
    {
        int height = imageData.GetLength(0);
        int width = imageData.GetLength(1);
        
        // FITS uses big-endian format
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Convert uint to ushort (16-bit) and write in big-endian
                ushort pixelValue = (ushort)Math.Min(imageData[y, x], ushort.MaxValue);
                byte[] bytes = BitConverter.GetBytes(pixelValue);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);
                writer.Write(bytes);
            }
        }
    }
    
    // Legacy method for backward compatibility
    public static void SaveFits(string filePath, ushort[,] imageData)
    {
        var fitsWriter = new FitsWriter();
        uint[,] uintData = new uint[imageData.GetLength(0), imageData.GetLength(1)];
        
        for (int y = 0; y < imageData.GetLength(0); y++)
        {
            for (int x = 0; x < imageData.GetLength(1); x++)
            {
                uintData[y, x] = imageData[y, x];
            }
        }
        
        fitsWriter.SaveFits(filePath, uintData);
    }
} 