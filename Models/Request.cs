namespace InternalRequestSystem.Models
{
    public class Request
    {
        public int Id { get; set; }

        public string? Title { get; set; } 

        public string? Description { get; set; } 

        public string? RequestType { get; set; } 

        public string Status { get; set; } = "Pending";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string SubmittedByName { get; set; } = string.Empty;
        public string SubmittedByEmail { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? UserId { get; set; }
        public AppUser? User { get; set; }

        public List<Approval> Approvals { get; set; } = new List<Approval>();
    }
}