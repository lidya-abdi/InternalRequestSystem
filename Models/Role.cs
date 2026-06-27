using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class Role
    {

        public int Id { get; set; }
        
        public string? RoleName { get; set; } 
        public List<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
