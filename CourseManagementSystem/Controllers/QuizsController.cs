using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Models;
using CourseManagementSystem.Repository.QuizService;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CourseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizsController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;

        public QuizsController(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        // GET: api/Quizs
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        {
            var quizzes = await _quizRepository.GetQuizzesAsync();

            if (quizzes == null)
            {
                return NotFound();
            }

            return Ok(quizzes);
        }

        // GET: api/Quizs/5
        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuiz(int id)
        {
            var quiz = await _quizRepository.GetQuizByIdAsync(id);

            if (quiz == null)
            {
                return NotFound();
            }

            return Ok(quiz);
        }

        // PUT: api/Quizs/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuiz(int id, Quiz quiz)
        {
            if (id != quiz.QuizId)
            {
                return BadRequest();
            }

            var updatedQuiz = await _quizRepository.UpdateQuizAsync(id, quiz);

            if (updatedQuiz == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Quizs
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Quiz>> PostQuiz([FromForm] Quiz quiz, IFormFile imageFile)
        {
            var createdQuiz = await _quizRepository.CreateQuizAsync(quiz, imageFile);

            return CreatedAtAction("GetQuiz", new { id = createdQuiz.QuizId }, createdQuiz);
        }

        // DELETE: api/Quizs/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var deletedQuiz = await _quizRepository.DeleteQuizAsync(id);

            if (deletedQuiz == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
