using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Projekt2.ViewModels;

public partial class KoszykViewModel : ViewModelBase
{
    public ObservableCollection<string> KoszykItems { get; }
    private readonly Action<string> _navigate;

    public TransportViewModel TransportVm { get; }

    [ObservableProperty]
    private IBrush _transportButtonColor = new SolidColorBrush(Color.Parse("#cccccc"));

    public KoszykViewModel(ObservableCollection<string> koszykItems, Action<string> navigate, TransportViewModel transportVm)
    {
        KoszykItems = koszykItems;
        _navigate = navigate;
        TransportVm = transportVm;

        // Nasłuchuj zmian w TransportVm
        TransportVm.PropertyChanged += OnTransportChanged;
        UpdateTransportButtonColor();
    }

    private void OnTransportChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TransportViewModel.IsTransportWybrany))
        {
            UpdateTransportButtonColor();
        }
    }

    private void UpdateTransportButtonColor()
    {
        TransportButtonColor = TransportVm.IsTransportWybrany
            ? new SolidColorBrush(Color.Parse("#27ae60"))
            : new SolidColorBrush(Color.Parse("#cccccc"));
    }

    [RelayCommand]
    private void GoToSklep() => _navigate("Sklep");

    [RelayCommand]
    private void GoToTransport() => _navigate("Transport");

    [RelayCommand]
    private void GoToPlatnosc()
    {
        if (TransportVm.IsTransportWybrany)
            _navigate("Platnosc");
    }
}
