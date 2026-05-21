using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Projekt11.Views;

public partial class SampleListView : UserControl
{
    public SampleListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
