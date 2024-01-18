using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyPlayerGameUrlException(
    string brandCode,
    string playerCode,
    string username,
    string gameProviderCode,
    string gameCode) : Exception($"玩家遊戲商網址驗證失敗.({brandCode}|{playerCode}|{username}|{gameProviderCode}|{gameCode})")
{
}
