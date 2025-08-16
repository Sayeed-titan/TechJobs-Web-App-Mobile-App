namespace TechJobs.Application.DTOs;

public record PageRequest(int Page = 1, int PageSize = 10)
{
    public int SafePage => Page < 1 ? 1 : Page;
    public int SafePageSize => PageSize < 1 ? 10 : (PageSize > 100 ? 100 : PageSize);
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
