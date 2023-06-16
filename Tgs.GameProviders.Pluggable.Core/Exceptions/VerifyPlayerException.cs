using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyPlayerException : Exception
{
    public VerifyPlayerException(string playerCode, string gameProviderCode) : base($"玩家認證取得遊戲供應商失敗.({playerCode}-{gameProviderCode})")
    {
    }
}
