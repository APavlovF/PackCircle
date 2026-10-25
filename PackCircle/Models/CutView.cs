using CommunityToolkit.Mvvm.ComponentModel;

namespace PackCircle.Models;

public partial class CutView : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LastSheetPercent))]
    int _circlesOnSheet;
    [ObservableProperty] bool _isBestChoice;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LastSheetPercent))]
    int _lastSheetCircles;
    public decimal LastSheetPercent => CirclesOnSheet != 0 ? (decimal)LastSheetCircles / CirclesOnSheet : 0;
    [ObservableProperty] int _sheets;
}