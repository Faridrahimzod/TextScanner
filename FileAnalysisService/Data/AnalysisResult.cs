using System.Collections.Generic;
using System;
namespace FileAnalysisService.Data;

public class AnalysisResult
{
    public Guid Id { get; set; }
    public string FileId { get; set; } = default!;
    public int Paragraphs { get; set; }
    public int Words { get; set; }
    public int Characters { get; set; }
    public Dictionary<string,int> WordFrequencies { get; set; } = new();
}
