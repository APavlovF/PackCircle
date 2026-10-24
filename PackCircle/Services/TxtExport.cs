using System.IO;
using System.Text;
using PackCircle.Models;

namespace PackCircle.Services;

public class TxtExport : IExport
{
    public async Task Export(string filePath, ICollection<Circle> circles)
    {
        StringBuilder content = new StringBuilder();
        content.AppendLine("X\tY");

        foreach (Circle circle in circles)
        {
            content.AppendLine($"{circle.X}\t{circle.Y}");
        }

        await File.WriteAllTextAsync(filePath, content.ToString(), Encoding.UTF8);
    }
}