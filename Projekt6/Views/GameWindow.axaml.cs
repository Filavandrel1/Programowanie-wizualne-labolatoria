using Avalonia.Controls;
using Avalonia.Threading;
using Projekt6.ViewModels;
using System;

namespace Projekt6.Views;

public partial class GameWindow : Window
{
    public GameWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is GameWindowViewModel vm)
        {
            vm.GameEnded += OnGameEnded;
        }
    }

    private async void OnGameEnded(string message, bool won)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var dlg = new ResultDialog(won ? "Wygrana" : "Przegrana", message);
            await dlg.ShowDialog(this);
            Close();
        });
    }
}
