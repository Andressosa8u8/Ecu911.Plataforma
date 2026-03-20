using Ecu911.CatalogService.Data;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.CatalogService.Repositories;

public class DownloadAuditRepository : IDownloadAuditRepository
{
    private readonly AppDbContext _context;

    public DownloadAuditRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DownloadAudit>> GetAllAsync()
    {
        return await _context.DownloadAudits
            .Include(x => x.DocumentItem)
            .Include(x => x.DocumentFile)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<List<DownloadAudit>> GetByDocumentItemIdAsync(Guid documentItemId)
    {
        return await _context.DownloadAudits
            .Include(x => x.DocumentItem)
            .Include(x => x.DocumentFile)
            .Where(x => x.DocumentItemId == documentItemId)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<List<DownloadAudit>> GetByDocumentFileIdAsync(Guid documentFileId)
    {
        return await _context.DownloadAudits
            .Include(x => x.DocumentItem)
            .Include(x => x.DocumentFile)
            .Where(x => x.DocumentFileId == documentFileId)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<DownloadAudit> AddAsync(DownloadAudit entity)
    {
        _context.DownloadAudits.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}