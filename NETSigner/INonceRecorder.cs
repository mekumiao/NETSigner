namespace NETSigner;

/// <summary>
/// Nonce记录器接口
/// </summary>
public interface INonceRecorder
{
    /// <summary>
    /// 记录nonce值
    /// </summary>
    /// <param name="nonce"></param>
    /// <param name="delay">延迟时间</param>
    /// <returns>返回 true 说明写入新值成功。反之说明列表中已有值，导致写入失败，返回false</returns>
    bool Record(string nonce, TimeSpan delay);
}
