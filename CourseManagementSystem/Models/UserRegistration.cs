using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class UserRegistration
    {
        [Key]
        public int UserId { get; set; }

        public string? UserName { get; set; }

        public string? MailId { get; set; }

        public string? Password { get; set; }
    }
}
