using System.ComponentModel.DataAnnotations;

namespace TransportSchadules.ViewModels
{
    public class ScheduleViewModel
    {

        public int RouteId { get; set; }

        public string Weekday { get; set; } = null!;

        public TimeSpan ArrivalTime { get; set; }

        public int Year { get; set; }

    }
}
