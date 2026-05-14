using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt10.Models;

namespace Projekt10.ViewModels;

public class BarItem
{
    public string Name { get; set; } = string.Empty;
    public int Length { get; set; }
    public double BarHeight { get; set; }
}

public partial class MainWindowViewModel : ViewModelBase
{
    private const double ChartMaxBarHeight = 240.0;

    [ObservableProperty]
    private ObservableCollection<FastaSequence> _sequences = new();

    [ObservableProperty]
    private FastaSequence? _selectedSequence;

    [ObservableProperty]
    private string _statusMessage = "Gotowy. Wczytaj plik(i) FASTA, aby rozpocząć.";

    [ObservableProperty]
    private ObservableCollection<BarItem> _chartBars = new();

    [ObservableProperty]
    private int _chartMaxLength;

    public bool HasSequences => Sequences.Count > 0;
    public bool HasSelection => SelectedSequence != null;

    public MainWindowViewModel()
    {
        Sequences.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HasSequences));
            UpdateChart();
        };
    }

    partial void OnSelectedSequenceChanged(FastaSequence? value)
    {
        OnPropertyChanged(nameof(HasSelection));
    }

    [RelayCommand]
    private async Task LoadFilesAsync()
    {
        var topLevel = GetTopLevel();
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Wybierz plik(i) FASTA",
            AllowMultiple = true,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Pliki FASTA")
                {
                    Patterns = new[] { "*.fasta", "*.fa", "*.fna", "*.ffn", "*.faa", "*.frn", "*.txt" }
                },
                FilePickerFileTypes.All
            }
        });

        if (files == null || files.Count == 0) return;

        int loaded = 0;
        int failed = 0;
        var errors = new List<string>();

        foreach (var file in files)
        {
            try
            {
                var path = file.TryGetLocalPath();
                if (string.IsNullOrEmpty(path)) continue;

                var parsed = FastaParser.ParseFile(path);
                foreach (var seq in parsed)
                {
                    Sequences.Add(seq);
                    loaded++;
                }
            }
            catch (Exception ex)
            {
                failed++;
                errors.Add($"{file.Name}: {ex.Message}");
            }
        }

        StatusMessage = failed == 0
            ? $"Wczytano {loaded} sekwencji z {files.Count} plik(ów)."
            : $"Wczytano {loaded} sekwencji. Błędy: {failed}. {string.Join(" | ", errors)}";
    }

    [RelayCommand]
    private void ClearSequences()
    {
        Sequences.Clear();
        SelectedSequence = null;
        StatusMessage = "Lista wyczyszczona.";
    }

    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        if (Sequences.Count == 0) return;
        var topLevel = GetTopLevel();
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Zapisz wyniki do CSV",
            SuggestedFileName = "analiza_fasta.csv",
            DefaultExtension = "csv",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } }
            }
        });

        if (file == null) return;
        var path = file.TryGetLocalPath();
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            AnalysisExporter.ExportCsv(Sequences, path);
            StatusMessage = $"Wyeksportowano CSV: {path}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Błąd eksportu CSV: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ExportJsonAsync()
    {
        if (Sequences.Count == 0) return;
        var topLevel = GetTopLevel();
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Zapisz wyniki do JSON",
            SuggestedFileName = "analiza_fasta.json",
            DefaultExtension = "json",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } }
            }
        });

        if (file == null) return;
        var path = file.TryGetLocalPath();
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            AnalysisExporter.ExportJson(Sequences, path);
            StatusMessage = $"Wyeksportowano JSON: {path}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Błąd eksportu JSON: {ex.Message}";
        }
    }

    private void UpdateChart()
    {
        ChartBars.Clear();
        if (Sequences.Count == 0)
        {
            ChartMaxLength = 0;
            return;
        }

        var max = Sequences.Max(s => s.Length);
        ChartMaxLength = max;
        var safeMax = Math.Max(max, 1);

        foreach (var s in Sequences)
        {
            ChartBars.Add(new BarItem
            {
                Name = s.Name,
                Length = s.Length,
                BarHeight = ChartMaxBarHeight * s.Length / safeMax
            });
        }
    }

    private static TopLevel? GetTopLevel()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }
}
