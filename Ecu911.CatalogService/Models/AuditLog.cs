namespace Ecu911.CatalogService.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
    }
}