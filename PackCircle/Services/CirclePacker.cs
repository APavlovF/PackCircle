using PackCircle.Models;
using System.Collections.ObjectModel;

namespace PackCircle.Services;

public abstract class CirclePacker
{
    public const double Eps = 1e-9;

    public record struct PackContext(
        double EffectiveW,
        double EffectiveH,
        double Pitch,
        double Margin,
        double D
    );

    public ObservableCollection<Circle> Pack(double w, double h, double d, double gap, double margin)
    {
        ObservableCollection<Circle> result = [];

        double effectiveW = w - 2 * margin;
        double effectiveH = h - 2 * margin;

        if (d <= 0 || effectiveW < d || effectiveH < d || margin < 0) return result;

        double pitch = d + gap;
        PackContext context = new PackContext(effectiveW, effectiveH, pitch, margin, d);

        return PackInternal(context);
    }

    protected abstract ObservableCollection<Circle> PackInternal(PackContext data);
}