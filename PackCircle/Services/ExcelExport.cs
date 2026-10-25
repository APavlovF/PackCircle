using MiniExcelLibs;
using PackCircle.Models;

namespace PackCircle.Services
{
    public class ExcelExport : IExport
    {
        public async Task Export(string filePath, ICollection<Circle> circles) => await MiniExcel.SaveAsAsync(filePath,
            circles.Select(c => new { X = c.GetCenterX(), Y = c.GetCenterY() }), overwriteFile: true);
    }
}
