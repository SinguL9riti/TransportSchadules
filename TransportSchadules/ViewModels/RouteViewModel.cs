using TransportSchadules.Models;
using System.ComponentModel.DataAnnotations;

namespace TransportSchadules.ViewModels
{
    public class RouteViewModel
    {
        [Display(Name = "Код маршрута")]
        public int RouteId { get; set; }

        [Display(Name = "Маршрут")]
        public string Name { get; set; } = null!;

        [Display(Name = "Тип транспорта")]
        public string TransportType { get; set; } = null!;

        [Display(Name = "Продолжительность")]
        public int PlannedTravelTime { get; set; }

        [Display(Name = "Дистанция")]
        public decimal Distance { get; set; }

        [Display(Name = "Экспресс")]
        public bool IsExpress { get; set; }

    }
}
