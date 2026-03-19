using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt3.Models;
using Projekt3.Services;

namespace Projekt3.ViewModels;

public partial class SecondWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Referencja do wspólnego Store - ten sam obiekt co w MainWindowViewModel.
    /// Dzięki temu dodanie pracownika tutaj natychmiast aktualizuje DataGrid w oknie 1,
    /// bo oba okna korzystają z tej samej ObservableCollection (jak BindingSource w WinForms).
    /// </summary>
    private readonly PracownicyStore _store;

    [ObservableProperty]
    private string _imie = string.Empty;

    [ObservableProperty]
    private string _nazwisko = string.Empty;

    [ObservableProperty]
    private string _wiekText = string.Empty;

    [ObservableProperty]
    private string? _wybraneStanowisko;

    public ObservableCollection<string> Stanowiska { get; } = new()
    {
        "Programista",
        "Tester",
        "Analityk",
        "Kierownik",
        "Stażysta",
        "Designer",
        "Administrator"
    };

    /// <summary>
    /// Event informujący o zamknięciu okna (po zatwierdzeniu lub anulowaniu).
    /// </summary>
    public event EventHandler? OknoZamkniete;

    public SecondWindowViewModel() : this(new PracownicyStore()) { }

    public SecondWindowViewModel(PracownicyStore store)
    {
        _store = store;
    }

    /// <summary>
    /// Zatwierdza formularz - dodaje pracownika bezpośrednio do Store.
    /// Odpowiednik: dataTable.Rows.Add(id, imie, nazwisko);
    /// Ponieważ Store.Pracownicy to ObservableCollection,
    /// DataGrid w oknie 1 automatycznie wyświetli nowy wiersz.
    /// </summary>
    [RelayCommand]
    private void Zatwierdz()
    {
        int.TryParse(WiekText, out var wiek);

        var pracownik = new Pracownik
        {
            Imie = Imie,
            Nazwisko = Nazwisko,
            Wiek = wiek,
            Stanowisko = WybraneStanowisko ?? string.Empty
        };

        // Dodanie do wspólnego Store - dane od razu widoczne w oknie 1
        _store.Dodaj(pracownik);

        OknoZamkniete?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Anuluj()
    {
        OknoZamkniete?.Invoke(this, EventArgs.Empty);
    }
}
