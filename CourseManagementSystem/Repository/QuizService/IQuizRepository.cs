using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.Repository.QuizService
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetQuizzesAsync();
        Task<Quiz> GetQuizByIdAsync(int id);
        Task<Quiz> UpdateQuizAsync(int id, Quiz quiz);
        Task<Quiz> CreateQuizAsync([FromForm] Quiz quiz, IFormFile imageFile);
        Task<Quiz> DeleteQuizAsync(int id);
    }
}
