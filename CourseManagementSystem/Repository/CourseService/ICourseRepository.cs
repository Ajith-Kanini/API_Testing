using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.Repository.CourseService
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<Course> GetCourseByIdAsync(int id);
        Task<Course> UpdateCourseAsync(int id, Course course);
        Task<Course> CreateCourseAsync([FromForm] Course course, IFormFile imageFile);
        Task<Course> DeleteCourseAsync(int id);
    }
}
