using CourseManagementSystem.Models;
using CourseManagementSystem.Repository.CourseService;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CourseManagementTesting
{
    public class UnitTest1
    {
        private readonly Mock<ICourseRepository> mockRepo;
        public UnitTest1()
        {
            mockRepo = new Mock<ICourseRepository>();
        }
        [Fact]
        public void Get_Course_Test()
        {
            //Arrange
            var expCourses = new List<Course>
            {
                new Course{CourseId=111,CourseName="Javascript",CourseFees=67890},
                new Course{CourseId=112,CourseName="SQL",CourseFees=6780},
                new Course{CourseId=113,CourseName="HTML", CourseFees = 6790}
            };

            mockRepo.Setup(repo=>repo.GetCoursesAsync()).ReturnsAsync(expCourses);

            // Act
            var actualCourses = mockRepo.Object.GetCoursesAsync().Result;

            // Assert
            Assert.Equal(expCourses, actualCourses);
        }

        [Fact]
        public async Task GetCourseByIdAsync_ShouldReturnCourse()
        {
            // Arrange
            int courseId = 111;
            var expectedCourse = new Course { CourseId = courseId, CourseName = "Javascript", CourseFees = 67890 };

            mockRepo.Setup(repo => repo.GetCourseByIdAsync(courseId)).ReturnsAsync(expectedCourse);

            var repository = mockRepo.Object;

            // Act
            var actualCourse = await repository.GetCourseByIdAsync(courseId);

            // Assert
            Assert.Equal(expectedCourse, actualCourse);
        }

        [Fact]
        public async Task UpdateCourseAsync_ShouldReturnUpdatedCourse()
        {
            // Arrange
            int courseId = 111;
            var updatedCourse = new Course { CourseId = courseId, CourseName = "Python", CourseFees = 5000 };
            var expectedCourse = new Course { CourseId = courseId, CourseName = "Python ", CourseFees = 5000 };

            mockRepo.Setup(repo => repo.UpdateCourseAsync(courseId, updatedCourse)).ReturnsAsync(expectedCourse);

            var repository = mockRepo.Object;

            // Act
            var actualCourse = await repository.UpdateCourseAsync(courseId, updatedCourse);

            // Assert
            Assert.Equal(expectedCourse, actualCourse);
        }

        [Fact]
        public async Task CreateCourseAsync_ShouldReturnCreatedCourse()
        {
            // Arrange
            var newCourse = new Course { CourseName = ".Net", CourseFees = 10000 };
            var imageFile = new Mock<IFormFile>().Object;
            var expectedCourse = new Course { CourseId = 1, CourseName = ".Net", CourseFees = 10000 };

            mockRepo.Setup(repo => repo.CreateCourseAsync(newCourse, imageFile)).ReturnsAsync(expectedCourse);

            var repository = mockRepo.Object;

            // Act
            var actualCourse = await repository.CreateCourseAsync(newCourse, imageFile);

            // Assert
            Assert.Equal(expectedCourse, actualCourse);
        }

        [Fact]
        public async Task DeleteCourseAsync_ShouldReturnDeletedCourse()
        {
            // Arrange
            int courseId = 111;
            var expectedCourse = new Course { CourseId = courseId, CourseName = "Java", CourseFees = 5000 };

            mockRepo.Setup(repo => repo.DeleteCourseAsync(courseId)).ReturnsAsync(expectedCourse);

            var repository = mockRepo.Object;

            // Act
            var actualCourse = await repository.DeleteCourseAsync(courseId);

            // Assert
            Assert.Equal(expectedCourse, actualCourse);
        }


        [Fact]
        public async Task GetCourseByIdAsync_ShouldReturnCourse1()
        {
            // Arrange
            int courseId = 111;
            var expectedCourse = new Course { CourseId = courseId, CourseName = "Javascript", CourseFees = 67890 };

            mockRepo.Setup(repo => repo.GetCourseByIdAsync(courseId)).ReturnsAsync(expectedCourse);

            var repository = mockRepo.Object;

            // Act
            var actualCourse = await repository.GetCourseByIdAsync(courseId);

            // Assert
            Assert.Equal(expectedCourse, actualCourse);
            Assert.NotNull(actualCourse);
        }

        [Fact]
        public async Task UpdateCourseAsync_ShouldReturnUpdatedCourse1()
        {
            // Arrange
            int courseId = 111;
            var updatedCourse = new Course { CourseId = courseId, CourseName = "Updated Course", CourseFees = 5000 };
            var expectedCourse = new Course { CourseId = courseId, CourseName = "Updated Course", CourseFees = 5000 };

            mockRepo.Setup(repo => repo.UpdateCourseAsync(courseId, updatedCourse)).ReturnsAsync(expectedCourse);

            var repository = mockRepo.Object;

            // Act
            var actualCourse = await repository.UpdateCourseAsync(courseId, updatedCourse);

            // Assert
            Assert.Equal(expectedCourse, actualCourse);
            Assert.Contains(actualCourse.CourseName, "Updated Course");
        }

        [Fact]
        public async Task CreateCourseAsync_ShouldReturnCreatedCourse1()
        {
            // Arrange
            var newCourse = new Course { CourseName = "New Course", CourseFees = 10000 };
            var imageFile = new Mock<IFormFile>().Object;
            var expectedCourse = new Course { CourseId = 1, CourseName = "New Course", CourseFees = 10000 };

            mockRepo.Setup(repo => repo.CreateCourseAsync(newCourse, imageFile)).ReturnsAsync(expectedCourse);

            var repository = mockRepo.Object;

            // Act
            var actualCourse = await repository.CreateCourseAsync(newCourse, imageFile);

            // Assert
            Assert.Equal(expectedCourse, actualCourse);
            Assert.NotNull(actualCourse);
        }
    }
}