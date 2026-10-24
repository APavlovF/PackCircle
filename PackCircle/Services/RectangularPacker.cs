using PackCircle.Models;
using System.Collections.ObjectModel;

namespace PackCircle.Services;

public class RectangularPacker : CirclePacker
{
    protected override ObservableCollection<Circle> PackInternal(PackContext data)
    {
        ObservableCollection<Circle> result = [];
        int cols = (int)Math.Floor((data.EffectiveW - data.D) / data.Pitch + Eps) + 1;
        int rows = (int)Math.Floor((data.EffectiveH - data.D) / data.Pitch + Eps) + 1;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result.Add(
                    new Circle(data.D, data.Margin + c * data.Pitch, data.Margin + r * data.Pitch));
            }
        }
        return result;
    }
}