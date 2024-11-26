using TransportSchadules.ViewModels;

public class PageViewModel

{
    public int PageNumber { get; private set; }
    public int TotalPages { get; private set; }

    public PageViewModel(int count, int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public int PreviousPage => PageNumber > 1 ? PageNumber - 1 : 1;
    public int NextPage => PageNumber < TotalPages ? PageNumber + 1 : TotalPages;
}
