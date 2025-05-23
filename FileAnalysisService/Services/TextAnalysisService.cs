using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using FileAnalysisService.Data;

namespace FileAnalysisService.Services
{
    public static class TextAnalysisService
    {
        public static AnalysisResult Analyze(string fileId, string content)
        {
            var paragraphs = content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Length;

            var wordsArr = Regex.Matches(content, @"\b[\p{L}\p{Nd}]+\b")
                                 .Cast<Match>()
                                 .Select(m => m.Value.ToLowerInvariant())
                                 .ToArray();

            var wordsCount = wordsArr.Length;
            var charsCount = content.Length;

            var freqs = wordsArr
                        .GroupBy(w => w)
                        .ToDictionary(g => g.Key, g => g.Count());

            return new AnalysisResult
            {
                Id = Guid.NewGuid(),
                FileId = fileId,
                Paragraphs = paragraphs,
                Words = wordsCount,
                Characters = charsCount,
                WordFrequencies = freqs
            };
        }

        public static double JaccardSimilarity(string a, string b)
        {
            var setA = new HashSet<string>(a.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            var setB = new HashSet<string>(b.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            var intersection = setA.Intersect(setB).Count();
            var union = setA.Union(setB).Count();
            return union == 0 ? 0 : (double)intersection / union;
        }
    }
}

