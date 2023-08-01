using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyPlayerBalanceException : Exception
{
    public VerifyPlayerBalanceException(string playerCode, string username, string gameProviderCode) : base($"玩家點數認證取得遊戲供應商失敗.({playerCode}|{username}|{gameProviderCode}|)")
    {
    }
}
