using System.Text.RegularExpressions;

namespace IndependentWork14;

public class SearchIndex
{
    private readonly Dictionary<string, HashSet<int>> _invertedIndex = new();

    public int WordCount => _invertedIndex.Count;

    public void AddDocument(int docId, string content)
    {
        foreach (string word in SplitIntoWords(content))
        {
            if (!_invertedIndex.TryGetValue(word, out HashSet<int>? documents))
            {
                documents = new HashSet<int>();
                _invertedIndex.Add(word, documents);
            }

            documents.Add(docId);
        }
    }

    public HashSet<int> Search(string word)
    {
        return _invertedIndex.TryGetValue(word.ToLowerInvariant(), out HashSet<int>? documents)
            ? new HashSet<int>(documents)
            : new HashSet<int>();
    }

    public HashSet<int> SearchMultiple(params string[] words)
    {
        if (words.Length == 0)
        {
            return new HashSet<int>();
        }

        HashSet<int> result = Search(words[0]);

        foreach (string word in words.Skip(1))
        {
            result.IntersectWith(Search(word));
        }

        return result;
    }

    private static IEnumerable<string> SplitIntoWords(string text)
    {
        return Regex.Matches(text.ToLowerInvariant(), @"[\p{L}\p{N}]+").Select(match => match.Value);
    }
}
