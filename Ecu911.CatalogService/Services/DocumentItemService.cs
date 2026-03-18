using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;
using Ecu911.CatalogService.Services;  // Agregar el servicio de auditoría

namespace Ecu911.CatalogService.Services
{
    public class DocumentItemService : IDocumentItemService
    {
        private readonly IDocumentItemRepository _repository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly AuditService _auditService;

        public DocumentItemService(
            IDocumentItemRepository repository,
            IDocumentTypeRepository documentTypeRepository,
            AuditService auditService)
        {
            _repository = repository;
            _documentTypeRepository = documentTypeRepository;
            _auditService = auditService;
        }

        public async Task<PagedResultDto<DocumentItemDto>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            if (pageIndex <= 0)
            {
                throw new ArgumentException("El número de página debe ser mayor a 0.");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("El tamaño de página debe ser mayor a 0.");
            }

            var items = await _repository.GetAllAsync();

            var totalCount = items.Count;

            var pagedItems = items
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            return new PagedResultDto<DocumentItemDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Items = pagedItems
            };
        }

        public async Task<DocumentItemDto?> GetByIdAsync(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<DocumentItemDto> CreateAsync(CreateDocumentItemDto input, string? username)
        {

            var documentTypeExists = await _documentTypeRepository.ExistsAsync(input.DocumentTypeId);

            if (!documentTypeExists)
            {
                throw new ArgumentException("El tipo de documento seleccionado no existe o está inactivo.");
            }

            var entity = new DocumentItem
            {
                Title = input.Title,
                Description = input.Description,
                DocumentTypeId = input.DocumentTypeId,
                CreatedBy = username,
                CreatedAt = DateTime.UtcNow
            };

            // Guardar en la base de datos
            var created = await _repository.AddAsync(entity);

            // Auditoría
            _auditService.LogAction("Create", username ?? "Unknown", $"Created DocumentItem with title: {input.Title}");

            return MapToDto(created);
        }

        public async Task<DocumentItemDto?> UpdateAsync(Guid id, UpdateDocumentItemDto input, string? username)
        {
            var documentTypeExists = await _documentTypeRepository.ExistsAsync(input.DocumentTypeId);

            if (!documentTypeExists)
            {
                throw new ArgumentException("El tipo de documento seleccionado no existe o está inactivo.");
            }

            var updated = await _repository.UpdateAsync(id, input.Title, input.Description, input.DocumentTypeId, username);

            if (updated != null)
            {
                _auditService.LogAction("Update", username ?? "Unknown", $"Updated DocumentItem with ID: {id}");
            }

            return updated == null ? null : MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id, string? username)
        {
            var deleted = await _repository.DeleteAsync(id, username);

            if (deleted)
            {
                // Auditoría
                _auditService.LogAction("Delete", username ?? "Unknown", $"Deleted DocumentItem with ID: {id}");
            }

            return deleted;
        }

        private static DocumentItemDto MapToDto(DocumentItem x)
        {
            return new DocumentItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                DocumentTypeId = x.DocumentTypeId,
                DocumentTypeName = x.DocumentType?.Name ?? "Desconocido"
            };
        }
    }
}