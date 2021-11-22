using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.Pluggable.ViewModels
{
    public class GameRoundClosedViewModel
    {
        public decimal BetAmount { get; init; }

        public GameRoundClosedClientRoundStatus ClientRoundStatus { get; init; }

        public decimal Commission { get; init; }

        public DateTimeOffset ExternalClosedOn { get; init; }

        public Guid OperatorId { get; init; }

        public string Remark { get; init; } = default!;

        public decimal SettleAmount { get; init; }

        public decimal Turnover { get; init; }

        public decimal ValidBetAmount { get; init; }

        public decimal WinLoss { get; init; }
    }
}
