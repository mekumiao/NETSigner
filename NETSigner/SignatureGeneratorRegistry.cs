namespace NETSigner;

/// <summary>
/// 签名生成器注册表
/// </summary>
public class SignatureGeneratorRegistry
{
    private static readonly ISignatureGenerator _signatureGenerator = new HmacSHA256SignatureGenerator();

    private IDictionary<string, ISignatureGenerator> _dictGenerators;

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

    public SignatureGeneratorRegistry(IEnumerable<ISignatureGenerator> generators)
    {
        _dictGenerators = generators.DistinctBy(x => x.Name).ToDictionary(x => x.Name);
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
