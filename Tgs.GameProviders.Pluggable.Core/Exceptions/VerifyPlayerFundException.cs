using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyPlayerFundException(
    string brandCode,
    string playerCode,
    string username,
    string gameProviderCode) : Exception($"玩家轉入/轉出驗證失敗.({brandCode}|{playerCode}|{username}|{gameProviderCode})")
{
}
