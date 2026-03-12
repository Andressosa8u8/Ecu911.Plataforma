using Ecu911.CatalogService.Data;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.CatalogService.Repositories
{
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
                .Include(x => x.DocumentType) // Incluimos DocumentType para evitar null en el DTO
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<DocumentItem?> GetByIdAsync(Guid id)
        {
            return await _context.DocumentItems
                .Include(x => x.DocumentType) // Incluimos DocumentType
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<DocumentItem> AddAsync(DocumentItem item)
        {
            _context.DocumentItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<DocumentItem?> UpdateAsync(Guid id, string title, string description, Guid documentTypeId, string? username)
        {
            var existing = await _context.DocumentItems
                .Include(x => x.DocumentType) // Incluimos DocumentType
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (existing == null)
                return null;

            existing.Title = title;
            existing.Description = description;
            existing.DocumentTypeId = documentTypeId;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = username;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id, string? username)
        {
            var existing = await _context.DocumentItems
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (existing == null)
                return false;

            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = username;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}