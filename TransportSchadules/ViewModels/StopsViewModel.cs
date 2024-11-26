using TransportSchadules.Models;

namespace TransportSchadules.ViewModels
{
    public class StopsViewModel
    {
        public IEnumerable<Stop> Stops { get; set; } = Enumerable.Empty<Stop>();
        public PageViewModel PageViewModel { get; set; }
    }
}
