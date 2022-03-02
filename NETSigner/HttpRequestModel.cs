namespace NETSigner;

public class HttpRequestModel
{
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public IDictionary<string, string?> Headers { get; }
    public IDictionary<string, string?>? Query { get; set; }
    public IDictionary<string, string?>? Form { get; set; }

    public HttpRequestModel(IDictionary<string, string?> headers)
    {
        Headers = headers;
    }
}
