using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.Pluggable.ViewModels
{
    public class ClosedGameRoundViewModel
    {
        [Required]
        public decimal BetAmount { get; init; }

        [Required]
        [Range(2, 5, ErrorMessage = "Value for {0} must be between {1} and {2}(2:Void 3:Win 4:Loss 5:Draw).")]
        public int ClientRoundStatus { get; init; }

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
