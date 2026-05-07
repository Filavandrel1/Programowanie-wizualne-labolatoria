using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt9.Models;
using Projekt9.Services;

namespace Projekt9.ViewModels;

/// <summary>
/// Ekran listy zapisanych formularzy: w lewej kolumnie lista wszystkich
/// wpisów z bazy SQLite, po prawej podgląd zaznaczonego formularza
/// (wszystkie 15 pól wniosku tylko-do-odczytu).
/// </summary>
public partial class SavedFormsViewModel : ViewModelBase
{
    private readonly FormDatabase _db = new();

    public event Action? BackRequested;

    /// <summary>Wszystkie zapisane formularze (od najnowszego).</summary>
    public ObservableCollection<FormEntry> Entries { get; } = new();

    /// <summary>Formularz wybrany przez użytkownika z listy.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private FormEntry? _selected;

    /// <summary>True, gdy w bazie nie ma jeszcze żadnych wpisów.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEntries))]
    private bool _isEmpty;

    public bool HasSelection => Selected is not null;
    public bool HasEntries => !IsEmpty;

    public SavedFormsViewModel()
    {
        Refresh();
    }

    [RelayCommand]
    private void Refresh()
    {
        Entries.Clear();
        foreach (var e in _db.GetAll())
            Entries.Add(e);

        IsEmpty = Entries.Count == 0;
        Selected = Entries.Count > 0 ? Entries[0] : null;
    }

    [RelayCommand]
    private void Back() => BackRequested?.Invoke();
}
