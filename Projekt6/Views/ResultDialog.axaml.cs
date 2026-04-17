using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Projekt6.Views;

public partial class ResultDialog : Window
{
    public ResultDialog()
    {
        InitializeComponent();
    }

    public ResultDialog(string title, string message) : this()
    {
        Title = title;
        MessageText.Text = message;
    }

    private void OnOkClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
