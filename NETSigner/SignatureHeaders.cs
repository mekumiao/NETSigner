namespace NETSigner;

public class SignatureHeaders : HashSet<string>
{
    private static IEnumerable<string> _except = new[]
    {
        SignatureConstant.XCaSignature,
        SignatureConstant.XCaSignatureHeaders,
        SignatureConstant.Accept,
        SignatureConstant.ContentMD5,
        SignatureConstant.ContentType,
        SignatureConstant.Date,
    };

    public SignatureHeaders()
    {
        base.Add(SignatureConstant.XCaKey);
        base.Add(SignatureConstant.XCaTimestamp);
        base.Add(SignatureConstant.XCaNonce);
    }

    public SignatureHeaders(IEnumerable<string> collection) : base(collection)
    {
        base.Add(SignatureConstant.XCaKey);
        base.Add(SignatureConstant.XCaTimestamp);
        base.Add(SignatureConstant.XCaNonce);
    }

    public new bool Add(string item)
    {
        if (!_except.Contains(item))
        {
            return base.Add(item);
        }
        return false;
    }

    public static SignatureHeaders Parse(string value)
    {
        _ = value ?? throw new ArgumentNullException(nameof(value));

        var set = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Except(_except)
                       .ToHashSet(StringComparer.OrdinalIgnoreCase);
        return new SignatureHeaders(set);
    }

    public override string ToString() => string.Join(',', this);
}
