using TransportSchadules.ViewModels;


namespace TransportSchadules.Services
{
    public interface IViewModelService
    {
        HomeViewModel GetHomeViewModel(int numberRows = 10);
    }
}
