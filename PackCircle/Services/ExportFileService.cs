using Microsoft.Win32;
using PackCircle.Models;
using System.IO;
using System.Windows;

namespace PackCircle.Services;

public class ExportFileService(IExport csvExport, IExport excelExport, IExport txtExport)
{
    readonly Dictionary<string, string> _filters = new()
    {
        { "CSV", "CSV files (*.csv)|*.csv" },
        { "Excel", "Excel Workbook (*.xlsx)|*.xlsx" },
        { "Text", "Text files (*.txt)|*.txt" }
    };

    public async Task CreateFile(ICollection<Circle> circles, string fileType)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = Filter(fileType),
            Title = "Export Center Positions",
            FileName = "CenterPositions"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            string filePath = saveFileDialog.FileName;
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();
                switch (extension)
                {
                    case ".csv":
                        await csvExport.Export(filePath, circles);
                        break;
                    case ".txt":
                        await txtExport.Export(filePath, circles);
                        break;
                    case ".xlsx":
                        await excelExport.Export(filePath, circles);
                        break;
                }

                MessageBox.Show("File created.", "Export ok", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (IOException ex) when (IsFileLocked(ex))
            {
                MessageBox.Show(
                    $"'{Path.GetFileName(filePath)}' is currently open." +
                    $"\n\nIf you want to overwrite it close the file and try again." +
                    $"\n\nOtherwise select another file name.",
                    "File Is Open", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch
            {
                MessageBox.Show("File not created.", "Export Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }

    string Filter(string fileType) => _filters.GetValueOrDefault(fileType, string.Join('|', _filters.Values));

    static bool IsFileLocked(IOException exception)
    {
        // 0x80070020 is the standard HRESULT for ERROR_SHARING_VIOLATION
        // 0x80070021 is the standard HRESULT for ERROR_LOCK_VIOLATION
        int hr = exception.HResult & 0xFFFF;
        return hr is 32 or 33;
    }
}