namespace OpenMedStack.Web.Upload
{
    using System;
    using System.IO;
    using Microsoft.Net.Http.Headers;

    internal static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
        public static string GetBoundary(this MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }

        public static bool IsMultipartContentType(this string contentType) =>
            !string.IsNullOrEmpty(contentType)
            && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;

        public static bool HasFormDataContentDisposition(this ContentDispositionHeaderValue contentDisposition) =>
            // Content-Disposition: form-data; name="key";
            contentDisposition != null
            && contentDisposition.DispositionType.Equals("form-data")
            && string.IsNullOrEmpty(contentDisposition.FileName.Value)
            && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);

        public static bool HasFileContentDisposition(this ContentDispositionHeaderValue contentDisposition) =>
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            contentDisposition != null
            && contentDisposition.DispositionType.Equals("form-data")
            && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
    }
}