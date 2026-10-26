using PackCircle.Models;

namespace PackCircle.Services;

public interface IExport
{
    FileTypeExtension Type { get; }
    Task Export(string filePath, ICollection<Circle> circles);
}