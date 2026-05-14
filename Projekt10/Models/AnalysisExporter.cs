using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

namespace Projekt10.Models;

public static class AnalysisExporter
{
    public static void ExportCsv(IEnumerable<FastaSequence> sequences, string path)
    {
        var rows = sequences.Select(s => new
        {
            s.Name,
            s.Description,
            s.SourceFile,
            Length = s.Length,
            GCContent = s.GCContent,
            CodonCount = s.CodonCount,
            CountA = s.CountA,
            CountT = s.CountT,
            CountG = s.CountG,
            CountC = s.CountC,
            CountN = s.CountN,
            CountOther = s.CountOther
        });

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };

        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, config);
        csv.WriteRecords(rows);
    }

    public static void ExportJson(IEnumerable<FastaSequence> sequences, string path)
    {
        var data = sequences.Select(s => new
        {
            s.Name,
            s.Description,
            s.SourceFile,
            Length = s.Length,
            GCContent = s.GCContent,
            CodonCount = s.CodonCount,
            BaseCounts = new
            {
                A = s.CountA,
                T = s.CountT,
                G = s.CountG,
                C = s.CountC,
                N = s.CountN,
                Other = s.CountOther
            }
        });

        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
