using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt6.Models;
using System;
using System.Threading.Tasks;

namespace Projekt6.ViewModels;

public partial class CellViewModel : ViewModelBase
{
    public int Row { get; }
    public int Col { get; }
    public CellContent Content { get; set; }

    [ObservableProperty] private bool _isRevealed;
    [ObservableProperty] private string _displayText = "";
    [ObservableProperty] private bool _isEnabled = true;

    public event Action<CellViewModel>? Clicked;

    public CellViewModel(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public void Reveal()
    {
        IsRevealed = true;
        DisplayText = Content switch
        {
            CellContent.Dydelf => "D",
            CellContent.Szop => "S",
            CellContent.Krokodyl => "K",
            CellContent.Empty => "",
            _ => ""
        };
    }

    public void Hide()
    {
        IsRevealed = false;
        DisplayText = "";
    }

    [RelayCommand]
    private void Click()
    {
        Clicked?.Invoke(this);
    }
}
