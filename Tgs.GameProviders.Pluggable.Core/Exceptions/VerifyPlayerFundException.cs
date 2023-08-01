using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyPlayerFundException : Exception
{
    public VerifyPlayerFundException(
        string brandCode,
        string playerCode,
        string username,
        string gameProviderCode)
        : base($"玩家轉入/轉出驗證失敗.({brandCode}|{playerCode}|{username}|{gameProviderCode})")
    {
    }
}
