// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the StreamExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack;

using System.IO;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Defines the extension methods for <see cref="Stream"/>.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Reads the contents of the <see cref="Stream"/> as a <see cref="string"/>.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="encoding">The stream <see cref="Encoding"/>.</param>
    /// <returns>The <see cref="string"/> output.</returns>
    public static Task<string> ReadAsString(this Stream stream, Encoding? encoding = null)
    {
        using var reader = new StreamReader(
            stream,
            encoding ?? Encoding.UTF8,
            true,
            4096,
            true);
        return reader.ReadToEndAsync();
    }

    public static void Write(this Stream stream, string value, Encoding? encoding = null)
    {
        var buffer = (encoding ?? Encoding.UTF8).GetBytes(value);
        Write(stream, buffer);
    }

    public static void Write(this Stream stream, byte[] buffer)
    {
        stream.Write(buffer, 0, buffer.Length);
    }

    public static async Task WriteAsync(this Stream stream, byte[] buffer)
    {
        await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
    }
}