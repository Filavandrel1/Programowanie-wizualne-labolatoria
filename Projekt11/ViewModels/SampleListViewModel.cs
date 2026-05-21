using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt11.Models;
using Projekt11.Services;

namespace Projekt11.ViewModels;

/// <summary>
/// Lista próbek z filtrowaniem (po typie) i wyszukiwaniem (po dowolnym polu).
/// </summary>
public partial class SampleListViewModel : ViewModelBase
{
    private readonly SampleDatabase _db = new();
    private List<Sample> _all = new();

    public event Action? NewSampleRequested;
    public event Action<Sample>? EditSampleRequested;
    public event Action<Sample>? LabelRequested;

    /// <summary>Aktualnie widoczne próbki (po filtrowaniu).</summary>
    public ObservableCollection<Sample> Items { get; } = new();

    /// <summary>Wszystkie dostępne wartości filtra typu (z opcją „Wszystkie”).</summary>
    public IReadOnlyList<string> TypeFilters { get; } =
        new[] { "Wszystkie", "DNA", "RNA", "Bialko", "Inny" };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private string _search = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private string _selectedType = "Wszystkie";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [NotifyCanExecuteChangedFor(nameof(LabelCommand))]
    private Sample? _selected;

    public bool IsEmpty => Items.Count == 0;
    public bool HasItems => Items.Count > 0;

    public SampleListViewModel()
    {
        Reload();
    }

    partial void OnSearchChanged(string value) => ApplyFilter();
    partial void OnSelectedTypeChanged(string value) => ApplyFilter();

    private void Reload()
    {
        _all = _db.GetAll();
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        IEnumerable<Sample> q = _all;

        if (!string.IsNullOrEmpty(SelectedType) && SelectedType != "Wszystkie")
            q = q.Where(s => s.Type.ToString().Equals(SelectedType, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var needle = Search.Trim();
            q = q.Where(s =>
                (s.SampleId    ?? "").Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                (s.Name        ?? "").Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                (s.Description ?? "").Contains(needle, StringComparison.OrdinalIgnoreCase));
        }

        Items.Clear();
        foreach (var s in q) Items.Add(s);

        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(HasItems));
    }

    [RelayCommand]
    private void Refresh() => Reload();

    [RelayCommand]
    private void New() => NewSampleRequested?.Invoke();

    private bool HasSelection() => Selected is not null;

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Edit()
    {
        if (Selected is not null) EditSampleRequested?.Invoke(Selected);
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Delete()
    {
        if (Selected is null) return;
        _db.Delete(Selected.RowId);
        Reload();
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Label()
    {
        if (Selected is not null) LabelRequested?.Invoke(Selected);
    }

    [RelayCommand]
    private void ClearSearch() => Search = string.Empty;
}
