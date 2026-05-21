using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt11.Models;
using Projekt11.Services;

namespace Projekt11.ViewModels;

/// <summary>
/// Formularz nowej / edytowanej próbki.
/// </summary>
public partial class SampleEditViewModel : ViewModelBase
{
    private readonly SampleDatabase _db = new();

    public event Action? Saved;
    public event Action? Cancelled;

    [ObservableProperty] private int _rowId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _sampleId = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _name = string.Empty;

    [ObservableProperty] private string _selectedType = "DNA";
    [ObservableProperty] private DateTimeOffset? _collectionDate = DateTimeOffset.Now;
    [ObservableProperty] private string _description = string.Empty;

    [ObservableProperty] private string _statusMessage = string.Empty;

    public IReadOnlyList<string> Types { get; } = new[] { "DNA", "RNA", "Bialko", "Inny" };

    public bool IsEditMode => RowId > 0;
    public string ScreenTitle => IsEditMode ? "Edycja próbki" : "Nowa próbka";

    public SampleEditViewModel() { }

    public SampleEditViewModel(Sample s) : this()
    {
        RowId = s.RowId;
        SampleId = s.SampleId;
        Name = s.Name;
        SelectedType = s.Type.ToString();
        CollectionDate = new DateTimeOffset(s.CollectionDate);
        Description = s.Description;
        OnPropertyChanged(nameof(IsEditMode));
        OnPropertyChanged(nameof(ScreenTitle));
    }

    private bool CanSave() =>
        !string.IsNullOrWhiteSpace(SampleId) &&
        !string.IsNullOrWhiteSpace(Name);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        try
        {
            if (_db.SampleIdExists(SampleId.Trim(), ignoreRowId: RowId))
            {
                StatusMessage = $"❌ Próbka o ID „{SampleId}” już istnieje.";
                return;
            }

            var sample = new Sample
            {
                RowId          = RowId,
                SampleId       = SampleId.Trim(),
                Name           = Name.Trim(),
                Type           = Enum.TryParse<SampleType>(SelectedType, out var t) ? t : SampleType.Inny,
                CollectionDate = CollectionDate?.DateTime ?? DateTime.Today,
                Description    = Description ?? string.Empty,
            };

            if (IsEditMode) _db.Update(sample);
            else            _db.Insert(sample);

            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            StatusMessage = "❌ Błąd zapisu: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Cancel() => Cancelled?.Invoke();
}
