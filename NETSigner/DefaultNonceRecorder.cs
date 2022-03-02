using System.Collections.Concurrent;

namespace NETSigner;

public class DefaultNonceRecorder : INonceRecorder
{
    private readonly ConcurrentDictionary<string, byte> _cache = new();

    public bool Record(string nonce, TimeSpan delay)
    {
        return _cache.TryAdd(nonce, 0);
    }
}
