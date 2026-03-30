using Avalonia;
using System;

namespace MairiesHub;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .ConfigureFonts(fm =>
            {
                fm.AddFontCollection(new Avalonia.Media.Fonts.EmbeddedFontCollection(
                    new Uri("fonts:MairiesHub"),
                    new Uri("avares://MairiesHub/Assets/Fonts")));
            })
            .LogToTrace();
}
