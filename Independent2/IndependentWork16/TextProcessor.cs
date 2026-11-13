using System.Text.RegularExpressions;

namespace IndependentWork16;

public static class TextProcessor
{
    private static readonly Regex WordPattern = new(@"[\w#+]+", RegexOptions.Compiled);

    private static readonly HashSet<string> StopWords = new()
    {
        "a", "an", "the", "is", "are", "was", "were",
        "in", "on", "at", "to", "for", "of", "and", "or"
    };

    public static IEnumerable<string> Tokenize(string text)
    {
        return WordPattern.Matches(text.ToLowerInvariant()).Select(match => match.Value);
    }

    public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
    {
        return words.Where(word => !StopWords.Contains(word));
    }

    public static IEnumerable<string> Normalize(string text)
    {
        return RemoveStopWords(Tokenize(text));
    }
}
