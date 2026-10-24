using CommunityToolkit.Mvvm.ComponentModel;

namespace PackCircle.Models;

public partial class Circle(double diameter, double x, double y) : ObservableObject
{
    [ObservableProperty] bool _isFilled;
    public double Diameter { get; set; } = diameter;
    public double X { get; set; } = Math.Round(x, 4);
    public double Y { get; set; } = Math.Round(y, 4);
}