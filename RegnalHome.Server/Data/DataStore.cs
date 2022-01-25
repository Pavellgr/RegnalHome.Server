using System.Collections.Concurrent;

namespace RegnalHome.Server.Data;

public class DataStore
{
    private readonly ConcurrentDictionary<string, object> _data = new();

    public object? TryGetValue(string key)
    {
        _data.TryGetValue(key, out var result);

        return result;
    }

    public void AddUpdate(string key, object data)
    {
        _data.AddOrUpdate(key, data, (_, _) => data);
    }
}