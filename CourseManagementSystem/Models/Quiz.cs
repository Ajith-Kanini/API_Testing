using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        public string? QuizName { get; set; }
        public string? QuizImage { get; set; } = string.Empty;

        public string? QuizDifLevel { get; set; }

        public int? CourseId { get; set; }

        public virtual Course? Course { get; set; }
    }
}
