using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Projekt9.Views;

public partial class FormView : UserControl
{
    public FormView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
