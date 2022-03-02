namespace NETSigner;

/// <summary>
/// 签名生成器注册表
/// </summary>
public class SignatureGeneratorRegistry
{
    private static readonly ISignatureGenerator _signatureGenerator = new HmacSHA256SignatureGenerator();

    private readonly IDictionary<string, ISignatureGenerator> _dictGenerators;

    public SignatureGeneratorRegistry()
    {
        var hmacSHA1Signature = new HmacSHA1SignatureGenerator();
        var hmacSHA256Signature = new HmacSHA256SignatureGenerator();
        _dictGenerators = new Dictionary<string, ISignatureGenerator>
        {
            { hmacSHA1Signature.Name, hmacSHA1Signature },
            { hmacSHA256Signature.Name, hmacSHA256Signature },
        };
    }

    private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
    {
        using IEnumerator<TSource> enumerator = source.GetEnumerator();

        if (enumerator.MoveNext())
        {
            var set = new HashSet<TKey>(7, comparer);
            do
            {
                TSource element = enumerator.Current;
                if (set.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
            while (enumerator.MoveNext());
        }
    }

    public SignatureGeneratorRegistry(IEnumerable<ISignatureGenerator> generators)
    {
        _dictGenerators = DistinctByIterator(generators, x => x.Name, null).ToDictionary(x => x.Name);
    }

    public ISignatureGenerator GetGenerator(string? name)
    {
        if (name != null && _dictGenerators.TryGetValue(name, out var value))
        {
            return value;
        }
        return _signatureGenerator;
    }

    public bool Register(string name, ISignatureGenerator signatureGenerator)
    {
        return _dictGenerators.TryAdd(name, signatureGenerator);
    }
}
