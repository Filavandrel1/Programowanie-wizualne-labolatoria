using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Projekt10.Models;

public static class FastaParser
{
    public static List<FastaSequence> ParseFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return Parse(lines, Path.GetFileName(filePath));
    }

    public static List<FastaSequence> Parse(IEnumerable<string> lines, string sourceFile = "")
    {
        var result = new List<FastaSequence>();
        FastaSequence? current = null;
        var seqBuilder = new StringBuilder();
        bool hasHeader = false;
        int lineNo = 0;

        foreach (var rawLine in lines)
        {
            lineNo++;
            var line = rawLine.TrimEnd('\r', '\n');
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith(">"))
            {
                if (current != null)
                {
                    current.Sequence = seqBuilder.ToString();
                    result.Add(current);
                }
                hasHeader = true;
                var headerContent = line.Substring(1).Trim();
                if (string.IsNullOrEmpty(headerContent))
                    throw new FormatException($"Nieprawidłowy format FASTA: pusty nagłówek w linii {lineNo}.");

                var spaceIdx = headerContent.IndexOf(' ');
                string name, desc;
                if (spaceIdx >= 0)
                {
                    name = headerContent.Substring(0, spaceIdx);
                    desc = headerContent.Substring(spaceIdx + 1).Trim();
                }
                else
                {
                    name = headerContent;
                    desc = string.Empty;
                }

                current = new FastaSequence
                {
                    Name = name,
                    Description = desc,
                    SourceFile = sourceFile
                };
                seqBuilder = new StringBuilder();
            }
            else
            {
                if (!hasHeader)
                    throw new FormatException($"Nieprawidłowy format FASTA: linia {lineNo} nie ma poprzedzającego nagłówka.");

                foreach (var c in line)
                {
                    if (char.IsWhiteSpace(c)) continue;
                    if (!char.IsLetter(c) && c != '-' && c != '*')
                        throw new FormatException($"Nieprawidłowy znak '{c}' w sekwencji (linia {lineNo}).");
                    seqBuilder.Append(c);
                }
            }
        }

        if (current != null)
        {
            current.Sequence = seqBuilder.ToString();
            result.Add(current);
        }

        if (result.Count == 0)
            throw new FormatException("Plik nie zawiera żadnych rekordów FASTA.");

        foreach (var s in result)
        {
            if (s.Sequence.Length == 0)
                throw new FormatException($"Sekwencja '{s.Name}' jest pusta.");
        }

        return result;
    }
}
