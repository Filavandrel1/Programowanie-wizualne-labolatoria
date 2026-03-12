using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Projekt2.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    // Współdzielona lista produktów w koszyku
    public ObservableCollection<string> KoszykItems { get; } = new();

    public KoszykViewModel KoszykVm { get; }
    public SklepViewModel SklepVm { get; }
    public TransportViewModel TransportVm { get; }
    public PlatnoscViewModel PlatnoscVm { get; }

    // Akcja nawigacji przekazywana do child ViewModeli
    public Action<string> Navigate { get; }

    public MainWindowViewModel()
    {
        Navigate = NavigateTo;

        TransportVm = new TransportViewModel(Navigate);
        KoszykVm = new KoszykViewModel(KoszykItems, Navigate, TransportVm);
        SklepVm = new SklepViewModel(KoszykItems, Navigate);
        PlatnoscVm = new PlatnoscViewModel(Navigate);

        // Start z Koszykiem
        _currentView = KoszykVm;
    }

    private void NavigateTo(string viewName)
    {
        CurrentView = viewName switch
        {
            "Koszyk" => KoszykVm,
            "Sklep" => SklepVm,
            "Transport" => TransportVm,
            "Platnosc" => PlatnoscVm,
            _ => KoszykVm
        };
    }
}
