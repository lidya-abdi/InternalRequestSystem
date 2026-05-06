namespace InternalRequestSystem.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public List<AppUser> Users { get; set; } = new List<AppUser>();

        public List<Request> Requests { get; set; } = new List<Request>();
    }
}
