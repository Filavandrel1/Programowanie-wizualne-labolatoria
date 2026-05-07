using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Projekt9.Views;

public partial class SavedFormsView : UserControl
{
    public SavedFormsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
