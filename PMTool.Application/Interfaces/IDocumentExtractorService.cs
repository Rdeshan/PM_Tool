namespace PMTool.Application.Interfaces;

public interface IDocumentExtractorService
{
    /// <summary>Extracts plain text from .txt, .pdf, or .docx files.</summary>
    Task<string> ExtractTextAsync(Stream fileStream, string fileName);
}
