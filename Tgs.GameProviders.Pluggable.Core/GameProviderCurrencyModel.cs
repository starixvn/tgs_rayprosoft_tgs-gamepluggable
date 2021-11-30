using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameProviderCurrencyModel(
        Guid GameProviderId,
        string CurrencyCode,
        string GameProviderCurrencyCode
        );
}
