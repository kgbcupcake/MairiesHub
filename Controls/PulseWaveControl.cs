using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using MairiesHub.Models;

namespace MairiesHub.Controls;

public class PulseWaveControl : Control
{
    private static readonly Random Rng = new();

    // ── Styled property ──────────────────────────────────────────────────────
    public static readonly StyledProperty<ServiceHealth> ColorModeProperty =
        AvaloniaProperty.Register<PulseWaveControl, ServiceHealth>(
            nameof(ColorMode), ServiceHealth.Stopped);

    public ServiceHealth ColorMode
    {
        get => GetValue(ColorModeProperty);
        set => SetValue(ColorModeProperty, value);
    }

    // ── State ────────────────────────────────────────────────────────────────
    private readonly Queue<double> _samples = new();
    private const int MaxSamples = 60;

    private DispatcherTimer? _timer;

    private static readonly double[] EcgPattern =
    [
        0.0, 0.0, 0.02, 0.0, 0.0, 0.0,   // flat baseline
        0.0, 0.15, 0.8, 1.0, 0.6, -0.2,   // sharp upstroke → overshoot
        -0.35, 0.1, 0.25, 0.15, 0.05, 0.0, // S-wave → T-wave rebound
        0.0, 0.0, 0.0, 0.0, 0.0, 0.0      // return to baseline
    ];

    // ── Lifecycle ────────────────────────────────────────────────────────────
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1200) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer?.Stop();
        _timer = null;
        base.OnDetachedFromVisualTree(e);
    }

    // ── Sample generation ────────────────────────────────────────────────────
    private void OnTick(object? sender, EventArgs e)
    {
        var health = ColorMode;

        switch (health)
        {
            case ServiceHealth.Healthy:
                // Feed all pattern steps at once then coast on baseline
                foreach (var step in EcgPattern)
                {
                    double noise = (Rng.NextDouble() - 0.5) * 0.03;
                    Enqueue(step + noise);
                }
                // fill remaining with flat baseline + tiny noise
                int remaining = MaxSamples - EcgPattern.Length;
                for (int i = 0; i < remaining; i++)
                    Enqueue((Rng.NextDouble() - 0.5) * 0.025);
                break;

            case ServiceHealth.Degraded:
                // Irregular low-amplitude erratic blips
                for (int i = 0; i < MaxSamples; i++)
                {
                    double v = (Rng.NextDouble() - 0.5) * 0.4;
                    if (Rng.NextDouble() < 0.08)
                        v += (Rng.NextDouble() - 0.5) * 0.7; // occasional spike
                    Enqueue(v);
                }
                break;

            case ServiceHealth.Stopped:
            default:
                // Flat line with tiny noise
                for (int i = 0; i < MaxSamples; i++)
                    Enqueue((Rng.NextDouble() - 0.5) * 0.02);
                break;
        }

        InvalidateVisual();
    }

    private void Enqueue(double v)
    {
        if (_samples.Count >= MaxSamples)
            _samples.Dequeue();
        _samples.Enqueue(v);
    }

    // ── Rendering ────────────────────────────────────────────────────────────
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var samples = _samples.ToArray();
        if (samples.Length < 2) return;

        double w = Bounds.Width;
        double h = Bounds.Height;
        if (w <= 0 || h <= 0) return;

        const double hPad = 8.0;
        double drawW = w - hPad * 2;

        var color = ColorMode switch
        {
            ServiceHealth.Healthy  => Color.Parse("#22c55e"),
            ServiceHealth.Degraded => Color.Parse("#f59e0b"),
            _                      => Color.Parse("#ef4444")
        };

        var glowColor = Color.FromArgb(64, color.R, color.G, color.B);  // ~25% alpha
        var glowPen = new Pen(new SolidColorBrush(glowColor), 6.0);
        var linePen = new Pen(new SolidColorBrush(color), 1.5);

        double min = double.MaxValue, max = double.MinValue;
        foreach (var s in samples)
        {
            if (s < min) min = s;
            if (s > max) max = s;
        }
        double range = max - min;
        if (range < 0.001) range = 1.0;

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            double stepX = drawW / (samples.Length - 1);
            for (int i = 0; i < samples.Length; i++)
            {
                double x = hPad + i * stepX;
                double normalized = (samples[i] - min) / range;
                double y = h - (normalized * h * 0.75) - h * 0.125;

                if (i == 0) ctx.BeginFigure(new Point(x, y), false);
                else        ctx.LineTo(new Point(x, y));
            }
        }

        context.DrawGeometry(null, glowPen, geometry);
        context.DrawGeometry(null, linePen, geometry);
    }
}
