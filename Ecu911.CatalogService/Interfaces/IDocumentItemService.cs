using Ecu911.CatalogService.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecu911.CatalogService.Interfaces
{
    public interface IDocumentItemService
    {
        Task<PagedResultDto<DocumentItemDto>> GetAllAsync(int pageIndex = 1, int pageSize = 10);
        Task<DocumentItemDto?> GetByIdAsync(Guid id);
        Task<DocumentItemDto> CreateAsync(CreateDocumentItemDto input, string? username);
        Task<DocumentItemDto?> UpdateAsync(Guid id, UpdateDocumentItemDto input, string? username);
        Task<bool> DeleteAsync(Guid id, string? username);
    }
}