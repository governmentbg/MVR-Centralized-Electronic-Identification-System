using System.Diagnostics;
using System.Text;

namespace eID.PJS.LocalLogsSearch.Service;

/// <summary>
/// A set of extension methods for <see cref="Stream"/>.
/// </summary>
public static class StreamExtensions
{
    private const char CR = '\r';
    private const char LF = '\n';
    private const char NULL = (char)0;

    /// <summary>
    /// Returns the number of lines in the given <paramref name="stream"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static long CountLines(this Stream stream, Encoding? encoding = default)
    {
        if (stream == null) throw new ArgumentException(nameof(stream));
        if (encoding == null) throw new ArgumentException(nameof(encoding));

        stream.Position = 0;

        var lineCount = 0L;
        var byteBuffer = new byte[1024 * 1024];
        var detectedEOL = NULL;
        var currentChar = NULL;
        int bytesRead;

        if (encoding is null || Equals(encoding, Encoding.ASCII) || Equals(encoding, Encoding.UTF8))
        {
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                for (var i = 0; i < bytesRead; i++)
                {
                    currentChar = (char)byteBuffer[i];

                    if (detectedEOL != NULL)
                    {
                        if (currentChar == detectedEOL)
                        {
                            lineCount++;
                        }

                    }
                    else if (currentChar == LF || currentChar == CR)
                    {
                        detectedEOL = currentChar;
                        lineCount++;
                    }
                }
            }
        }
        else
        {
            var charBuffer = new char[byteBuffer.Length];

            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                var charCount = encoding.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0);

                for (var i = 0; i < charCount; i++)
                {
                    currentChar = charBuffer[i];

                    if (detectedEOL != NULL)
                    {
                        if (currentChar == detectedEOL)
                        {
                            lineCount++;
                        }
                    }
                    else if (currentChar == LF || currentChar == CR)
                    {
                        detectedEOL = currentChar;
                        lineCount++;
                    }
                }
            }
        }

        if (currentChar != LF && currentChar != CR && currentChar != NULL)
        {
            lineCount++;
        }

        return lineCount;
    }

    /// <summary>
    /// Determines a text file's encoding by analyzing its byte order mark (BOM).
    /// Defaults to null when detection of the text file's endianness fails.
    /// </summary>
    /// <param name="filename">The text file to analyze.</param>
    /// <returns>The detected encoding.</returns>
    public static Encoding? GetEncoding(this FileStream stream)
    {
        // Read the BOM

        var curPosition = stream.Position;

        var bom = new byte[4];
        stream.Read(bom, 0, 4);

        stream.Position = curPosition;

        // Analyze the BOM
#pragma warning disable SYSLIB0001 // Type or member is obsolete
        if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
#pragma warning restore SYSLIB0001 // Type or member is obsolete
        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
        if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
        if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
        if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

        // We actually have no idea what the encoding is if we reach this point, so we return null
        return null;
    }

    /// <summary>
    /// Determines a text file's encoding by analyzing its byte order mark (BOM).
    /// Defaults to ASCII when detection of the text file's endianness fails.
    /// </summary>
    /// <param name="filename">The text file to analyze.</param>
    /// <returns>The detected encoding.</returns>
    public static Encoding? GetEncoding(string filename)
    {
        // Read the BOM
        Encoding? result;

        using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            result = file.GetEncoding();
        }

        return result;
    }
}


