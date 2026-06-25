using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PMTool.Application.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PMTool.Application.Services.Ai;

public class DocumentExtractorService : IDocumentExtractorService
{
    public async Task<string> ExtractTextAsync(Stream fileStream, string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".txt" or ".md" => await ExtractTxtAsync(fileStream),
            ".pdf"          => ExtractPdf(fileStream),
            ".docx"         => ExtractDocx(fileStream),
            _ => throw new NotSupportedException(
                $"File type '{ext}' is not supported. Please upload .txt, .pdf, or .docx.")
        };
    }

    private static async Task<string> ExtractTxtAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private static string ExtractPdf(Stream stream)
    {
        var sb = new StringBuilder();
        using var pdf = PdfDocument.Open(stream);
        foreach (Page page in pdf.GetPages())
            sb.AppendLine(page.Text);
        return sb.ToString();
    }

    private static string ExtractDocx(Stream stream)
    {
        var sb = new StringBuilder();
        using var doc = WordprocessingDocument.Open(stream, isEditable: false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body == null) return string.Empty;

        foreach (var para in body.Descendants<Paragraph>())
            sb.AppendLine(para.InnerText);

        return sb.ToString();
    }
}
