namespace MairiesHub;

public static class AppVersion
{
    public static string Current { get; } =
        System.Reflection.Assembly.GetExecutingAssembly()
            .GetName().Version?.ToString(3) ?? "0.0.0";
}
