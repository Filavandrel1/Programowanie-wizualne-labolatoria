using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Projekt2.ViewModels;

public partial class TransportViewModel : ViewModelBase
{
    private readonly Action<string> _navigate;

    [ObservableProperty]
    private bool _isInpost;

    [ObservableProperty]
    private bool _isDpd;

    [ObservableProperty]
    private bool _isFedex;

    [ObservableProperty]
    private bool _isNaMiejscu;

    [ObservableProperty]
    private string _wybranyTransport = "";

    [ObservableProperty]
    private bool _isTransportWybrany;

    public TransportViewModel(Action<string> navigate)
    {
        _navigate = navigate;
    }

    partial void OnIsInpostChanged(bool value) => UpdateTransportWybrany();
    partial void OnIsDpdChanged(bool value) => UpdateTransportWybrany();
    partial void OnIsFedexChanged(bool value) => UpdateTransportWybrany();
    partial void OnIsNaMiejscuChanged(bool value) => UpdateTransportWybrany();

    private void UpdateTransportWybrany()
    {
        IsTransportWybrany = IsInpost || IsDpd || IsFedex || IsNaMiejscu;
    }

    [RelayCommand]
    private void Zatwierdz()
    {
        if (IsInpost) WybranyTransport = "Kurier InPost";
        else if (IsDpd) WybranyTransport = "Kurier DPD";
        else if (IsFedex) WybranyTransport = "Kurier FedEx";
        else if (IsNaMiejscu) WybranyTransport = "Na miejscu";
        else WybranyTransport = "Nie wybrano";

        _navigate("Koszyk");
    }

    [RelayCommand]
    private void Anuluj()
    {
        IsInpost = false;
        IsDpd = false;
        IsFedex = false;
        IsNaMiejscu = false;
        WybranyTransport = "";

        _navigate("Koszyk");
    }
}
