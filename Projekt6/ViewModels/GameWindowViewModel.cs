using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Projekt6.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Projekt6.ViewModels;

public partial class GameWindowViewModel : ViewModelBase
{
    public ObservableCollection<CellViewModel> Cells { get; } = new();

    public int Columns { get; }
    public int Rows { get; }

    [ObservableProperty] private string _timeText = "";
    [ObservableProperty] private bool _isBoardEnabled = true;

    private readonly int _totalDydelfy;
    private int _foundDydelfy;

    private readonly DispatcherTimer _gameTimer;
    private int _remainingSeconds;

    private DispatcherTimer? _crocodileTimer;
    private CellViewModel? _pendingCrocodile;

    public event Action<string, bool>? GameEnded;

    public GameWindowViewModel()
    {
        var s = GameSettings.Instance;
        Columns = s.BoardX;
        Rows = s.BoardY;
        _totalDydelfy = s.Dydelfy;
        _remainingSeconds = s.TimeSeconds;
        TimeText = $"{_remainingSeconds} s";

        BuildBoard(s);

        _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _gameTimer.Tick += OnGameTick;
        _gameTimer.Start();
    }

    private void BuildBoard(GameSettings s)
    {
        int total = Rows * Columns;
        int dydelfy = s.Dydelfy;
        int szopy = s.Szopy;
        int krokodyle = s.Krokodyle;

        if (dydelfy + szopy + krokodyle > total)
        {
            int overflow = dydelfy + szopy + krokodyle - total;
            szopy = Math.Max(0, szopy - overflow);
        }

        
        var list = new List<CellViewModel>();
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Columns; c++)
                list.Add(new CellViewModel(r, c));

        var rnd = new Random();
        var indices = new List<int>();
        for (int i = 0; i < total; i++) indices.Add(i);
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        int k = 0;
        for (int i = 0; i < dydelfy && k < indices.Count; i++, k++)
            list[indices[k]].Content = CellContent.Dydelf;
        for (int i = 0; i < szopy && k < indices.Count; i++, k++)
            list[indices[k]].Content = CellContent.Szop;
        for (int i = 0; i < krokodyle && k < indices.Count; i++, k++)
            list[indices[k]].Content = CellContent.Krokodyl;

        foreach (var cell in list)
        {
            cell.Clicked += OnCellClicked;
            Cells.Add(cell);
        }
    }

    private void OnGameTick(object? sender, EventArgs e)
    {
        _remainingSeconds--;
        if (_remainingSeconds <= 0)
        {
            _remainingSeconds = 0;
            TimeText = "0 s";
            EndGame("Koniec czasu! Przegrana.", won: false);
            return;
        }
        TimeText = $"{_remainingSeconds} s";
    }

    private void OnCellClicked(CellViewModel cell)
    {
        if (!IsBoardEnabled) return;

        if (_pendingCrocodile != null)
        {
            if (cell == _pendingCrocodile)
            {
                StopCrocodileTimer();
                cell.Hide();
                cell.IsEnabled = true; 
                cell.IsEnabled = false;
                _pendingCrocodile = null;
                return;
            }
            
            return;
        }

        if (cell.IsRevealed || !cell.IsEnabled) return;

        cell.Reveal();

        switch (cell.Content)
        {
            case CellContent.Empty:
                cell.IsEnabled = false;
                break;

            case CellContent.Dydelf:
                _foundDydelfy++;
                cell.IsEnabled = false;
                if (_foundDydelfy >= _totalDydelfy)
                {
                    EndGame("Wygrana! Znaleziono wszystkie Dydelfy.", won: true);
                }
                break;

            case CellContent.Szop:
                ScheduleSzopClose(cell);
                break;

            case CellContent.Krokodyl:
                StartCrocodileTimer(cell);
                break;
        }
    }

    private void ScheduleSzopClose(CellViewModel szop)
    {
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            if (!IsBoardEnabled) return;

            
            foreach (var neighbor in GetNeighborsIncluding(szop))
            {
                if (neighbor.Content == CellContent.Dydelf && neighbor.IsRevealed)
                {
                    _foundDydelfy--;
                }
                neighbor.Hide();
                neighbor.IsEnabled = true;
            }
        };
        timer.Start();
    }

    private IEnumerable<CellViewModel> GetNeighborsIncluding(CellViewModel center)
    {
        for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                int r = center.Row + dr;
                int c = center.Col + dc;
                if (r < 0 || r >= Rows || c < 0 || c >= Columns) continue;
                var cell = GetCell(r, c);
                if (cell != null) yield return cell;
            }
    }

    private CellViewModel? GetCell(int row, int col)
    {
        int idx = row * Columns + col;
        return idx >= 0 && idx < Cells.Count ? Cells[idx] : null;
    }

    private void StartCrocodileTimer(CellViewModel cell)
    {
        _pendingCrocodile = cell;
        _crocodileTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _crocodileTimer.Tick += (_, _) =>
        {
            StopCrocodileTimer();
            if (_pendingCrocodile != null)
            {
                _pendingCrocodile = null;
                EndGame("Krokodyl! Nie zdazyles zamknac kosza. Przegrana.", won: false);
            }
        };
        _crocodileTimer.Start();
    }

    private void StopCrocodileTimer()
    {
        _crocodileTimer?.Stop();
        _crocodileTimer = null;
    }

    private void EndGame(string message, bool won)
    {
        IsBoardEnabled = false;
        _gameTimer.Stop();
        StopCrocodileTimer();
        foreach (var cell in Cells) cell.IsEnabled = false;
        GameEnded?.Invoke(message, won);
    }
}
