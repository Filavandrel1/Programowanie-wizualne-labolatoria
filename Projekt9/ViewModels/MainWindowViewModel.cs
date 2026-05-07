using CommunityToolkit.Mvvm.ComponentModel;

namespace Projekt9.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Aktualnie wyświetlany ekran (Home / Formularz / Lista zapisanych).
    /// MainWindow zawiera ContentControl podpięty pod tę właściwość, a
    /// ViewLocator zamienia ViewModel na odpowiedni View.
    /// </summary>
    [ObservableProperty]
    private ViewModelBase _currentPage;

    public MainWindowViewModel()
    {
        var home = new HomeViewModel();
        home.NavigationRequested += page => CurrentPage = page;
        _currentPage = home;
    }
}
