namespace GiftOfTheGivers.Tests.Controllers
{
    using GOTGF_Project.Controllers;
    using GOTGF_Project.Data;
    using GOTGF_Project.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class DonationsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithDonations()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Donations.Add(new Donation { DonationID = 1, Amount = 100 });
            context.SaveChanges();

            var controller = new DonationsController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Donation>>(viewResult.Model);
            Assert.Single(model); // one donation added
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var context = GetInMemoryDbContext();
            var controller = new DonationsController(context);

            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Post_RedirectsToIndex_WhenModelIsValid()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new DonationsController(context);
            var donation = new Donation
            {
                DonationID = 1,
                Amount = 200,
                DonationDate = DateTime.Now,
                UserId = "user1",
                ProjectID = 1
            };

            // Act
            var result = await controller.Create(donation);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesDonation_AndRedirects()
        {
            var context = GetInMemoryDbContext();
            context.Donations.Add(new Donation { DonationID = 1, Amount = 300 });
            context.SaveChanges();

            var controller = new DonationsController(context);

            var result = await controller.DeleteConfirmed(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Empty(context.Donations); // donation deleted
        }

        [Fact]
        public void DonationsController_HasAuthorizeAttribute()
        {
            var attr = (AuthorizeAttribute)Attribute.GetCustomAttribute(
                typeof(DonationsController),
                typeof(AuthorizeAttribute)
            );
            Assert.NotNull(attr);
        }
    }
}

