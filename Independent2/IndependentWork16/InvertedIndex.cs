namespace IndependentWork16;

public class InvertedIndex
{
    private readonly Dictionary<string, HashSet<int>> _index;
    private readonly Dictionary<int, Document> _documents;

    public InvertedIndex()
    {
        _index = new Dictionary<string, HashSet<int>>();
        _documents = new Dictionary<int, Document>();
    }

    public int DocumentCount => _documents.Count;

    public int UniqueWords => _index.Keys.Count;

    public void AddDocument(Document doc)
    {
        _documents.Add(doc.Id, doc);

        foreach (string word in TextProcessor.Normalize(doc.Content))
        {
            if (!_index.TryGetValue(word, out HashSet<int>? documentIds))
            {
                documentIds = new HashSet<int>();
                _index.Add(word, documentIds);
            }

            documentIds.Add(doc.Id);
        }
    }

    public List<Document> SearchSingleWord(string word)
    {
        string? normalized = NormalizeQueryWord(word);

        if (normalized is null || !_index.TryGetValue(normalized, out HashSet<int>? documentIds))
        {
            return new List<Document>();
        }

        return ToDocuments(documentIds);
    }

    public List<Document> SearchMultipleWords(params string[] words)
    {
        HashSet<int>? result = null;

        foreach (string word in words)
        {
            string? normalized = NormalizeQueryWord(word);

            if (normalized is null)
            {
                continue;
            }

            if (!_index.TryGetValue(normalized, out HashSet<int>? documentIds))
            {
                return new List<Document>();
            }

            if (result is null)
            {
                result = new HashSet<int>(documentIds);
            }
            else
            {
                result.IntersectWith(documentIds);
            }
        }

        return result is null ? new List<Document>() : ToDocuments(result);
    }

    public List<Document> SearchAnyWord(params string[] words)
    {
        var result = new HashSet<int>();

        foreach (string word in words)
        {
            string? normalized = NormalizeQueryWord(word);

            if (normalized is not null && _index.TryGetValue(normalized, out HashSet<int>? documentIds))
            {
                result.UnionWith(documentIds);
            }
        }

        return ToDocuments(result);
    }

    public List<(Document Document, int Matches)> SearchRanked(params string[] words)
    {
        var matches = new Dictionary<int, int>();

        foreach (string word in words.Select(NormalizeQueryWord).OfType<string>().Distinct())
        {
            if (!_index.TryGetValue(word, out HashSet<int>? documentIds))
            {
                continue;
            }

            foreach (int id in documentIds)
            {
                matches[id] = matches.GetValueOrDefault(id) + 1;
            }
        }

        return matches
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key)
            .Select(pair => (Document: _documents[pair.Key], Matches: pair.Value))
            .ToList();
    }

    private static string? NormalizeQueryWord(string word)
    {
        return TextProcessor.Normalize(word).FirstOrDefault();
    }

    private List<Document> ToDocuments(IEnumerable<int> documentIds)
    {
        return documentIds
            .OrderBy(id => id)
            .Select(id => _documents[id])
            .ToList();
    }
}
