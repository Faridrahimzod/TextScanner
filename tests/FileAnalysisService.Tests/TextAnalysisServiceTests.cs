
using FileAnalysisService.Services;
using Xunit;

public class TextAnalysisServiceTests
{
    [Fact]
    public void Analyze_ShouldCountWordsCorrectly()
    {
        var content = "hello world\n\nhello again";
        var result = TextAnalysisService.Analyze("test", content);
        Assert.Equal(2, result.Paragraphs);
        Assert.Equal(3, result.Words);
        Assert.Equal(content.Length, result.Characters);
    }

    [Fact]
    public void JaccardSimilarity_SameText_Returns1()
    {
        var sim = TextAnalysisService.JaccardSimilarity("a b c", "a b c");
        Assert.Equal(1.0, sim);
    }

    [Fact]
    public void JaccardSimilarity_NoOverlap_Returns0()
    {
        var sim = TextAnalysisService.JaccardSimilarity("a b", "c d");
        Assert.Equal(0.0, sim);
    }
}
