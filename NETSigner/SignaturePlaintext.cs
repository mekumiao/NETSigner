using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NETSigner;

public class SignaturePlaintext
{
    private static SignatureHeaders _signatureHeaders = new();
    public long Timestamp { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string Nonce { get; private set; } = string.Empty;
    public string? SignatureMethod { get; private set; }
    public string Path { get; private set; }
    public string Method { get; private set; }
    public string? Accept { get; private set; }
    public string? ContentMD5 { get; private set; }
    public string? ContentType { get; private set; }
    public string? Date { get; private set; }
    public IDictionary<string, string?>? Querys { get; set; }
    public IDictionary<string, string?>? Forms { get; set; }
    private readonly IDictionary<string, string?> _headers;

    protected SignaturePlaintext(string path, string method, IDictionary<string, string?> headers)
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

    public static bool TryParse(string path, string method, IDictionary<string, string?> headers, VerifyResult result, [NotNullWhen(true)] out SignaturePlaintext? plaintext)
    {
        plaintext = null;

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
            plaintext = new SignaturePlaintext(path, method, Intersect(headers, SignatureHeaders.Parse(signatureHeaders)));
        }
        else
        {
            plaintext = new SignaturePlaintext(path, method, Intersect(headers, _signatureHeaders));
        }

        if (!string.IsNullOrWhiteSpace(headers[SignatureConstant.XCaKey]))
        {
            plaintext.Key = headers[SignatureConstant.XCaKey]!;
        }
        else
        {
            result.ErrorMessage = $"Invalid parameter, The {SignatureConstant.XCaKey} is Invalid";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(headers[SignatureConstant.XCaNonce]))
        {
            plaintext.Nonce = headers[SignatureConstant.XCaNonce]!;
        }
        else
        {
            result.ErrorMessage = $"Invalid parameter, The {SignatureConstant.XCaNonce} is Invalid";
            return false;
        }

        if (long.TryParse(headers[SignatureConstant.XCaTimestamp], out var timestamp))
        {
            plaintext.Timestamp = timestamp;
        }
        else
        {
            result.ErrorMessage = $"Invalid parameter, The {SignatureConstant.XCaTimestamp} is Invalid";
            return false;
        }

        if (headers.TryGetValue(SignatureConstant.XCaSignatureMethod, out var signatureMethod))
        {
            plaintext.SignatureMethod = signatureMethod;
        }

        if (headers.TryGetValue(SignatureConstant.ContentMD5, out var md5))
        {
            plaintext.ContentMD5 = md5;
        }

        if (headers.TryGetValue(SignatureConstant.ContentType, out var type))
        {
            plaintext.ContentType = type;
        }

        if (headers.TryGetValue(SignatureConstant.Date, out var date))
        {
            plaintext.Date = date;
        }

        if (headers.TryGetValue(SignatureConstant.Accept, out var accept))
        {
            plaintext.Accept = accept;
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
