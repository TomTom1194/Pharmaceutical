namespace WebApp.Helpers;

public static class SpecificationTypeResolver
{
    public const string Tablet = "Tablet";
    public const string Capsule = "Capsule";
    public const string LiquidFilling = "LiquidFilling";

    public static string? Resolve(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return null;

        if (categoryName.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            return Tablet;

        if (categoryName.Contains("Capsule", StringComparison.OrdinalIgnoreCase))
            return Capsule;

        if (categoryName.Contains("Liquid", StringComparison.OrdinalIgnoreCase))
            return LiquidFilling;

        return null;
    }
}
