using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class Approval
    {
        public int Id { get; set; }

        public int RequestId { get; set; }
        public Request? Request { get; set; }

        public int ApprovedByUserId { get; set; }
        public AppUser? ApprovedByUser { get; set; }

        [Required]
        public string Decision { get; set; } = "Pending";

        public DateTime DecisionDate { get; set; } = DateTime.Now;

        public string? Comment { get; set; }
    }
}