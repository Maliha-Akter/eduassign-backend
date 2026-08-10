using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using MongoDB.Driver;
using EduAssign.API.Services;
using EduAssign.API.Models;
using EduAssign.API.DTOs.Assignments;
using System.Threading;

namespace EduAssign.Tests
{
    public class AssignmentServiceTests
    {
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<IMongoCollection<Assignment>> _mockCollection;
        private readonly AssignmentService _service;

        public AssignmentServiceTests()
        {
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<Assignment>>();

            _mockDatabase.Setup(db => db.GetCollection<Assignment>("assignments", null))
                         .Returns(_mockCollection.Object);

            _service = new AssignmentService(_mockDatabase.Object);
        }

        [Fact]
        public async Task CreateAssignmentAsync_Should_Create_Published_Assignment()
        {
            // Arrange
            var teacherId = "teacher_123";
            var request = new CreateAssignmentRequest
            {
                Title = "Math Homework",
                Description = "Solve 1-10",
                ClassId = "class_1",
                SubjectId = "sub_1",
                Deadline = DateTime.UtcNow.AddDays(7),
                MaximumMarks = 100,
                Status = "Published"
            };

            // Act
            var result = await _service.CreateAssignmentAsync(request, teacherId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Published", result.Status);
            Assert.Equal(teacherId, result.TeacherId);
            
            // Verify MongoDB insert was called exactly once
            _mockCollection.Verify(c => c.InsertOneAsync(
                It.IsAny<Assignment>(), 
                null, 
                default(CancellationToken)), 
            Times.Once);
        }

        [Fact]
        public async Task CreateAssignmentAsync_Should_Create_Draft_Assignment()
        {
            // Arrange
            var teacherId = "teacher_123";
            var request = new CreateAssignmentRequest
            {
                Title = "Draft Assignment",
                Description = "Draft Desc",
                ClassId = "class_1",
                SubjectId = "sub_1",
                Deadline = DateTime.UtcNow.AddDays(3),
                MaximumMarks = 50,
                Status = "Draft"
            };

            // Act
            var result = await _service.CreateAssignmentAsync(request, teacherId);

            // Assert
            Assert.Equal("Draft", result.Status);
        }
    }
}