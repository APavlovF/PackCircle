using CommunityToolkit.Mvvm.ComponentModel;

namespace PackCircle.Models;

public partial class Circle(double diameter, double x, double y) : ObservableObject
{
    [ObservableProperty] bool _isFilled;
    public double Diameter { get; set; } = diameter;
    public double X { get; set; } = x;
    public double Y { get; set; } = y;

    public double GetCenterX() => Math.Round(X + Diameter / 2, 4);

    public double GetCenterY() => Math.Round(Y + Diameter / 2, 4);
}