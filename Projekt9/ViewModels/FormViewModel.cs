using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt9.Models;
using Projekt9.Services;

namespace Projekt9.ViewModels;

/// <summary>
/// ViewModel ekranu wypełniania formularza "Wniosek o przeprowadzenie
/// egzaminu komisyjnego". Zawiera 15 pól wniosku + dodatkowe pole
/// "Nazwa formularza", pod którym wpis zostanie zapisany w bazie.
/// </summary>
public partial class FormViewModel : ViewModelBase
{
    private readonly FormDatabase _db = new();

    public event Action? BackRequested;

    // ----- meta -----

    /// <summary>Nazwa, pod jaką formularz zostanie zapisany w bazie.</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _formName = string.Empty;

    /// <summary>Komunikat zwrotny po zapisie/walidacji.</summary>
    [ObservableProperty] private string _statusMessage = string.Empty;

    // ----- 15 pól wniosku -----

    [ObservableProperty] private DateTimeOffset? _requestDate = DateTimeOffset.Now; // 1
    [ObservableProperty] private string _albumNumber = string.Empty;                 // 2
    [ObservableProperty] private string _fullName = string.Empty;                    // 3
    [ObservableProperty] private string _semesterYear = string.Empty;                // 4
    [ObservableProperty] private string _fieldAndDegree = string.Empty;              // 5

    [ObservableProperty] private string _subject = string.Empty;                     // 6
    [ObservableProperty] private string _points = string.Empty;                      // 7
    [ObservableProperty] private string _lecturer = string.Empty;                    // 8
    [ObservableProperty] private string _justification = string.Empty;               // 9
    [ObservableProperty] private DateTimeOffset? _studentSignDate = DateTimeOffset.Now; // 10 (data)
    [ObservableProperty] private string _studentSignature = string.Empty;             // 10 (podpis)

    [ObservableProperty] private bool _decisionApproved = true;                      // 11
    [ObservableProperty] private string _committeeMember1 = string.Empty;            // 12
    [ObservableProperty] private string _committeeMember2 = string.Empty;            // 13
    [ObservableProperty] private string _committeeMember3 = string.Empty;            // 14
    [ObservableProperty] private DateTimeOffset? _decisionDate = DateTimeOffset.Now; // 15

    [RelayCommand]
    private void Back() => BackRequested?.Invoke();

    private bool CanSave() => !string.IsNullOrWhiteSpace(FormName);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        var entry = new FormEntry
        {
            Name = FormName.Trim(),
            CreatedAt = DateTime.Now,

            RequestDate = RequestDate?.DateTime,
            AlbumNumber = AlbumNumber,
            FullName = FullName,
            SemesterYear = SemesterYear,
            FieldAndDegree = FieldAndDegree,

            Subject = Subject,
            Points = Points,
            Lecturer = Lecturer,
            Justification = Justification,
            StudentSignDate = StudentSignDate?.DateTime,
            StudentSignature = StudentSignature,

            DecisionApproved = DecisionApproved,
            CommitteeMember1 = CommitteeMember1,
            CommitteeMember2 = CommitteeMember2,
            CommitteeMember3 = CommitteeMember3,
            DecisionDate = DecisionDate?.DateTime,
        };

        try
        {
            var id = _db.Save(entry);
            StatusMessage = $"✅ Zapisano formularz \"{entry.Name}\" (id={id}).";
        }
        catch (Exception ex)
        {
            StatusMessage = "❌ Błąd zapisu: " + ex.Message;
        }
    }
}
