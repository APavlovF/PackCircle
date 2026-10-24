using PackCircle.Models;

namespace PackCircle.Services;

public interface IExport
{
    Task Export(string filePath, ICollection<Circle> circles);
}