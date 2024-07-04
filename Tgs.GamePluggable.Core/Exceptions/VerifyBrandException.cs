using System;

namespace Tgs.GamePluggable.Core.Exceptions;

public class VerifyBrandException(
	string brandCode,
	string gameProviderCode,
	string currencyCode) : Exception($"平台驗證失敗.({brandCode}|{gameProviderCode}|{currencyCode})")
{
}
