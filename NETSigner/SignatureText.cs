using System.Text;

namespace NETSigner;

public class SignatureText
{
    public bool IsSuccess => ErrorMessage == string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string HTTPMethod { get; set; } = string.Empty;
    public string? Accept { get; set; }
    public string? ContentMD5 { get; set; }
    public string? ContentType { get; set; }
    public string? Date { get; set; }
    public IDictionary<string, string?>? Headers { get; set; }
    public IDictionary<string, string?>? Query { get; set; }
    public IDictionary<string, string?>? Form { get; set; }
    public HashSet<string>? SignatureHeaders { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(HTTPMethod);
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
        if (Headers?.Any() == true)
        {
            foreach (var item in Headers.OrderBy(x => x.Key))
            {
                builder.AppendFormat("{0}:{1}", item.Key, item.Value);
                builder.Append('\n');
            }
        }
        builder.Append(Path);
        if (Query?.Any() == true)
        {
            var parameter = new Dictionary<string, string?>(Query);
            if (Form?.Any() == true)
            {
                foreach (var item in Form)
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
        return builder.ToString();
    }
}
