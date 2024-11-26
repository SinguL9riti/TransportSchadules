using TransportSchadules.Models;

namespace TransportSchadules.ViewModels
{
    public class SchedulesIndexViewModel
    {
        public IEnumerable<Schedule> Schedules { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
