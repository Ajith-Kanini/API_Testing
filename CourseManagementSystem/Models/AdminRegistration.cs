using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class AdminRegistration
    {
        [Key]
        public int AdminId { get; set; }

        public string? AdminName { get; set; }

        public string? AdminMailId { get; set; }

        public string? AdminPassword { get; set; }
    }
}
