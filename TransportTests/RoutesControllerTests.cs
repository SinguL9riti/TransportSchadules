using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TransportSchadules.Controllers;
using TransportSchadules.Data;
using TransportSchadules.Models;
using TransportSchadules.ViewModels;
using Xunit;

namespace TransportTests
{
    public class RoutesControllerTests
    {
        private readonly TransportDbContext _context;
        private readonly RoutesController _controller;

        public RoutesControllerTests()
        {
            // Создаем контекст с InMemoryDatabase
            var options = new DbContextOptionsBuilder<TransportDbContext>()
                .UseInMemoryDatabase(databaseName: "TestRoutesDatabase")
                .Options;

            _context = new TransportDbContext(options);

            // Добавляем тестовые данные в базу данных
            _context.Routes.Add(new Route { RouteId = 1, Name = "Route 1", TransportType = "Bus", PlannedTravelTime = 60, Distance = 20, IsExpress = true });
            _context.Routes.Add(new Route { RouteId = 2, Name = "Route 2", TransportType = "Trolleybus", PlannedTravelTime = 45, Distance = 15, IsExpress = false });
            _context.SaveChanges();

            _controller = new RoutesController(_context);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithRoute()
        {
            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Route>(viewResult.Model);
            Assert.Equal(1, model.RouteId); 
        }
    }
}
