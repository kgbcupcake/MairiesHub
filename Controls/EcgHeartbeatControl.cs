using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MairiesHub.Controls;

public class EcgHeartbeatControl : Control
{
    private readonly Queue<double> _samples = new();
    private const int MaxSamples = 60;

    public static readonly StyledProperty<int> SampleCapacityProperty =
        AvaloniaProperty.Register<EcgHeartbeatControl, int>(nameof(SampleCapacity), MaxSamples);

    public int SampleCapacity
    {
        get => GetValue(SampleCapacityProperty);
        set => SetValue(SampleCapacityProperty, value);
    }

    public void AddSample(double value)
    {
        while (_samples.Count >= SampleCapacity)
            _samples.Dequeue();
        _samples.Enqueue(value);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var samples = _samples.ToArray();
        if (samples.Length < 2)
            return;

        double width = Bounds.Width;
        double height = Bounds.Height;
        if (width <= 0 || height <= 0)
            return;

        double min = double.MaxValue;
        double max = double.MinValue;
        foreach (var s in samples)
        {
            if (s < min) min = s;
            if (s > max) max = s;
        }

        double range = max - min;
        if (range < 0.001) range = 1.0;

        var glowPen = new Pen(new SolidColorBrush(Color.Parse("#3300bcd4")), 5.0);
        var linePen = new Pen(new SolidColorBrush(Color.Parse("#00bcd4")), 1.8);
        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            double stepX = width / (samples.Length - 1);
            for (int i = 0; i < samples.Length; i++)
            {
                double x = i * stepX;
                double normalized = (samples[i] - min) / range;
                double y = height - (normalized * height * 0.85) - height * 0.075;

                if (i == 0)
                    ctx.BeginFigure(new Point(x, y), false);
                else
                    ctx.LineTo(new Point(x, y));
            }
        }

        context.DrawGeometry(null, glowPen, geometry);
        context.DrawGeometry(null, linePen, geometry);
    }
}
