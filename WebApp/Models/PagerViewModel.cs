namespace WebApp.Models;

public class PagerViewModel
{
    public string Action { get; set; } = "Index";
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public string ItemLabel { get; set; } = "items";
    public Dictionary<string, string?> RouteValues { get; set; } = new();
}
