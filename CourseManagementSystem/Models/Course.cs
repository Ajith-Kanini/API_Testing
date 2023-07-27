using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        public string? CourseName { get; set; }
        public string? CourseImage { get; set; }=string.Empty;
        public string? CourseDuration { get; set; }

        public decimal? CourseFees { get; set; }

        public int? CourseRating { get; set; }

        public virtual ICollection<Quiz>? Quizzes { get; set; } = new List<Quiz>();
    }
}
