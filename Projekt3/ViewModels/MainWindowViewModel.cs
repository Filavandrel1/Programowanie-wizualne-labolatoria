using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt3.Models;
using Projekt3.Services;
using Projekt3.Views;

namespace Projekt3.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Wspólne źródło danych - odpowiednik BindingSource z WinForms.
    /// Przechowuje kolekcję pracowników współdzieloną między oknami.
    /// </summary>
    private readonly PracownicyStore _store;

    /// <summary>
    /// Kolekcja pracowników wystawiona dla DataGrid - odpowiednik:
    ///   dataGridView1.DataSource = bindingSource1;
    /// DataGrid binduje do tej właściwości, a ona wskazuje na Store.Pracownicy.
    /// </summary>
    public ObservableCollection<Pracownik> Pracownicy => _store.Pracownicy;

    [ObservableProperty]
    private Pracownik? _wybranyPracownik;

    public MainWindowViewModel()
    {
        // Inicjalizacja Store - odpowiednik tworzenia DataTable + BindingSource w Form1()
        _store = new PracownicyStore();

        // Dodanie przykładowych danych - odpowiednik:
        //   dataTable.Rows.Add(1, "Jan", "Nowak");
        //   dataTable.Rows.Add(2, "Joanna", "Kowalska");
        _store.ZaladujPrzykladoweDane();
    }

    /// <summary>
    /// Pomocnicza metoda zwracająca główne okno aplikacji.
    /// Potrzebna do wyświetlania SaveFileDialog / OpenFileDialog.
    /// </summary>
    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

    /// <summary>
    /// Otwiera drugie okno (formularz) i przekazuje Store.
    /// Po zatwierdzeniu w oknie 2 pracownik jest dodawany do Store,
    /// a ponieważ DataGrid binduje do Store.Pracownicy (ObservableCollection),
    /// nowy wiersz automatycznie pojawia się w oknie 1.
    /// </summary>
    [RelayCommand]
    private void Dodaj()
    {
        // Tworzymy ViewModel drugiego okna i przekazujemy wspólny Store
        var vm = new SecondWindowViewModel(_store);
        var secondWindow = new SecondWindow
        {
            DataContext = vm
        };

        // Zamknij okno po dodaniu pracownika
        vm.OknoZamkniete += (_, _) =>
        {
            secondWindow.Close();
        };

        secondWindow.Show();
    }

    [RelayCommand]
    private void Usun()
    {
        if (WybranyPracownik != null)
        {
            _store.Usun(WybranyPracownik);
            WybranyPracownik = null;
        }
    }

    /// <summary>
    /// Odpowiednik btnSaveToCSV_Click - wyświetla SaveFileDialog
    /// i zapisuje dane do wybranego pliku CSV.
    /// </summary>
    [RelayCommand]
    private async Task ZapiszDoCsv()
    {
        try
        {
            var window = GetMainWindow();
            if (window == null) return;

            // Wyświetlanie okna dialogowego wyboru lokalizacji zapisu
            // (odpowiednik SaveFileDialog z WinForms)
            var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Wybierz lokalizację zapisu pliku CSV",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new("Pliki CSV (*.csv)") { Patterns = new[] { "*.csv" } },
                    new("Wszystkie pliki (*.*)") { Patterns = new[] { "*.*" } }
                },
                DefaultExtension = "csv",
                SuggestedFileName = "pracownicy"
            });

            // Jeśli użytkownik wybierze lokalizację i zatwierdzi, zapisz plik CSV
            if (file != null)
            {
                var filePath = file.Path.LocalPath;
                // Użyj metody ExportToCSV i podaj ścieżkę do pliku CSV
                _store.ExportToCSV(filePath);
            }
        }
        catch (Exception)
        {
            // obsługa błędów
        }
    }

    /// <summary>
    /// Odpowiednik btnLoadCSV_Click - wyświetla OpenFileDialog
    /// i wczytuje dane z wybranego pliku CSV.
    /// </summary>
    [RelayCommand]
    private async Task OdczytajZCsv()
    {
        try
        {
            var window = GetMainWindow();
            if (window == null) return;

            // Wyświetlenie okna dialogowego wyboru pliku CSV
            // (odpowiednik OpenFileDialog z WinForms)
            var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Wybierz plik CSV do wczytania",
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("Pliki CSV (*.csv)") { Patterns = new[] { "*.csv" } },
                    new("Wszystkie pliki (*.*)") { Patterns = new[] { "*.*" } }
                },
                AllowMultiple = false
            });

            // Jeśli użytkownik wybierze plik i zatwierdzi, wczytaj dane z pliku CSV
            if (files.Count > 0)
            {
                var filePath = files[0].Path.LocalPath;
                // Wywołanie funkcji wczytującej dane z pliku CSV
                _store.LoadCSVToDataGridView(filePath);
            }
        }
        catch (Exception)
        {
            // obsługa błędów
        }
    }

    /// <summary>
    /// Serializuje dane z tabeli do pliku JSON.
    /// Dane z kolekcji Pracownicy są transformowane na obiekty PracownikJson (DTO)
    /// i serializowane za pomocą System.Text.Json.JsonSerializer.
    /// </summary>
    [RelayCommand]
    private async Task ZapiszDoJson()
    {
        try
        {
            var window = GetMainWindow();
            if (window == null) return;

            // Wyświetlenie okna dialogowego wyboru lokalizacji zapisu
            var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Wybierz lokalizację zapisu pliku JSON",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new("Pliki JSON (*.json)") { Patterns = new[] { "*.json" } },
                    new("Wszystkie pliki (*.*)") { Patterns = new[] { "*.*" } }
                },
                DefaultExtension = "json",
                SuggestedFileName = "pracownicy"
            });

            // Jeśli użytkownik wybierze lokalizację i zatwierdzi, zapisz plik JSON
            if (file != null)
            {
                var filePath = file.Path.LocalPath;
                _store.ExportToJson(filePath);
            }
        }
        catch (Exception)
        {
            // obsługa błędów
        }
    }
}
