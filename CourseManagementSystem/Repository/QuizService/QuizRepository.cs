using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Repository.QuizService
{
    public class QuizRepository: IQuizRepository
    {
        private readonly CourseQuizDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public QuizRepository(CourseQuizDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesAsync()
        {
            return await _context.Quizzes.ToListAsync();
        }

        public async Task<Quiz> GetQuizByIdAsync(int id)
        {
            return await _context.Quizzes.FindAsync(id);
        }

        public async Task<Quiz> UpdateQuizAsync(int id, Quiz quiz)
        {
            if (id != quiz.QuizId)
            {
                return null;
            }

            _context.Entry(quiz).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuizExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return quiz;
        }

        public async Task<Quiz> CreateQuizAsync([FromForm] Quiz quiz, IFormFile imageFile)
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

                quiz.QuizImage = fileName;
                Course acc = await _context.Courses.FirstOrDefaultAsync(x => x.CourseId == quiz.CourseId);
                quiz.Course = acc;
                _context.Quizzes.Add(quiz);
                _context.SaveChanges();

                return quiz;
            }
            catch (Exception ex)
            {
                // Rethrow the exception with additional information
                throw new Exception("Error occurred while posting the room.", ex);
            }
        }

        public async Task<Quiz> DeleteQuizAsync(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return null;
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return quiz;
        }

        private bool QuizExists(int id)
        {
            return (_context.Quizzes?.Any(e => e.QuizId == id)).GetValueOrDefault();
        }
    }
}
