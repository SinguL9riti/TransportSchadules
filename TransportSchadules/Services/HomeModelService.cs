using TransportSchadules.ViewModels;
using TransportSchadules.Data;
using TransportSchadules.Models;
using TransportSchadules.Services;

namespace TransportSchadules.Services
{
    // Класс выборки 10 записей из всех таблиц 
    public class HomeModelService(TransportDbContext context) : IViewModelService
    {
        private readonly TransportDbContext _context = context;

        public HomeViewModel GetHomeViewModel(int numberRows = 10)
        {
            var personnels = _context.Personnels.Take(numberRows).ToList();
            var schedules = _context.Schedules.Take(numberRows).ToList();
            var stops = _context.Stops.Take(numberRows).ToList();
            List<RouteViewModel> routes = [.. _context.Routes
                .OrderBy(d => d.RouteId)
                .Select(t => new RouteViewModel
                {
                    RouteId = t.RouteId,
                    Name = t.Name,
                    TransportType = t.TransportType,
                    PlannedTravelTime = t.PlannedTravelTime,
                    Distance = t.Distance,
                    IsExpress = t.IsExpress,
                })
                .Take(numberRows)];

            HomeViewModel homeViewModel = new()
            {
                Personnels = personnels,
                Schedules = schedules,
                Stops = stops,
                Routes = routes
            };
            return homeViewModel;
        }
    }
}
