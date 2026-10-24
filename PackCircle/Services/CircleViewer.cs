using PackCircle.Models;
using System.Collections.ObjectModel;

namespace PackCircle.Services;

public class CircleViewer
{
    public void UpdatePreviews(SheetViewMode viewMode,
        ObservableCollection<Circle> rectangularCircles,
        ObservableCollection<Circle> triangularCircles,
        CutView rectangularCut,
        CutView triangularCut,
        int quantity)
    {
        if (viewMode == SheetViewMode.LastSheet)
        {
            UpdateCircles(rectangularCircles, rectangularCut.LastSheetCircles);
            UpdateCircles(triangularCircles, triangularCut.LastSheetCircles);
        }
        else
        {
            UpdateCircles(rectangularCircles, Math.Min(quantity, rectangularCircles.Count));
            UpdateCircles(triangularCircles, Math.Min(quantity, triangularCircles.Count));
        }
    }

    static void UpdateCircles(ObservableCollection<Circle> circles, int filledCount)
    {
        for (int i = 0; i < circles.Count; i++)
        {
            circles[i].IsFilled = i < filledCount;
        }
    }
}