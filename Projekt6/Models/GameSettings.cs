namespace Projekt6.Models;

public class GameSettings
{
    public int BoardX { get; set; } = 3;
    public int BoardY { get; set; } = 3;
    public int TimeSeconds { get; set; } = 60;
    public int Dydelfy { get; set; } = 6;
    public int Szopy { get; set; } = 3;
    public int Krokodyle { get; set; } = 1;

    public const int MinBoard = 3;
    public const int MaxBoard = 10;
    public const int MinTime = 10;
    public const int MaxTime = 60;
    public const int MinDydelfy = 1;
    public const int MaxDydelfy = 6;
    public const int MinSzopy = 3;
    public const int MaxSzopy = 8;
    public const int MinKrokodyle = 0;
    public const int MaxKrokodyle = 1;

    public static GameSettings Instance { get; } = new GameSettings();
}
