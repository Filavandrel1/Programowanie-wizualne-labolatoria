using System;

namespace Projekt10.Models;

public class FastaSequence
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sequence { get; set; } = string.Empty;
    public string SourceFile { get; set; } = string.Empty;

    public int Length => Sequence.Length;

    public int CountA => CountBase('A');
    public int CountT => CountBase('T');
    public int CountG => CountBase('G');
    public int CountC => CountBase('C');
    public int CountN => CountBase('N');
    public int CountOther
    {
        get
        {
            int other = 0;
            foreach (var c in Sequence)
            {
                var u = char.ToUpperInvariant(c);
                if (u != 'A' && u != 'T' && u != 'G' && u != 'C' && u != 'N') other++;
            }
            return other;
        }
    }

    public double GCContent
    {
        get
        {
            if (Length == 0) return 0.0;
            var gc = CountG + CountC;
            return Math.Round(100.0 * gc / Length, 2);
        }
    }

    public int CodonCount => Length / 3;

    private int CountBase(char target)
    {
        int count = 0;
        var t = char.ToUpperInvariant(target);
        foreach (var c in Sequence)
        {
            if (char.ToUpperInvariant(c) == t) count++;
        }
        return count;
    }
}
