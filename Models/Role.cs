using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class Role
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Role name is required.")]
        public string RoleName { get; set; } = string.Empty;
        public List<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
