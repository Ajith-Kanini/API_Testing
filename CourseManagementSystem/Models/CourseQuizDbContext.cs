using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Models
{
    public class CourseQuizDbContext:DbContext
    {
        public  DbSet<Course> Courses { get; set; }

        public  DbSet<Quiz> Quizzes { get; set; }
        public  DbSet<MyCart> MyCarts { get; set; }

        public  DbSet<UserRegistration> UserRegistrations { get; set; }
        public  DbSet<AdminRegistration> AdminRegistrations { get; set; }
        public CourseQuizDbContext(DbContextOptions<CourseQuizDbContext> options) : base(options)
        {

        }
    }
}
