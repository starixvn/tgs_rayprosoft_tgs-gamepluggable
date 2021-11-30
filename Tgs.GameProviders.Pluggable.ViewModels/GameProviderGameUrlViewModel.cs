using System;

namespace Tgs.GameProviders.Pluggable.ViewModels
{
    public record GameProviderGameUrlViewModel(
        string SubGameProviderCode,
        string PlayerCode,
        Guid LoginRecordId,
        bool IsMobile,
        int GameId,
        string Language,
        string IpAddress,
        Uri BackUrl);
}
