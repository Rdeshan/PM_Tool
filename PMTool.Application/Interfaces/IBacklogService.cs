using PMTool.Application.DTOs.Backlog;

namespace PMTool.Application.Interfaces;

public interface IBacklogService
{
    Task<List<BacklogItemDTO>> GetBacklogItemsAsync(Guid projectId, Guid? productId, Guid? subProjectId, int? status);
    Task<BacklogItemDTO?> CreateBacklogItemAsync(CreateBacklogItemRequest request);
    Task<BacklogItemDTO?> UpdateBacklogFieldAsync(UpdateBacklogFieldRequest request);
    Task<bool> ReorderItemsAsync(Guid projectId, Guid? productId, List<ReorderBacklogItemRequest> items);
    Task<bool> DeleteItemAsync(Guid itemId);
}
