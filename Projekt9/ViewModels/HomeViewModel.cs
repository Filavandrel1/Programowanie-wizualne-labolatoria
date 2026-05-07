using System;
using CommunityToolkit.Mvvm.Input;

namespace Projekt9.ViewModels;

/// <summary>
/// Ekran startowy z dwoma przyciskami:
///   1) Wypełnij nowy formularz
///   2) Podgląd formularzy zapisanych w bazie
/// </summary>
public partial class HomeViewModel : ViewModelBase
{
    /// <summary>
    /// Zdarzenie podnoszone, gdy użytkownik wybierze opcję z menu głównego.
    /// MainWindowViewModel nasłuchuje go i podmienia CurrentPage.
    /// </summary>
    public event Action<ViewModelBase>? NavigationRequested;

    [RelayCommand]
    private void NewForm()
    {
        var vm = new FormViewModel();
        vm.BackRequested += () => NavigationRequested?.Invoke(this);
        NavigationRequested?.Invoke(vm);
    }

    [RelayCommand]
    private void ViewSavedForms()
    {
        var vm = new SavedFormsViewModel();
        vm.BackRequested += () => NavigationRequested?.Invoke(this);
        NavigationRequested?.Invoke(vm);
    }
}
