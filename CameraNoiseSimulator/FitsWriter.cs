using System.Text;

namespace NoiseSimulator;

/// <summary>
/// Simple FITS writer for 16-bit astronomical data
/// Supports only the specific case needed for this noise simulator
/// </summary>
public class FitsWriter
{
    /// <summary>
    /// Saves 16-bit astronomical data to FITS format
    /// FITS 16-bit integers are signed (-32768 to +32767)
    /// BZERO and BSCALE are used to map to physical values
    /// </summary>
    /// <param name="filename">Output filename</param>
    /// <param name="data">16-bit image data (1024x1024)</param>
    /// <param name="bzero">Zero point for scaling (default 0)</param>
    /// <param name="bscale">Scale factor (default 1)</param>
    public static void SaveFits(string filename, ushort[,] data, double bzero = 0.0, double bscale = 1.0)
    {
        const int width = 1024;
        const int height = 1024;
        
        using (FileStream fs = new FileStream(filename, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            // Write FITS header
            WriteFitsHeader(writer, width, height, bzero, bscale);
            
            // Verify header is exactly 2880 bytes
            long headerSize = fs.Position;
            if (headerSize != 2880)
            {
                throw new InvalidOperationException($"Header size is {headerSize} bytes, expected 2880 bytes");
            }
            
            // Write data (FITS uses big-endian format)
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Convert unsigned ushort to signed short for FITS
                    ushort unsignedValue = data[y, x];
                    
                    // Map to signed 16-bit range with offset:
                    // Raw values 0-65535 -> FITS values -32768 to +32767
                    // Physical value = BZERO + BSCALE * FITS_value
                    // With BZERO=32768 and BSCALE=1.0, this gives proper offset
                    short signedValue = (short)(unsignedValue - 32768);
                    
                    // Write as big-endian (FITS standard requires big-endian)
                    byte[] bytes = BitConverter.GetBytes(signedValue);
                    if (BitConverter.IsLittleEndian)
                    {
                        // Reverse bytes for big-endian
                        writer.Write(bytes[1]); // High byte
                        writer.Write(bytes[0]); // Low byte
                    }
                    else
                    {
                        // Already big-endian
                        writer.Write(bytes[0]); // High byte
                        writer.Write(bytes[1]); // Low byte
                    }
                }
            }
            
            // Calculate and apply data padding
            long dataEndPosition = fs.Position;
            long dataSize = dataEndPosition - headerSize;
            int padding = (int)(2880 - (dataSize % 2880));
            if (padding == 2880) padding = 0; // No padding needed if already aligned
            
            if (padding > 0)
            {
                byte[] padBytes = new byte[padding];
                writer.Write(padBytes);
            }
            
            // Verify total file size
            long totalSize = fs.Position;
            long expectedSize = headerSize + dataSize + padding;
            if (totalSize != expectedSize)
            {
                throw new InvalidOperationException($"File size mismatch: actual={totalSize}, expected={expectedSize}");
            }
            
            // Ensure file ends at 2880-byte boundary
            if (totalSize % 2880 != 0)
            {
                throw new InvalidOperationException($"File size {totalSize} is not a multiple of 2880 bytes");
            }
        }
    }
    
    /// <summary>
    /// Writes the FITS header with standard astronomical keywords
    /// </summary>
    private static void WriteFitsHeader(BinaryWriter writer, int width, int height, double bzero, double bscale)
    {
        // Write comprehensive FITS header for 16-bit data
        // Each line must be exactly 80 characters with proper spacing
        WriteFitsLogical(writer, "SIMPLE", true, "FITS standard");
        WriteFitsInteger(writer, "BITPIX", 16, "Number of bits per data pixel");
        WriteFitsInteger(writer, "NAXIS", 2, "Number of data axes");
        WriteFitsInteger(writer, "NAXIS1", width, "Length of data axis 1");
        WriteFitsInteger(writer, "NAXIS2", height, "Length of data axis 2");
        WriteFitsFloat(writer, "BZERO", bzero, "Physical value corresponding to zero");
        WriteFitsFloat(writer, "BSCALE", bscale, "Physical value scaling factor");
        WriteFitsString(writer, "BUNIT", "ADU", "Physical units of the array values");
        WriteFitsString(writer, "TELESCOP", "NoiseSimulator", "Telescope used");
        WriteFitsString(writer, "INSTRUME", "Simulated", "Instrument used");
        WriteFitsString(writer, "OBJECT", "Simulated Data", "Object name");
        WriteFitsString(writer, "SOFTWARE", "NoiseSimulator", "Software used");
        WriteFitsFloat(writer, "EXPTIME", 1.0, "Exposure time in seconds");
        WriteFitsFloat(writer, "GAIN", 1.0, "Detector gain in e-/ADU");
        WriteFitsFloat(writer, "RDNOISE", 1.0, "Read noise in e-");
        WriteHeaderLine(writer, "COMMENT   Noise Simulator generated FITS file");
        WriteHeaderLine(writer, "END");
        
        // Pad to complete 2880-byte header block (36 * 80 = 2880)
        // FITS standard requires complete 2880-byte blocks
        long currentPosition = writer.BaseStream.Position;
        int padding = (int)(2880 - currentPosition);
        if (padding > 0)
        {
            byte[] padBytes = new byte[padding];
            // Fill with spaces, not nulls
            for (int i = 0; i < padding; i++)
            {
                padBytes[i] = (byte)' ';
            }
            writer.Write(padBytes);
        }
        
        // Verify header is exactly 2880 bytes
        long finalHeaderSize = writer.BaseStream.Position;
        if (finalHeaderSize != 2880)
        {
            throw new InvalidOperationException($"Final header size is {finalHeaderSize} bytes, expected 2880 bytes");
        }
    }
    
    /// <summary>
    /// Writes a single FITS header line (exactly 80 characters)
    /// </summary>
    private static void WriteHeaderLine(BinaryWriter writer, string line)
    {
        // FITS header lines must be exactly 80 characters in ASCII encoding
        // Format: KEYWORD = VALUE / COMMENT
        // Ensure proper spacing and no null bytes
        string paddedLine = line.PadRight(80, ' '); // Use space, not null
        
        // Use explicit ASCII encoding to ensure compatibility
        byte[] lineBytes = new byte[80];
        for (int i = 0; i < 80; i++)
        {
            if (i < paddedLine.Length)
            {
                char c = paddedLine[i];
                // Ensure ASCII compatibility
                if (c > 127) c = ' '; // Replace non-ASCII with space
                lineBytes[i] = (byte)c;
            }
            else
            {
                lineBytes[i] = (byte)' ';
            }
        }
        
        writer.Write(lineBytes);
    }
    
    /// <summary>
    /// Writes a FITS header keyword with proper formatting
    /// </summary>
    private static void WriteFitsKeyword(BinaryWriter writer, string keyword, string value, string comment)
    {
        // FITS keyword format: KEYWORD = VALUE / COMMENT
        // KEYWORD is 8 characters, VALUE starts at position 10, COMMENT starts at position 32
        string keywordPart = keyword.PadRight(8);
        string valuePart = value.PadRight(20);
        string commentPart = comment.PadRight(47); // Leave space for " / "
        
        string fullLine = $"{keywordPart}= {valuePart}/ {commentPart}";
        WriteHeaderLine(writer, fullLine);
    }
    
    /// <summary>
    /// Writes a FITS logical keyword (T or F)
    /// </summary>
    private static void WriteFitsLogical(BinaryWriter writer, string keyword, bool value, string comment)
    {
        string keywordPart = keyword.PadRight(8);
        string valuePart = (value ? "T" : "F").PadLeft(20);
        string commentPart = comment.PadRight(47);
        
        string fullLine = $"{keywordPart}= {valuePart}/ {commentPart}";
        WriteHeaderLine(writer, fullLine);
    }
    
    /// <summary>
    /// Writes a FITS integer keyword
    /// </summary>
    private static void WriteFitsInteger(BinaryWriter writer, string keyword, int value, string comment)
    {
        string keywordPart = keyword.PadRight(8);
        string valuePart = value.ToString().PadLeft(20);
        string commentPart = comment.PadRight(47);
        
        string fullLine = $"{keywordPart}= {valuePart}/ {commentPart}";
        WriteHeaderLine(writer, fullLine);
    }
    
    /// <summary>
    /// Writes a FITS float keyword
    /// </summary>
    private static void WriteFitsFloat(BinaryWriter writer, string keyword, double value, string comment)
    {
        string keywordPart = keyword.PadRight(8);
        string valuePart = value.ToString("F6", System.Globalization.CultureInfo.InvariantCulture).PadLeft(20);
        string commentPart = comment.PadRight(47);
        
        string fullLine = $"{keywordPart}= {valuePart}/ {commentPart}";
        WriteHeaderLine(writer, fullLine);
    }
    
    /// <summary>
    /// Writes a FITS string keyword
    /// </summary>
    private static void WriteFitsString(BinaryWriter writer, string keyword, string value, string comment)
    {
        string keywordPart = keyword.PadRight(8);
        string valuePart = $"'{value}'".PadLeft(20);
        string commentPart = comment.PadRight(47);
        
        string fullLine = $"{keywordPart}= {valuePart}/ {commentPart}";
        WriteHeaderLine(writer, fullLine);
    }
} 