using PackCircle.Models;

namespace PackCircle.Services;

public class ExportFactory(IEnumerable<IExport> exporters)
{
    readonly Dictionary<FileTypeExtension, IExport> _exporters = exporters.ToDictionary(e => e.Type);

    public IExport Get(FileTypeExtension type) => _exporters[type];
}