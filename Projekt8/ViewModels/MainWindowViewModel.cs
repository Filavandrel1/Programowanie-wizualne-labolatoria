using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Projekt8.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Title { get; } = "Hub gier karcianych";

    public ObservableCollection<GameItemViewModel> Games { get; } = new()
    {
        new GameItemViewModel("Wojna", "Klasyczna gra na dwie osoby"),
        new GameItemViewModel("Makao", "Gra towarzyska z kartami"),
        new GameItemViewModel("Pasjans", "Jednoosobowy klasyk"),
    };

    [ObservableProperty] private string? _statusMessage;

    [RelayCommand]
    private void PlayGame(GameItemViewModel? game)
    {
        if (game is null) return;
        StatusMessage = $"Wybrano gre: {game.Name} (jeszcze nie zaimplementowano)";
    }
}

public partial class GameItemViewModel : ViewModelBase
{
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _description = "";

    public GameItemViewModel() { }

    public GameItemViewModel(string name, string description)
    {
        _name = name;
        _description = description;
    }
}
