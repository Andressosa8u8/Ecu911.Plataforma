using Ecu911.CatalogService.Data;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.CatalogService.Repositories;

public class DocumentItemRepository : IDocumentItemRepository
{
    private readonly AppDbContext _context;

    public DocumentItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocumentItem>> GetAllAsync()
    {
        return await _context.DocumentItems
            .Include(x => x.DocumentType)
            .Include(x => x.RepositoryNode)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<DocumentItem?> GetByIdAsync(Guid id)
    {
        return await _context.DocumentItems
            .Include(x => x.DocumentType)
            .Include(x => x.RepositoryNode)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<DocumentItem> AddAsync(DocumentItem item)
    {
        _context.DocumentItems.Add(item);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(item.Id) ?? item;
    }

    public async Task<DocumentItem?> UpdateAsync(
        Guid id,
        string title,
        string description,
        Guid documentTypeId,
        Guid? repositoryNodeId,
        string? username)
    {
        var entity = await _context.DocumentItems
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (entity == null)
        {
            return null;
        }

        entity.Title = title;
        entity.Description = description;
        entity.DocumentTypeId = documentTypeId;
        entity.RepositoryNodeId = repositoryNodeId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = username;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var entity = await _context.DocumentItems
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (entity == null)
        {
            return false;
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = username;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsActiveByDocumentTypeIdAsync(Guid documentTypeId)
    {
        return await _context.DocumentItems
            .AnyAsync(x => x.DocumentTypeId == documentTypeId && !x.IsDeleted);
    }
}