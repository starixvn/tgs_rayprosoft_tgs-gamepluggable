using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyPlayerGameUrlException : Exception
{
    public VerifyPlayerGameUrlException(string playerCode, string gameProviderCode, string gameCode) : base($"玩家認證取得遊戲供應商網址失敗.({playerCode}|{gameProviderCode}|{gameCode})")
    {
    }
}
