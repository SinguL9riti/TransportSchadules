using TransportSchadules.Models;
using Route = TransportSchadules.Models.Route;

namespace TransportSchadules.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TransportDbContext db)
        {
            db.Database.EnsureCreated();

            if (db.Stops.Any()) return;

            int numberOfStops = 20;
            int numberOfRoutes = 20;
            int numberOfSchedules = 20;
            int numberOfPersonnel = 100;
            Random rand = new Random();

            for (int i = 1; i <= numberOfStops; i++)
            {
                db.Stops.Add(new Stop
                {
                    Name = "Остановка_" + i,
                    IsTerminal = rand.Next(2) == 1,
                    HasDispatcher = rand.Next(2) == 1
                });
            }
            db.SaveChanges();

            for (int i = 1; i <= numberOfRoutes; i++)
            {
                db.Routes.Add(new Route
                {
                    Name = "Маршрут_" + i,
                    TransportType = rand.Next(2) == 1 ? "Автобус" : "Троллейбус",
                    PlannedTravelTime = 30 + rand.Next(120),
                    Distance = (decimal)Math.Round(5 + rand.NextDouble() * 45, 2),
                    IsExpress = rand.Next(2) == 1
                });
            }
            db.SaveChanges();

            string[] daysOfWeek = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };

            for (int i = 1; i <= numberOfSchedules; i++)
            {
                db.Schedules.Add(new Schedule
                {
                    RouteId = rand.Next(1, numberOfRoutes + 1),
                    Weekday = daysOfWeek[rand.Next(daysOfWeek.Length)],
                    ArrivalTime = TimeSpan.FromMinutes(rand.Next(1440)),
                    Year = DateTime.Now.Year - rand.Next(2)
                });
            }
            db.SaveChanges();

            string[] lastNames = { "Иванов", "Петров", "Сидоров", "Кузнецов", "Попов", "Мельник", "Точило" };

            for (int i = 1; i <= numberOfPersonnel; i++)
            {
                db.Personnels.Add(new Personnel
                {
                    RouteId = rand.Next(1, numberOfRoutes + 1),
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-rand.Next(365))),
                    Shift = rand.Next(2) == 1 ? "День" : "Ночь",
                    EmployeeList = $"{lastNames[rand.Next(lastNames.Length)]}_{i}, {lastNames[rand.Next(lastNames.Length)]}_{i + 1}"
                });
            }
            db.SaveChanges();
        }
    }
}


