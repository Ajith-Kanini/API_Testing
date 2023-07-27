using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Repository.CourseService
{
    public class CourseRepository: ICourseRepository
    {
        private readonly CourseQuizDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CourseRepository(CourseQuizDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<Course> UpdateCourseAsync(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return null;
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return course;
        }

        public async Task<Course> CreateCourseAsync([FromForm] Course course, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                course.CourseImage = fileName;

                _context.Courses.Add(course);
                _context.SaveChanges();

                return course;
            }
            catch (Exception ex)
            {
                // Rethrow the exception with additional information
                throw new Exception("Error occurred while posting the room.", ex);
            }
        }

        public async Task<Course> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return null;
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return course;
        }

        private bool CourseExists(int id)
        {
            return (_context.Courses?.Any(e => e.CourseId == id)).GetValueOrDefault();
        }
    }
}
