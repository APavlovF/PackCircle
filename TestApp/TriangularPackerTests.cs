using PackCircle.Models;
using PackCircle.Services;

namespace TestApp;

[TestClass]
public class TriangularPackerTests
{
    const double _delta = CirclePacker.Eps;

    [TestMethod]
    public void Pack_TriangularPacker_VerifiesFirstAndLastCircleCenters()
    {
        // Arrange
        // Sheet: 110x110, Margin: 5 -> Effective size: 100x100
        // Circle D: 20, Gap: 10 -> Pitch = 30, Radius = 10
        double w = 110;
        double h = 110;
        double d = 20;
        double gap = 10;
        double margin = 5;

        double pitch = d + gap;                     // 30
        double radius = d / 2.0;                    // 10
        double rowStep = pitch * Math.Sqrt(3) / 2;  // ~25.9807

        // Act
        TriangularPacker packer = new TriangularPacker();
        List<Circle> result = packer.Pack(w, h, d, gap, margin).ToList();

        // Assert
        Assert.IsNotNull(result);

        // 1. Math Verification for First Circle (Row 0, Col 0)
        // Row 0 is even -> rowOffset = 0
        // X = Margin (5) + Offset (0) + 0*Pitch + Radius (10) = 15
        // Y = Margin (5) + 0*rowStep + Radius (10) = 15
        Circle firstCircle = result.First();
        Assert.AreEqual(15, firstCircle.GetCenterX(), _delta, "First circle X center is incorrect.");
        Assert.AreEqual(15, firstCircle.GetCenterY(), _delta, "First circle Y center is incorrect.");

        // 2. Math Verification for Last Circle
        // For 100x100 effective space:
        // Rows: floor((100 - 20) / 25.9807) + 1 = floor(3.079) + 1 = 4 rows (Indices: 0, 1, 2, 3)
        // The last row index is r = 3 (which is odd -> 3 % 2 == 1)
        // rowOffset for Row 3 = Pitch / 2 = 15
        // Available width for circles on Row 3 = 100 - 20 - 15 = 65
        // Columns on Row 3 = floor(65 / 30) + 1 = floor(2.16) + 1 = 3 cols (Indices: 0, 1, 2)
        //
        // Final Circle Coordinates (Row 3, Col 2):
        // X = Margin (5) + rowOffset (15) + 2 * Pitch (60) + Radius (10) = 90
        // Y = Margin (5) + 3 * rowStep (3 * 25.9807) + Radius (10) = 5 + 77.9422 + 10 = 92.9422
        Circle lastCircle = result.Last();

        double expectedLastX = 90.0;
        double expectedLastY = Math.Round(margin + 3 * rowStep + radius, 4); // 92.9422...

        Assert.AreEqual(expectedLastX, lastCircle.GetCenterX(), _delta, "Last circle X center is incorrect.");
        Assert.AreEqual(expectedLastY, lastCircle.GetCenterY(), _delta, "Last circle Y center is incorrect.");
    }
}