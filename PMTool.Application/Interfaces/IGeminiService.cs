using PMTool.Domain.Entities;

namespace PMTool.Application.Interfaces;

public interface IGeminiService
{
    Task<List<AiBacklogDraftItem>> GenerateBacklogItemsAsync(string documentText);
}
