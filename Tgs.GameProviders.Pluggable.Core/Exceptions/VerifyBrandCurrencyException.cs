using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyBrandCurrencyException(Guid brandId, Guid gameProviderId, string currencyCode) : Exception($"平台幣別認證遊戲供應商失敗.({brandId}|{gameProviderId}|{currencyCode})")
{
}
