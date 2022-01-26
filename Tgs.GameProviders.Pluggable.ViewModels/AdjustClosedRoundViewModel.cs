using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.Pluggable.ViewModels
{
    public class AdjustClosedRoundViewModel
    {
        public decimal? AdjustAmount { get; init; }

        [Required]
        public decimal BetAmount { get; init; }

        [Required]
        public GameRoundClosedClientRoundStatus ClientRoundStatus { get; init; }

        [Required]
        public decimal Commission { get; init; }

        [Required]
        public DateTimeOffset ExternalClosedOn { get; init; }

        [Required]
        public Guid OperatorId { get; init; }

        public string Remark { get; init; } = default!;

        [Required]
        public decimal SettleAmount { get; init; }

        [Required]
        public decimal Turnover { get; init; }

        [Required]
        public decimal ValidBetAmount { get; init; }

        [Required]
        public decimal WinLoss { get; init; }
    }
}