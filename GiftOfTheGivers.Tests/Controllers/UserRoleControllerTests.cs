namespace GiftOfTheGivers.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GOTGF_Project.Controllers.Admin;
    using GOTGF_Project.Data;
    using GOTGF_Project.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class UserRoleControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<ILogger<UserRoleController>> _loggerMock;
        private readonly ApplicationDbContext _context;

        public UserRoleControllerTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _loggerMock = new Mock<ILogger<UserRoleController>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithUsers()
        {
            // Arrange
            var fakeUsers = new List<ApplicationUser>
        {
            new ApplicationUser { Id = "1", Email = "admin@gift.org", FullName = "Admin Tester" },
            new ApplicationUser { Id = "2", Email = "volunteer@gift.org", FullName = "Volunteer Tester" }
        }.AsQueryable();

            _userManagerMock.Setup(x => x.Users).Returns(fakeUsers);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Volunteer" });

            var controller = new UserRoleController(
                _context,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as UsersIndexViewModel;
            Assert.Equal(2, model.Users.Count);
        }

        //Example Negative Case
        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var controller = new UserRoleController(
                _context,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _loggerMock.Object
            );

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        //Example Edit Workflow
        [Fact]
        public async Task Edit_ValidRoleChange_RedirectsToIndex()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "test@gift.org", FullName = "Tester" };
            _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Volunteer" });
            _userManagerMock.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns("2");

            var controller = new UserRoleController(
                _context,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _loggerMock.Object
            );

            var model = new UserEditViewModel
            {
                Id = "1",
                SelectedRole = "Admin"
            };

            // Act
            var result = await controller.Edit(model) as RedirectToActionResult;

            // Assert
            Assert.Equal("Index", result.ActionName);
        }

    }
}

