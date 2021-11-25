using System;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices
{
    public record GameRoundClosedCommand
    {
        public Guid RoundId { get; init; }
        public decimal BetAmount { get; init; }
        public decimal ValidBetAmount { get; init; }
        public decimal SettleAmount { get; init; }
        public decimal Commission { get; init; }
        public decimal Turnover { get; init; }
        public decimal WinLoss { get; init; }
        public ClientRoundStatus ClientRoundStatus { get; init; }
        public DateTimeOffset ExternalClosedOn { get; init; }
        public Guid OperatorId { get; init; }
        public string Remark { get; init; } = default!;
    }
}
