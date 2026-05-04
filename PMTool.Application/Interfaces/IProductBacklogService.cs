using PMTool.Application.DTOs.Backlog;

namespace PMTool.Application.Interfaces;

public interface IProductBacklogService
{
    Task<List<ProductBacklogItemDTO>> GetBacklogItemsAsync(Guid productId, int? status);
    Task<ProductBacklogItemDTO?> CreateBacklogItemAsync(CreateProductBacklogItemRequest request);
    Task<ProductBacklogItemDTO?> UpdateBacklogFieldAsync(UpdateProductBacklogFieldRequest request);
    Task<bool> ReorderItemsAsync(Guid productId, List<ReorderProductBacklogItemRequest> items);
    Task<bool> DeleteItemAsync(Guid itemId);
    List<BacklogItemTypeDTO> GetBacklogItemTypes();
    List<BacklogItemStatusDTO> GetBacklogItemStatuses();
}
