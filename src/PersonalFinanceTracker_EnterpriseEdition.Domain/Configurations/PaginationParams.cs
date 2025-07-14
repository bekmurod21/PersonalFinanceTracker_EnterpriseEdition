namespace PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;

public class PaginationParams
{
    private const int maxPageSize = 1000;
    private int pageSize = 100;

    public int PageSize
    {
        get => pageSize;
        set => pageSize = value > maxPageSize ? maxPageSize : value;
    }
    public int PageIndex { get; set; } = 1;
}