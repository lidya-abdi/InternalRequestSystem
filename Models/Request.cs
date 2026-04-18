using System.ComponentModel.DataAnnotations;

namespace InternalRequestSystem.Models
{
    public class Request
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Request type is required")]
        public string RequestType { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
    }
}
