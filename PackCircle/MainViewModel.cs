using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PackCircle.Models;
using PackCircle.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;

namespace PackCircle;

public partial class MainViewModel : ObservableObject
{
    const double VerticalCutoffFactor = 0.8;
    readonly CircleViewer _circleViewer;
    readonly Dictionary<string, decimal> _comboUnits = new()
    {
        { "mm", 1m },
        { "cm", 10m },
        { "m", 1000m },
        { "in", 25.4m },
        { "ft", 304.8m }
    };
    readonly ExportFileService _exportFileService;
    readonly RectangularPacker _rectangularPacker;
    readonly TriangularPacker _triangularPacker;

    [ObservableProperty] SheetViewMode _currentViewMode = SheetViewMode.LastSheet;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(StackSheets))] double _height = 100;
    [ObservableProperty] double _margin = 2;
    [ObservableProperty] int _quantity = 10;
    [ObservableProperty] ObservableCollection<Circle> _rectangularCircles = [];
    [ObservableProperty] CutView _rectangularCut = new();
    [ObservableProperty] ObservableCollection<Circle> _triangularCircles = [];
    [ObservableProperty] string _selectedUnit = "mm";
    [ObservableProperty] double _size = 10;
    [ObservableProperty] double _space = 2;
    [ObservableProperty] CutView _triangularCut = new();
    [ObservableProperty][NotifyPropertyChangedFor(nameof(StackSheets))] double _width = 100;

    public IEnumerable<string> AvailableUnits => _comboUnits.Keys;
    public bool StackSheets => Width >= Height * VerticalCutoffFactor;

    public MainViewModel(CircleViewer circleViewer,
        ExportFileService exportFileService,
        RectangularPacker rectangularPacker,
        TriangularPacker triangularPacker)
    {
        _circleViewer = circleViewer;
        _exportFileService = exportFileService;
        _rectangularPacker = rectangularPacker;
        _triangularPacker = triangularPacker;
        Calculate();
    }

    [RelayCommand]
    public void Calculate()
    {
        if (!CanCalculate())
        {
            MessageBox.Show("Please enter valid input values.", "Input Error", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        RectangularCircles = _rectangularPacker.Pack(Width, Height, Size, Space, Margin);
        TriangularCircles = _triangularPacker.Pack(Width, Height, Size, Space, Margin);
        _rectangularCut.CirclesOnSheet = RectangularCircles.Count;
        if (RectangularCircles.Count > 0)
        {
            _rectangularCut.Sheets = (int)Math.Ceiling((double)_quantity / RectangularCircles.Count);
        }

        _rectangularCut.LastSheetCircles = _quantity - (_rectangularCut.Sheets - 1) * RectangularCircles.Count;
        _triangularCut.CirclesOnSheet = TriangularCircles.Count;
        if (TriangularCircles.Count > 0)
        {
            _triangularCut.Sheets = (int)Math.Ceiling((double)_quantity / TriangularCircles.Count);
        }

        _triangularCut.LastSheetCircles = _quantity - (_triangularCut.Sheets - 1) * TriangularCircles.Count;
        _rectangularCut.IsBestChoice = _rectangularCut.CirclesOnSheet > _triangularCut.CirclesOnSheet;
        _triangularCut.IsBestChoice = !_rectangularCut.IsBestChoice;
        _circleViewer.UpdatePreviews(CurrentViewMode, RectangularCircles, TriangularCircles, _rectangularCut,
            _triangularCut, _quantity);
    }

    [RelayCommand]
    public async Task ExportCenterPositions(string cutType)
    {
        ICollection<Circle> targetCircles = cutType switch
        {
            "Triangular" => TriangularCircles,
            "Rectangular" => RectangularCircles,
            _ => throw new ArgumentOutOfRangeException(nameof(cutType))
        };
        if (targetCircles.Count == 0) return;

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv|Excel Workbook (*.xlsx)|*.xlsx|Text files (*.txt)|*.txt",
            Title = "Export Center Positions",
            FileName = "CenterPositions"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                string filePath = saveFileDialog.FileName;
                string extension = Path.GetExtension(filePath).ToLower();
                switch (extension)
                {
                    case ".csv":
                        await _exportFileService.ExportToCsv(filePath, targetCircles);
                        break;
                    case ".txt":
                        await _exportFileService.ExportToTxt(filePath, targetCircles);
                        break;
                    case ".xlsx":
                        // Handle Excel export if needed
                        break;
                }

                MessageBox.Show("File created.", "Export ok", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("File not created.", "Export Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }

    bool CanCalculate() => Width > 0 && Height > 0 && Quantity > 0 && Size > 0 && Space >= 0 && Margin >= 0;

    [RelayCommand]
    void ChangeViewMode(SheetViewMode selectedMode) => CurrentViewMode = selectedMode;

    [RelayCommand]
    void CloseApplication() => Application.Current.Shutdown();

    partial void OnCurrentViewModeChanged(SheetViewMode value) => _circleViewer.UpdatePreviews(value, RectangularCircles,
        TriangularCircles, _rectangularCut, _triangularCut, _quantity);

    partial void OnHeightChanged(double value) => Calculate();

    partial void OnMarginChanged(double value) => Calculate();

    partial void OnQuantityChanged(int value) => Calculate();

    partial void OnSizeChanged(double value) => Calculate();

    partial void OnSpaceChanged(double value) => Calculate();

    partial void OnWidthChanged(double value) => Calculate();
}