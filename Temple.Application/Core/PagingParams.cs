namespace Temple.Application.Core;

public abstract class PagingParams
{
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    public string? HistoricalTime { get; set; } = null;
    public bool? IncludeHistoricalObjects { get; set; } = null;
    public string? DatabaseTime { get; set; } = null;

    private int _pageSize = 25;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
