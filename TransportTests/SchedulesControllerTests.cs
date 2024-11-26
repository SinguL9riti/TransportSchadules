using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TransportSchadules.Controllers;
using TransportSchadules.Data;
using TransportSchadules.Models;
using TransportSchadules.ViewModels;
using Xunit;
using System.Collections.Generic;

namespace TransportTests
{
    public class SchedulesControllerTests
    {
        private readonly TransportDbContext _context;
        private readonly SchedulesController _controller;

        public SchedulesControllerTests()
        {
            // Создаем базу данных в памяти
            var options = new DbContextOptionsBuilder<TransportDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TransportDbContext(options);

            // Наполняем базу данных тестовыми данными
            _context.Schedules.Add(new Schedule { ScheduleId = 1, RouteId = 1, Weekday = "Monday", ArrivalTime = TimeSpan.Parse("10:00"), Year = 2024 });
            _context.Schedules.Add(new Schedule { ScheduleId = 2, RouteId = 2, Weekday = "Tuesday", ArrivalTime = TimeSpan.Parse("10:00"), Year = 2024 });
            _context.SaveChanges();

            _controller = new SchedulesController(_context);
        }
        [Fact]
        public async Task Create_Post_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var scheduleViewModel = new ScheduleViewModel
            {
                RouteId = 1,
                Weekday = "Monday",
                ArrivalTime = TimeSpan.Parse("10:00"),
                Year = 2024
            };

            // Act
            var result = await _controller.Create(scheduleViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SchedulesController.Index), redirectResult.ActionName);

            // Проверяем, что новая запись была добавлена в базу данных
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Weekday == "Monday");
            Assert.NotNull(schedule);
            Assert.Equal("Monday", schedule.Weekday);
        }
    }
}
