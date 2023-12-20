using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyPlayerBalanceException(
    string brandCode,
    string playerCode,
    string username,
    string gameProviderCode) : Exception($"玩家點數驗證失敗.({brandCode}|{playerCode}|{username}|{gameProviderCode}|)")
{
}
