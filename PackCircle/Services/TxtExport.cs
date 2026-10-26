using System.IO;
using System.Text;
using PackCircle.Models;

namespace PackCircle.Services;

public class TxtExport : IExport
{
    public FileTypeExtension Type => FileTypeExtension.txt;

    public async Task Export(string filePath, ICollection<Circle> circles)
    {
        StringBuilder content = new StringBuilder();
        content.AppendLine("X\tY");

        foreach (Circle circle in circles)
        {
            content.AppendLine($"{circle.GetCenterX()}\t{circle.GetCenterY()}");
        }

        await File.WriteAllTextAsync(filePath, content.ToString(), Encoding.UTF8);
    }
}