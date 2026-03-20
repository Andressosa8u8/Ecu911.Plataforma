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
        private readonly INodeAccessService _nodeAccessService;
        private readonly IDocumentFileRepository _documentFileRepository;

        public DocumentItemService(
            IDocumentItemRepository repository,
            IDocumentTypeRepository documentTypeRepository,
            IRepositoryNodeRepository repositoryNodeRepository,
            IDocumentFileService documentFileService,
            IDocumentFileRepository documentFileRepository,
            AuditService auditService,
            INodeAccessService nodeAccessService)
        {
            _repository = repository;
            _documentTypeRepository = documentTypeRepository;
            _repositoryNodeRepository = repositoryNodeRepository;
            _documentFileService = documentFileService;
            _documentFileRepository = documentFileRepository;
            _auditService = auditService;
            _nodeAccessService = nodeAccessService;
        }

        public async Task<PagedResultDto<DocumentItemDto>> GetAllAsync(
            DocumentItemFilterDto filter,
            int pageIndex = 1,
            int pageSize = 10,
            bool isAdmin = false,
            Guid? organizationalUnitId = null)
        {
            if (pageIndex <= 0)
            {
                throw new ArgumentException("El número de página debe ser mayor a 0.");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("El tamaño de página debe ser mayor a 0.");
            }

            filter ??= new DocumentItemFilterDto();

            var items = await _repository.GetAllAsync();

            if (!isAdmin)
            {
                var readableNodeIds = await _nodeAccessService.GetReadableNodeIdsAsync(isAdmin, organizationalUnitId);

                items = items
                    .Where(x => x.RepositoryNodeId.HasValue && readableNodeIds.Contains(x.RepositoryNodeId.Value))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                var normalizedTitle = filter.Title.Trim().ToLower();

                items = items
                    .Where(x => !string.IsNullOrWhiteSpace(x.Title) &&
                                x.Title.ToLower().Contains(normalizedTitle))
                    .ToList();
            }

            if (filter.DocumentTypeId.HasValue)
            {
                items = items
                    .Where(x => x.DocumentTypeId == filter.DocumentTypeId.Value)
                    .ToList();
            }

            if (filter.RepositoryNodeId.HasValue)
            {
                items = items
                    .Where(x => x.RepositoryNodeId.HasValue &&
                                x.RepositoryNodeId.Value == filter.RepositoryNodeId.Value)
                    .ToList();
            }

            if (filter.CreatedFrom.HasValue)
            {
                var createdFrom = filter.CreatedFrom.Value.Date;

                items = items
                    .Where(x => x.CreatedAt.Date >= createdFrom)
                    .ToList();
            }

            if (filter.CreatedTo.HasValue)
            {
                var createdTo = filter.CreatedTo.Value.Date;

                items = items
                    .Where(x => x.CreatedAt.Date <= createdTo)
                    .ToList();
            }

            items = items
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

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

        public async Task<DocumentItemDto?> GetByIdAsync(Guid id, bool isAdmin, Guid? organizationalUnitId)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                return null;
            }

            if (!item.RepositoryNodeId.HasValue)
            {
                throw new ArgumentException("El documento no tiene un nodo de repositorio asociado.");
            }

            var canView = await _nodeAccessService.CanViewNodeAsync(
                item.RepositoryNodeId.Value,
                isAdmin,
                organizationalUnitId);

            if (!canView)
            {
                throw new UnauthorizedAccessException("No tiene permisos para visualizar este documento.");
            }

            return MapToDto(item);
        }

        public async Task<DocumentItemDto> CreateAsync(CreateDocumentItemDto input, string? username, bool isAdmin, Guid? organizationalUnitId)
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

            var canManage = await _nodeAccessService.CanManageNodeAsync(input.RepositoryNodeId, isAdmin, organizationalUnitId);

            if (!canManage)
            {
                throw new UnauthorizedAccessException("No tiene permisos para crear documentos en este nodo.");
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

        public async Task<DocumentItemDto?> UpdateAsync(Guid id, UpdateDocumentItemDto input, string? username, bool isAdmin, Guid? organizationalUnitId)
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

            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
            {
                return null;
            }

            var canManageTargetNode = await _nodeAccessService.CanManageNodeAsync(input.RepositoryNodeId, isAdmin, organizationalUnitId);

            if (!canManageTargetNode)
            {
                throw new UnauthorizedAccessException("No tiene permisos para editar documentos en este nodo.");
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

        public async Task<bool> DeleteAsync(Guid id, string? username, bool isAdmin, Guid? organizationalUnitId)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
            {
                return false;
            }

            if (!existing.RepositoryNodeId.HasValue)
            {
                throw new ArgumentException("El documento no tiene un nodo de repositorio asociado.");
            }

            var canDelete = await _nodeAccessService.CanDeleteFromNodeAsync(
                existing.RepositoryNodeId.Value,
                isAdmin,
                organizationalUnitId);

            if (!canDelete)
            {
                throw new UnauthorizedAccessException("No tiene permisos para eliminar documentos en este nodo.");
            }

            var existingFile = await _documentFileRepository.GetByDocumentItemIdAsync(id);

            if (existingFile != null)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el documento porque tiene un archivo adjunto activo. Elimine primero el archivo asociado.");
            }

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