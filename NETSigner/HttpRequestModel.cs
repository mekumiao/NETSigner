namespace NETSigner;

public class HttpRequestModel
{
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string? ContentMD5 { get; set; }
    public IDictionary<string, string?>? Query { get; set; }
    public IDictionary<string, string?>? Form { get; set; }
    public IDictionary<string, string?> Headers = new Dictionary<string, string?>();
}
