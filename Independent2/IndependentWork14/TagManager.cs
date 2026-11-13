namespace IndependentWork14;

public class TagManager
{
    private readonly Dictionary<string, HashSet<string>> _articleTags = new();

    public void AddTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _articleTags.Add(articleId, tags);
        }

        tags.Add(tag.Trim());
    }

    public void RemoveTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            return;
        }

        tags.Remove(tag);

        if (tags.Count == 0)
        {
            _articleTags.Remove(articleId);
        }
    }

    public HashSet<string> GetTags(string articleId)
    {
        return _articleTags.TryGetValue(articleId, out HashSet<string>? tags)
            ? new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public HashSet<string> GetAllUniqueTags()
    {
        var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (HashSet<string> tags in _articleTags.Values)
        {
            allTags.UnionWith(tags);
        }

        return allTags;
    }

    public HashSet<string> FindCommonTags(string articleId1, string articleId2)
    {
        HashSet<string> common = GetTags(articleId1);
        common.IntersectWith(GetTags(articleId2));
        return common;
    }

    public HashSet<string> FindTagsOnlyIn(string articleId, string otherArticleId)
    {
        HashSet<string> difference = GetTags(articleId);
        difference.ExceptWith(GetTags(otherArticleId));
        return difference;
    }

    public List<string> FindArticlesByTag(string tag)
    {
        return _articleTags
            .Where(pair => pair.Value.Contains(tag))
            .Select(pair => pair.Key)
            .ToList();
    }
}
