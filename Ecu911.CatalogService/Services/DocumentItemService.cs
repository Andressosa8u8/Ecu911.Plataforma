using Ecu911.CatalogService.DTOs;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Services
{
    public class DocumentItemService : IDocumentItemService
    {
        private readonly IDocumentItemRepository _repository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IRepositoryNodeRepository _repositoryNodeRepository;
        private readonly IDocumentFileService _documentFileService;
        private readonly AuditService _auditService;

        public DocumentItemService(
            IDocumentItemRepository repository,
            IDocumentTypeRepository documentTypeRepository,
            IRepositoryNodeRepository repositoryNodeRepository,
            IDocumentFileService documentFileService,
            AuditService auditService)
        {
            _repository = repository;
            _documentTypeRepository = documentTypeRepository;
            _repositoryNodeRepository = repositoryNodeRepository;
            _documentFileService = documentFileService;
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
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                throw new ArgumentException("El título del documento es obligatorio.");
            }

            var documentTypeExists = await _documentTypeRepository.ExistsAsync(input.DocumentTypeId);

            if (!documentTypeExists)
            {
                throw new ArgumentException("El tipo de documento seleccionado no existe o está inactivo.");
            }

            if (input.RepositoryNodeId == Guid.Empty)
            {
                throw new ArgumentException("El nodo del repositorio es obligatorio.");
            }

            var repositoryNodeExists = await _repositoryNodeRepository.ExistsAsync(input.RepositoryNodeId);

            if (!repositoryNodeExists)
            {
                throw new ArgumentException("El nodo del repositorio seleccionado no existe o está inactivo.");
            }

            var entity = new DocumentItem
            {
                Title = input.Title.Trim(),
                Description = input.Description?.Trim() ?? string.Empty,
                DocumentTypeId = input.DocumentTypeId,
                RepositoryNodeId = input.RepositoryNodeId,
                CreatedBy = username,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(entity);

            _auditService.LogAction(
                "Create",
                username ?? "Unknown",
                $"Created DocumentItem with title: {entity.Title}");

            return MapToDto(created);
        }

        public async Task<DocumentItemDto?> UpdateAsync(Guid id, UpdateDocumentItemDto input, string? username)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                throw new ArgumentException("El título del documento es obligatorio.");
            }

            var documentTypeExists = await _documentTypeRepository.ExistsAsync(input.DocumentTypeId);

            if (!documentTypeExists)
            {
                throw new ArgumentException("El tipo de documento seleccionado no existe o está inactivo.");
            }

            if (input.RepositoryNodeId == Guid.Empty)
            {
                throw new ArgumentException("El nodo del repositorio es obligatorio.");
            }

            var repositoryNodeExists = await _repositoryNodeRepository.ExistsAsync(input.RepositoryNodeId);

            if (!repositoryNodeExists)
            {
                throw new ArgumentException("El nodo del repositorio seleccionado no existe o está inactivo.");
            }

            var updated = await _repository.UpdateAsync(
                id,
                input.Title.Trim(),
                input.Description?.Trim() ?? string.Empty,
                input.DocumentTypeId,
                input.RepositoryNodeId,
                username);

            if (updated != null)
            {
                _auditService.LogAction(
                    "Update",
                    username ?? "Unknown",
                    $"Updated DocumentItem with ID: {id}");
            }

            return updated == null ? null : MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id, string? username)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
            {
                return false;
            }

            await _documentFileService.DeleteAsync(id, username);

            var deleted = await _repository.DeleteAsync(id, username);

            if (deleted)
            {
                _auditService.LogAction(
                    "Delete",
                    username ?? "Unknown",
                    $"Deleted DocumentItem with ID: {id}");
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
                DocumentTypeName = x.DocumentType?.Name ?? "Desconocido",
                RepositoryNodeId = x.RepositoryNodeId,
                RepositoryNodeName = x.RepositoryNode?.Name
            };
        }
    }
}