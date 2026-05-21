using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Projekt11.ViewModels;

/// <summary>
/// Główny kontener aplikacji – trzyma aktualnie wyświetlany ViewModel
/// i pozwala podstronom przełączać się między sobą.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage;

    public MainWindowViewModel()
    {
        var list = new SampleListViewModel();
        WireList(list);
        _currentPage = list;
    }

    private void Navigate(ViewModelBase vm) => CurrentPage = vm;

    private void WireList(SampleListViewModel list)
    {
        list.NewSampleRequested += () =>
        {
            var edit = new SampleEditViewModel();
            edit.Saved += () => GoToList();
            edit.Cancelled += () => GoToList();
            Navigate(edit);
        };

        list.EditSampleRequested += sample =>
        {
            var edit = new SampleEditViewModel(sample);
            edit.Saved += () => GoToList();
            edit.Cancelled += () => GoToList();
            Navigate(edit);
        };

        list.LabelRequested += sample =>
        {
            var label = new LabelPreviewViewModel(sample);
            label.BackRequested += () => GoToList();
            Navigate(label);
        };
    }

    private void GoToList()
    {
        var list = new SampleListViewModel();
        WireList(list);
        Navigate(list);
    }
}
