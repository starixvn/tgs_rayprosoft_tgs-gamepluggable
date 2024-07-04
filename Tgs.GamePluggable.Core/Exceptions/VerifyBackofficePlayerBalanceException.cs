using System;

namespace Tgs.GamePluggable.Core.Exceptions;

public class VerifyBackofficePlayerBalanceException(
	string brandCode,
	string gameProviderCode,
	string currencyCode) : Exception($"後臺玩家餘額驗證失敗.({brandCode}|{gameProviderCode}|{currencyCode})")
{
}
