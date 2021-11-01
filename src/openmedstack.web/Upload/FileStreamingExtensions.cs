namespace OpenMedStack.Web.Upload
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Text;
    using System.Threading;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Net.Http.Headers;

    internal static class FileStreamingExtensions
    {
        private static readonly FormOptions DefaultFormOptions = new();

        public static async IAsyncEnumerable<DataContent> HandleStream(
            this HttpRequest request,
            [EnumeratorCancellation] CancellationToken cancellation)
        {
            if (!request.ContentType!.IsMultipartContentType())
            {
                yield break;
            }

            var reader = CreateMultipartReader(request);
            var section = await reader.ReadNextSectionAsync(cancellation).ConfigureAwait(false);

            while (section != null)
            {
                //cancellationToken.ThrowIfCancellationRequested();
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                    section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (contentDisposition!.HasFileContentDisposition())
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition!.FileName.Value);

                        // **WARNING!**
                        // In the following example, the file is saved without
                        // scanning the file's contents. In most production
                        // scenarios, an anti-virus/anti-malware scanner API
                        // is used on the file before making the file available
                        // for download or for use by other systems.
                        // For more information, see the topic that accompanies
                        // this sample.

                        var encoding = GetEncoding(section);
                        var owner = request.HttpContext.User.Claims
                            .FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier)
                            ?.Value ?? string.Empty;

                        yield return new DataContent(owner, encoding, section.Body, trustedFileNameForDisplay, section.ContentType!);
                    }
                    else if (contentDisposition!.HasFormDataContentDisposition())
                    {
                        continue;
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync(cancellation).ConfigureAwait(false);
            }
        }

        private static MultipartReader CreateMultipartReader(HttpRequest request)
        {
            var boundary = MediaTypeHeaderValue.Parse(request.ContentType).GetBoundary(DefaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);
            return reader;
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in
            // most cases.
            return !hasMediaTypeHeader || mediaType?.Encoding == null ? Encoding.UTF8 : mediaType.Encoding;
        }
    }
}
