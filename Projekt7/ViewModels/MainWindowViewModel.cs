using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt7.Models;

namespace Projekt7.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private const int K = 4;

    [ObservableProperty] private string _dnaInput = "";
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private int _totalWindows;

    public ObservableCollection<NucleotideCount> Results { get; } = new();

    [RelayCommand]
    private void Analyze()
    {
        ErrorMessage = null;
        Results.Clear();
        TotalWindows = 0;

        var seq = (DnaInput ?? "").Trim().ToUpperInvariant();
        seq = Regex.Replace(seq, "\\s+", "");

        if (string.IsNullOrEmpty(seq))
        {
            ErrorMessage = "Wprowadz sekwencje DNA.";
            return;
        }
        if (!Regex.IsMatch(seq, "^[ACGT]+$"))
        {
            ErrorMessage = "Sekwencja moze zawierac tylko znaki A, C, G, T.";
            return;
        }
        if (seq.Length < K)
        {
            ErrorMessage = $"Sekwencja musi miec co najmniej {K} nukleotydow.";
            return;
        }

        var counts = new Dictionary<string, int>();
        for (int i = 0; i + K <= seq.Length; i++)
        {
            var sub = seq.Substring(i, K);
            counts[sub] = counts.TryGetValue(sub, out var c) ? c + 1 : 1;
        }

        TotalWindows = counts.Values.Sum();

        foreach (var kv in counts.OrderByDescending(x => x.Value).ThenBy(x => x.Key))
        {
            Results.Add(new NucleotideCount { Sequence = kv.Key, Count = kv.Value });
        }
    }

    [RelayCommand]
    private void Clear()
    {
        DnaInput = "";
        ErrorMessage = null;
        Results.Clear();
        TotalWindows = 0;
    }
}
