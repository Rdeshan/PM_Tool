using PMTool.Application.DTOs.Backlog;

namespace PMTool.Application.Interfaces;

public interface IProductBacklogService
{
    Task<List<ProductBacklogItemDTO>> GetBacklogItemsAsync(Guid productId, int? status);
    Task<ProductBacklogItemDTO?> CreateBacklogItemAsync(CreateProductBacklogItemRequest request);
    Task<ProductBacklogItemDTO?> UpdateBacklogFieldAsync(UpdateProductBacklogFieldRequest request);
    Task<BacklogSubtaskDto?> CreateSubtaskAsync(Guid parentId, CreateBacklogSubtaskDto request);
    Task<bool> UpdateSubtaskAsync(Guid subtaskId, CreateBacklogSubtaskDto request);
    Task<bool> UpdateSubtaskStatusAsync(Guid subtaskId, int status);
    Task<bool> DeleteSubtaskAsync(Guid subtaskId);
    Task<bool> ReorderItemsAsync(Guid productId, List<ReorderProductBacklogItemRequest> items);
    Task<bool> DeleteItemAsync(Guid itemId);
    List<BacklogItemTypeDTO> GetBacklogItemTypes();
    List<BacklogItemStatusDTO> GetBacklogItemStatuses();
}
