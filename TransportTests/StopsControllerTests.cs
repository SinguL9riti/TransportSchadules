using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSchadules.Controllers;
using TransportSchadules.Data;
using TransportSchadules.Models;
using TransportSchadules.ViewModels;
using Xunit;

namespace TransportTests
{
    public class StopsControllerTests
    {
        private DbContextOptions<TransportDbContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<TransportDbContext>()
                .UseInMemoryDatabase(databaseName: "TransportDb")
                .Options;
        }

        [Fact]
        public async Task Index_ReturnsViewWithStops()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new TransportDbContext(options);

            context.Stops.AddRange(new List<Stop>
            {
                new Stop { StopId = 1, Name = "Stop A", IsTerminal = false, HasDispatcher = false },
                new Stop { StopId = 2, Name = "Stop B", IsTerminal = true, HasDispatcher = true }
            });
            await context.SaveChangesAsync();

            var controller = new StopsController(context);

            // Act
            var result = await controller.Index(null, 1) as ViewResult;
            var viewModel = result?.Model as StopsViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(viewModel);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new TransportDbContext(options);

            var controller = new StopsController(context);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesStop_WhenIdIsValid()
        {
            // Arrange
            var options = GetDbContextOptions();
            using var context = new TransportDbContext(options);

            var stop = new Stop { StopId = 1, Name = "Stop A" };
            context.Stops.Add(stop);
            await context.SaveChangesAsync();

            var controller = new StopsController(context);

            // Act
            var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(controller.Index), result.ActionName);
            Assert.Empty(context.Stops);
        }
    }
}
