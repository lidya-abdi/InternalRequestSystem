using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class RequestLog
    {
        public int Id { get; set; }

        public int? RequestId { get; set; }


        public string? Action { get; set; }

        public string? PerformedBy { get; set; } 

        public DateTime ActionDate { get; set; } = DateTime.Now;

        public string? Description { get; set; } 
    }
}