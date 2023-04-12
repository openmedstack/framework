// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferExtensions.cs" company="Reimers.dk">
//   Copyright © Reimers.dk
// </copyright>
// <summary>
//   Defines the buffer extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack
{
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the buffer extensions.
    /// </summary>
    public static class BufferExtensions
    {
        /// <summary>
        /// Reads the <see cref="Stream"/> into a <see cref="Task"/>.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> to read.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the operation.</param>
        /// <returns>The contents as an array of <see cref="byte"/>.</returns>
        public static async Task<byte[]> ReadToEnd(this Stream input, CancellationToken cancellationToken = default)
        {
            if (input is MemoryStream memoryInput)
            {
                return memoryInput.ToArray();
            }

            var ms = new MemoryStream();
            await using var _ = ms.ConfigureAwait(false);
            await input!.CopyToAsync(ms, 8192, cancellationToken).ConfigureAwait(false);
            return ms.ToArray();
        }

        /// <summary>
        /// Compresses the input bytes using gzip.
        /// </summary>
        /// <param name="inputData">The data to compress.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the operation.</param>
        /// <returns>The compressed bytes.</returns>
        public static async Task<byte[]> Compress(this byte[] inputData, CancellationToken cancellationToken = default)
        {
            var compressIntoMs = new MemoryStream();
            await using var _ = compressIntoMs.ConfigureAwait(false);
            var gzs = new GZipStream(compressIntoMs, CompressionMode.Compress);
            await using (gzs.ConfigureAwait(false))
            {
                await gzs.WriteAsync(inputData, 0, inputData.Length, cancellationToken).ConfigureAwait(false);
            }

            return compressIntoMs.ToArray();
        }

        /// <summary>
        /// Decompresses the input bytes using gzip.
        /// </summary>
        /// <param name="inputData">The data to decompress.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> for the operation.</param>
        /// <returns>The decompressed bytes.</returns>
        public static async Task<byte[]> Decompress(this byte[] inputData, CancellationToken cancellationToken = default)
        {
            var compressedMs = new MemoryStream(inputData);
            await using var _ = compressedMs.ConfigureAwait(false);
            var decompressedMs = new MemoryStream();
            await using var __ = decompressedMs.ConfigureAwait(false);
            var gzs = new GZipStream(compressedMs, CompressionMode.Decompress);
            await using (gzs.ConfigureAwait(false))
            {
                await gzs.CopyToAsync(decompressedMs, 8192, cancellationToken).ConfigureAwait(false);
            }

            return decompressedMs.ToArray();
        }
    }
}