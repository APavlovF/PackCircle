using System.IO;
using PackCircle.Models;
using System.Text;

namespace PackCircle.Services;

public class CsvExport : IExport
{
    public async Task Export(string filePath, ICollection<Circle> circles)
    {
        StringBuilder csvContent = new StringBuilder();
        csvContent.AppendLine("X,Y");
        foreach (Circle circle in circles)
        {
            csvContent.AppendLine($"{circle.GetCenterX()},{circle.GetCenterY()}");
        }

        await File.WriteAllTextAsync(filePath, csvContent.ToString(), Encoding.UTF8);
    }
}