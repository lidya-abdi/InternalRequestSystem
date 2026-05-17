using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class RequestLog
    {
        public int Id { get; set; }

        public int? RequestId { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty;

        public string PerformedBy { get; set; } = string.Empty;

        public DateTime ActionDate { get; set; } = DateTime.Now;

        public string Description { get; set; } = string.Empty;
    }
}