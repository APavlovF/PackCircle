using PackCircle.Models;
using System.Collections.ObjectModel;

namespace PackCircle.Services;

public class TriangularPacker : CirclePacker
{
    protected override ObservableCollection<Circle> PackInternal(PackContext data)
    {
        ObservableCollection<Circle> result = [];
        double rowStep = data.Pitch * Math.Sqrt(3) / 2;
        int rows = (int)Math.Floor((data.EffectiveH - data.D) / rowStep + Eps) + 1;

        for (int r = 0; r < rows; r++)
        {
            double rowOffset = r % 2 == 1 ? data.Pitch / 2 : 0;
            double available = data.EffectiveW - data.D - rowOffset;
            if (available < -Eps) continue;

            int cols = (int)Math.Floor(available / data.Pitch + Eps) + 1;
            for (int c = 0; c < cols; c++)
            {
                double x = data.Margin + rowOffset + c * data.Pitch;
                double y = data.Margin + r * rowStep;
                result.Add(new Circle(data.D, x, y));
            }
        }

        return result;
    }
}