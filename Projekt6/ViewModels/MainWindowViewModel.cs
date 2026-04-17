using System;
using CommunityToolkit.Mvvm.Input;
using Projekt6.Views;

namespace Projekt6.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [RelayCommand]
    private void OpenGame()
    {
        var window = new GameWindow
        {
            DataContext = new GameWindowViewModel()
        };
        window.Show();
    }

    [RelayCommand]
    private void OpenSettings()
    {
        var window = new SettingsWindow
        {
            DataContext = new SettingsWindowViewModel()
        };
        window.Show();
    }

    [RelayCommand]
    private void Exit()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
        else
        {
            Environment.Exit(0);
        }
    }
}
