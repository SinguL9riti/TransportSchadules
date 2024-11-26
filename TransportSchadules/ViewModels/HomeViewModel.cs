using TransportSchadules.Models;
using Route = TransportSchadules.Models.Route;

namespace TransportSchadules.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Personnel> Personnels { get; set; }

        public IEnumerable<RouteViewModel> Routes { get; set; }

        public IEnumerable<Schedule> Schedules { get; set; }

        public IEnumerable<Stop> Stops { get; set; }
    }
}
