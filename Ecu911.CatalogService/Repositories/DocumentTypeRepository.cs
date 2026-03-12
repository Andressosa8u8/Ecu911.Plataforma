using Ecu911.CatalogService.Data;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.CatalogService.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly AppDbContext _context;

    public DocumentTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocumentType>> GetAllAsync()
    {
        return await _context.DocumentTypes
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<DocumentType?> GetByIdAsync(Guid id)
    {
        return await _context.DocumentTypes
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<DocumentType> AddAsync(DocumentType entity)
    {
        _context.DocumentTypes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}