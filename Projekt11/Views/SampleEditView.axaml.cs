using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Projekt11.Views;

public partial class SampleEditView : UserControl
{
    public SampleEditView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
