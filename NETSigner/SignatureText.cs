using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NETSigner;

public class SignatureText
{
    private static SignatureHeaders _signatureHeaders = new();
    public string Path { get; private set; }
    public string Method { get; private set; }
    public string? Accept { get; private set; }
    public string? ContentMD5 { get; private set; }
    public string? ContentType { get; private set; }
    public string? Date { get; private set; }
    public IDictionary<string, string?>? Querys { get; set; }
    public IDictionary<string, string?>? Forms { get; set; }
    private readonly IDictionary<string, string?> _headers;

    protected SignatureText(string path, string method, IDictionary<string, string?> headers)
    {
        Path = path;
        Method = method;
        _headers = headers;
    }

    private static IDictionary<string, string?> Intersect(IDictionary<string, string?> headers, SignatureHeaders signatureHeaders)
    {
        return headers.Join(signatureHeaders, t1 => t1.Key, t2 => t2, (t1, t2) => t1, StringComparer.OrdinalIgnoreCase)
                      .ToDictionary(x => x.Key, y => y.Value);
    }

    public static bool TryParse(string path, string method, IDictionary<string, string?> headers, VerifyResult result, [NotNullWhen(true)] out SignatureText? signatureText)
    {
        signatureText = null;

        if (!headers.ContainsKey(SignatureConstant.XCaKey))
        {
            result.ErrorMessage = $"Missing parameter, The {SignatureConstant.XCaKey} is required";
            return false;
        }

        if (!headers.ContainsKey(SignatureConstant.XCaTimestamp))
        {
            result.ErrorMessage = $"Missing parameter, The {SignatureConstant.XCaTimestamp} is required";
            return false;
        }

        if (!headers.ContainsKey(SignatureConstant.XCaNonce))
        {
            result.ErrorMessage = $"Missing parameter, The {SignatureConstant.XCaNonce} is required";
            return false;
        }

        if (headers.TryGetValue(SignatureConstant.XCaSignatureHeaders, out var signatureHeaders) && !string.IsNullOrWhiteSpace(signatureHeaders))
        {
            signatureText = new SignatureText(path, method, Intersect(headers, SignatureHeaders.Parse(signatureHeaders)));
        }
        else
        {
            signatureText = new SignatureText(path, method, Intersect(headers, _signatureHeaders));
        }

        if (headers.TryGetValue(SignatureConstant.ContentMD5, out var md5))
        {
            signatureText.ContentMD5 = md5;
        }

        if (headers.TryGetValue(SignatureConstant.ContentType, out var type))
        {
            signatureText.ContentType = type;
        }

        if (headers.TryGetValue(SignatureConstant.Date, out var date))
        {
            signatureText.Date = date;
        }

        if (headers.TryGetValue(SignatureConstant.Accept, out var accept))
        {
            signatureText.Accept = accept;
        }

        return true;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(Method);
        builder.Append('\n');
        builder.Append(Accept);
        builder.Append('\n');
        if (!string.IsNullOrWhiteSpace(ContentMD5))
        {
            builder.Append(ContentMD5);
            builder.Append('\n');
        }
        builder.Append(ContentType);
        builder.Append('\n');
        builder.Append(Date);
        builder.Append('\n');
        if (_headers.Any())
        {
            foreach (var item in _headers.OrderBy(x => x.Key))
            {
                builder.AppendFormat("{0}:{1}", item.Key, item.Value);
                builder.Append('\n');
            }
        }
        builder.Append(Path);
        if (Querys?.Any() == true)
        {
            var parameter = new Dictionary<string, string?>(Querys);
            if (Forms?.Any() == true)
            {
                foreach (var item in Forms)
                {
                    parameter.TryAdd(item.Key, item.Value);
                }
            }
            builder.Append('?');
            foreach (var item in parameter.OrderBy(x => x.Key))
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    builder.AppendFormat("{0}&", item.Key);
                }
                else
                {
                    builder.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
            }
            builder.Remove(builder.Length - 1, 1);
        }
        builder.Append('\n');
        return builder.ToString();
    }
}
