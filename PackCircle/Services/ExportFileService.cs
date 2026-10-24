using System.IO;
using System.Text;
using PackCircle.Models;

namespace PackCircle.Services;

public class ExportFileService
{
    public async Task ExportToCsv(string filePath, ICollection<Circle> circles)
    {
        StringBuilder csvContent = new StringBuilder();
        csvContent.AppendLine("X,Y");
        foreach (Circle circle in circles)
        {
            csvContent.AppendLine($"{circle.X},{circle.Y}");
        }

        await File.WriteAllTextAsync(filePath, csvContent.ToString(), Encoding.UTF8);
    }

    public async Task ExportToTxt(string filePath, ICollection<Circle> circles)
    {
        StringBuilder content = new StringBuilder();
        content.AppendLine("X\tY"); // Tab-separated header

        foreach (Circle circle in circles)
        {
            content.AppendLine($"{circle.X}\t{circle.Y}");
        }

        await File.WriteAllTextAsync(filePath, content.ToString(), Encoding.UTF8);
    }
}