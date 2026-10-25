using System.Collections.ObjectModel;
using PackCircle.Models;
using PackCircle.Services;

namespace TestApp;

[TestClass]
public class RectangularPackerTests
{
    const double _delta = CirclePacker.Eps;

    [TestMethod]
    public void Pack_ValidInputs_ReturnsCorrectGridDimensions()
    {
        // Arrange
        // Sheet: 110x110. Margin: 5 -> Effective area: 100x100
        // Circle D: 20, Gap: 5 -> Pitch = 25
        double w = 110;
        double h = 110;
        double d = 20;
        double gap = 5;
        double margin = 5;

        // Act
        RectangularPacker packer = new RectangularPacker();
        ObservableCollection<Circle> result = packer.Pack(w, h, d, gap, margin);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(16, result.Count, "Should fit a 4x4 grid totaling 16 circles.");
    }

    [TestMethod]
    public void Pack_ValidInputs_VerifiesFirstAndLastCircleCenters()
    {
        // Arrange
        double w = 110;
        double h = 110;
        double d = 20; // Radius = 10
        double gap = 5;
        double margin = 5;

        // Act
        RectangularPacker packer = new RectangularPacker();
        List<Circle> result = packer.Pack(w, h, d, gap, margin).ToList();

        // Assert
        // Expected X/Y: Margin (5) + Radius (10) = 15
        Circle firstCircle = result.First();
        Assert.AreEqual(15, firstCircle.GetCenterX(), _delta, "First circle X center is incorrect.");
        Assert.AreEqual(15, firstCircle.GetCenterY(), _delta, "First circle Y center is incorrect.");

        // Last circle (top-right / bottom-right)
        // Row 4, Col 4: Margin (5) + 3 * Pitch (25) + Radius (10) = 90
        Circle lastCircle = result.Last();
        Assert.AreEqual(90, lastCircle.GetCenterX(), _delta, "Last circle X center is incorrect.");
        Assert.AreEqual(90, lastCircle.GetCenterY(), _delta, "Last circle Y center is incorrect.");
    }

    [TestMethod]
    [DataRow(100, 100, 20, 0, -5)] // Negative margin
    [DataRow(100, 100, -20, 0, 5)] // Negative diameter
    [DataRow(50, 50, 60, 0, 5)]    // Diameter too large for sheet width
    public void Pack_InvalidOrImpossibleInputs_ReturnsEmptyCollection(double w, double h, double d, double gap, double margin)
    {
        // Arrange
        RectangularPacker packer = new RectangularPacker();

        // Act
        ObservableCollection<Circle> result = packer.Pack(w, h, d, gap, margin);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count, "Should gracefully return an empty collection for invalid bounds.");
    }
}