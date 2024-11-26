using TransportSchadules.Models;
using Route = TransportSchadules.Models.Route;

namespace TransportSchadules.ViewModels
{
    public class RoutesIndexViewModel
    {
        public IEnumerable<Route> Routes { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
