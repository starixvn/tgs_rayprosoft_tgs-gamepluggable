using System;

namespace Tgs.GamePluggable.Core.Exceptions;

public class VerifyBackofficePlayerFundInException(
	string brandCode,
	string gameProviderCode,
	string currencyCode) : Exception($"後臺玩家資金轉入主錢包驗證失敗.({brandCode}|{gameProviderCode}|{currencyCode})")
{
}
