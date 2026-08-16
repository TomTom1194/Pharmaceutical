namespace WebApp.Models;

public class HomePageContentModel
{
    public string? HeroTitle { get; set; }
    public string? HeroSubtitle { get; set; }
    public string? AboutTitle { get; set; }
    public string? AboutDescription { get; set; }
    public List<string> SliderImages { get; set; } = new();
    public string? QuoteEmail { get; set; }
    public string? QuotePhone { get; set; }
    public List<JobOpeningModel> JobOpenings { get; set; } = new();
    public List<CoreValueModel> CoreValues { get; set; } = new();
}
