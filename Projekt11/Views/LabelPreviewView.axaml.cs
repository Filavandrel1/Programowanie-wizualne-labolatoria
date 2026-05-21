using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Projekt11.Views;

public partial class LabelPreviewView : UserControl
{
    public LabelPreviewView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
