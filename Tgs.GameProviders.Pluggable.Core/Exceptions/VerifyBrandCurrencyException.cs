using System;

namespace Tgs.GameProviders.Pluggable.Core.Exceptions;

public class VerifyBrandCurrencyException : Exception
{
    public VerifyBrandCurrencyException(Guid brandId, Guid gameProviderId, string currencyCode) : base($"平台幣別認證遊戲供應商失敗.({brandId}-{gameProviderId}-{currencyCode})")
    {
    }
}
