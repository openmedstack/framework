namespace OpenMedStack.Web.Upload
{
    using System.IO;
    using System.Text;

    internal record DataContent(string Owner, Encoding Encoding, Stream Content, string FileName, string MimeType);
}
