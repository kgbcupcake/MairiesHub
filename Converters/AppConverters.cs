using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using MairiesHub.Models;

namespace MairiesHub.Converters;

/// <summary>
/// Returns true when the bound string equals the ConverterParameter string.
/// Used for nav-button active indicators.
/// </summary>
public class StringEqualsConverter : IValueConverter
{
    public static readonly StringEqualsConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.ToString() == parameter?.ToString();

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Cyberpunk palette converter for Docker card view.
/// running → cyan (#22d3ee), exited/stopped → dark-red (#7f1d1d),
/// healthy → cyan, unhealthy/degraded → amber-warm (#fb923c), else slate (#475569).
/// </summary>
public class CyberpunkStateConverter : IValueConverter
{
    public static readonly CyberpunkStateConverter Instance = new();

    private static readonly IBrush Cyan      = new SolidColorBrush(Color.Parse("#22d3ee"));
    private static readonly IBrush DarkRed   = new SolidColorBrush(Color.Parse("#7f1d1d"));
    private static readonly IBrush AmberWarm = new SolidColorBrush(Color.Parse("#fb923c"));
    private static readonly IBrush Slate     = new SolidColorBrush(Color.Parse("#475569"));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? Cyan : DarkRed;

        return value?.ToString()?.ToLowerInvariant() switch
        {
            "running" or "active" or "online" or "healthy" => Cyan,
            "exited" or "stopped" or "failed" or "dead" or "offline" => DarkRed,
            "unhealthy" or "degraded" => AmberWarm,
            _ => Slate
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Maps a container/service state string to a status-indicator brush.
/// running → green, exited/stopped/failed → red, paused → orange, else grey.
/// </summary>
public class StateToColorConverter : IValueConverter
{
    public static readonly StateToColorConverter Instance = new();

    private static readonly IBrush Green  = new SolidColorBrush(Color.Parse("#4caf50"));
    private static readonly IBrush Red    = new SolidColorBrush(Color.Parse("#f44336"));
    private static readonly IBrush Orange = new SolidColorBrush(Color.Parse("#ff9800"));
    private static readonly IBrush Grey   = new SolidColorBrush(Color.Parse("#616161"));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Also handles bool — true = running/green, false = stopped/red
        if (value is bool b)
            return b ? Green : Red;

        return value?.ToString()?.ToLowerInvariant() switch
        {
            "running" or "active" or "online" or "true" => Green,
            "exited" or "stopped" or "failed" or "dead" or "offline" or "false" => Red,
            "paused" or "restarting" or "unknown" => Orange,
            _ => Grey
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Maps bool Running → ServiceHealth (true=Healthy, false=Stopped).
/// </summary>
public class BoolToHealthConverter : IValueConverter
{
    public static readonly BoolToHealthConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? ServiceHealth.Healthy : ServiceHealth.Stopped;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
