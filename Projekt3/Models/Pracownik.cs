using CommunityToolkit.Mvvm.ComponentModel;

namespace Projekt3.Models;

public partial class Pracownik : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _imie = string.Empty;

    [ObservableProperty]
    private string _nazwisko = string.Empty;

    [ObservableProperty]
    private int _wiek;

    [ObservableProperty]
    private string _stanowisko = string.Empty;
}
