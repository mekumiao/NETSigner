namespace NETSigner;

/// <summary>
/// 签名生成器接口
/// </summary>
public interface ISignatureGenerator
{
    string Name { get; }

    string Signature(string sk, string plainText);
}
