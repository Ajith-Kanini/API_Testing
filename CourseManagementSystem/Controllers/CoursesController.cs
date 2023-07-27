using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Models;
using CourseManagementSystem.Repository.CourseService;
using Microsoft.AspNetCore.Authorization;
using CourseManagementSystem.Models.DTO;
using System.Diagnostics;

namespace CourseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;

        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // GET: api/Courses
       // [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var courses = await _courseRepository.GetCoursesAsync();

            if (courses == null)
            {
                return NotFound();
            }

            return Ok(courses);
        }

        // GET: api/Courses/5
       // [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);

            if (course == null)
            {
                return BadRequest(new ErrorDTO { ErrorId = 1, ErrorStatusCode = 404, ErrorMessage = "No course found" });
            }

            return Ok(course);
        }

        // PUT: api/Courses/5
        // [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest(new ErrorDTO { ErrorId = 1, ErrorStatusCode = 400, ErrorMessage = "Course ID mismatch" });
            }

            var existingCourse = await _courseRepository.GetCourseByIdAsync(id);
            if (existingCourse == null)
            {
                return BadRequest(new ErrorDTO { ErrorId = 2, ErrorStatusCode = 404, ErrorMessage = "Course not found" });
            }

            var updatedCourse = await _courseRepository.UpdateCourseAsync(id,course);
            if (updatedCourse == null)
            {
                return NotFound();
            }

            return NoContent();
        }


        // POST: api/Courses
        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse([FromForm] Course course, IFormFile imageFile)
        {
            var createdCourse = await _courseRepository.CreateCourseAsync(course, imageFile);

            return CreatedAtAction("GetCourse", new { id = createdCourse.CourseId }, createdCourse);
        }

        // DELETE: api/Courses/5
       // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var deletedCourse = await _courseRepository.DeleteCourseAsync(id);

            if (deletedCourse == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
