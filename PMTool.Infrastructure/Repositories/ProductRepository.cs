using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMTool.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Project)
            .Include(p => p.ReleaseNotes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByProjectAndVersionAsync(Guid projectId, string versionName)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.VersionName == versionName);
    }

    public async Task<IEnumerable<Product>> GetByProjectAsync(Guid projectId)
    {
        return await _context.Products
            .Where(p => p.ProjectId == projectId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveByProjectAsync(Guid projectId)
    {
        return await _context.Products
            .Where(p => p.ProjectId == projectId && p.Status != 5) // Not Deprecated
            .OrderByDescending(p => p.PlannedReleaseDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetReleasedByProjectAsync(Guid projectId)
    {
        return await _context.Products
            .Where(p => p.ProjectId == projectId && p.Status == 4) // Released
            .OrderByDescending(p => p.ActualReleaseDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Product product)
    {
        try
        {
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        try
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> VersionExistsInProjectAsync(Guid projectId, string versionName)
    {
        return await _context.Products
            .AnyAsync(p => p.ProjectId == projectId && p.VersionName == versionName);
    }

    public async Task<IEnumerable<ReleaseNotes>> GetReleaseNotesAsync(Guid productId)
    {
        return await _context.ReleaseNotes
            .Where(rn => rn.ProductId == productId)
            .OrderByDescending(rn => rn.PublishedDate ?? rn.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> AddReleaseNotesAsync(ReleaseNotes releaseNotes)
    {
        try
        {
            releaseNotes.Id = Guid.NewGuid();
            releaseNotes.CreatedAt = DateTime.UtcNow;
            releaseNotes.UpdatedAt = DateTime.UtcNow;
            _context.ReleaseNotes.Add(releaseNotes);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateReleaseNotesAsync(ReleaseNotes releaseNotes)
    {
        try
        {
            releaseNotes.UpdatedAt = DateTime.UtcNow;
            _context.ReleaseNotes.Update(releaseNotes);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteReleaseNotesAsync(Guid releaseNotesId)
    {
        try
        {
            var releaseNotes = await _context.ReleaseNotes.FindAsync(releaseNotesId);
            if (releaseNotes == null)
                return false;

            _context.ReleaseNotes.Remove(releaseNotes);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> PublishReleaseNotesAsync(Guid releaseNotesId)
    {
        try
        {
            var releaseNotes = await _context.ReleaseNotes.FindAsync(releaseNotesId);
            if (releaseNotes == null)
                return false;

            releaseNotes.IsPublished = true;
            releaseNotes.PublishedDate = DateTime.UtcNow;
            releaseNotes.UpdatedAt = DateTime.UtcNow;
            _context.ReleaseNotes.Update(releaseNotes);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
