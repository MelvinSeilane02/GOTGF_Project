namespace GiftOfTheGivers.Tests.Controllers
{
    using GOTGF_Project.Controllers;
    using GOTGF_Project.Data;
    using GOTGF_Project.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class VolunteerAssignmentsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsView_WithAssignments()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.VolunteerAssignments.Add(new VolunteerAssignment
            {
                AssignmentID = 1,
                VolunteerID = 1,
                ProjectID = 1,
                AssignedDate = DateTime.Now
            });
            context.SaveChanges();

            var controller = new VolunteerAssignmentsController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<VolunteerAssignment>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var context = GetInMemoryDbContext();
            var controller = new VolunteerAssignmentsController(context);

            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenValidId()
        {
            var context = GetInMemoryDbContext();
            context.VolunteerAssignments.Add(new VolunteerAssignment
            {
                AssignmentID = 10,
                VolunteerID = 1,
                ProjectID = 1,
                AssignedDate = DateTime.Now
            });
            context.SaveChanges();

            var controller = new VolunteerAssignmentsController(context);

            var result = await controller.Details(10);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<VolunteerAssignment>(viewResult.Model);
            Assert.Equal(10, model.AssignmentID);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var context = GetInMemoryDbContext();
            var controller = new VolunteerAssignmentsController(context);

            var assignment = new VolunteerAssignment
            {
                VolunteerID = 2,
                ProjectID = 3,
                AssignedDate = DateTime.Now
            };

            var result = await controller.Create(assignment);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ValidUpdate_RedirectsToIndex()
        {
            var context = GetInMemoryDbContext();
            context.VolunteerAssignments.Add(new VolunteerAssignment
            {
                AssignmentID = 1,
                VolunteerID = 2,
                ProjectID = 2,
                AssignedDate = DateTime.Now
            });
            context.SaveChanges();

            var controller = new VolunteerAssignmentsController(context);

            var updated = new VolunteerAssignment
            {
                AssignmentID = 1,
                VolunteerID = 2,
                ProjectID = 3,
                AssignedDate = DateTime.Now
            };

            var result = await controller.Edit(1, updated);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesAssignment_AndRedirects()
        {
            var context = GetInMemoryDbContext();
            context.VolunteerAssignments.Add(new VolunteerAssignment
            {
                AssignmentID = 5,
                VolunteerID = 1,
                ProjectID = 1
            });
            context.SaveChanges();

            var controller = new VolunteerAssignmentsController(context);

            var result = await controller.DeleteConfirmed(5);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Empty(context.VolunteerAssignments);
        }

        [Fact]
        public void Controller_Has_AuthorizeAttribute()
        {
            var attr = (AuthorizeAttribute)Attribute.GetCustomAttribute(
                typeof(VolunteerAssignmentsController),
                typeof(AuthorizeAttribute)
            );

            Assert.NotNull(attr);
            Assert.Contains("Admin", attr.Roles);
        }

    }
}

