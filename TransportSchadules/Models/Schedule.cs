using System.ComponentModel.DataAnnotations;

namespace TransportSchadules.Models
{
    public partial class Schedule
    {
        [Display(Name = "Код расписания")]
        public int ScheduleId { get; set; }

        [Display(Name = "Код маршрута")]
        [Required]
        public int RouteId { get; set; }

        [Display(Name = "День Недели")]
        public string Weekday { get; set; } = null!;

        [Display(Name = "Время прибытия")]
        public TimeSpan ArrivalTime { get; set; }

        [Display(Name = "Год")]
        public int Year { get; set; }

        public virtual Route Route { get; set; } = null!;
    }
}
