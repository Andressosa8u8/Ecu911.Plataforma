namespace Ecu911.CatalogService.DTOs
{
    public class DocumentItemDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid DocumentTypeId { get; set; }
        public string? DocumentTypeName { get; set; }
    }
}