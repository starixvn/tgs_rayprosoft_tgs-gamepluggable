using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.Pluggable.ViewModels
{
    public record GameProviderFundViewModel
    {
        [Required]
        [Range(minimum: 0, maximum: (double)decimal.MaxValue)]
        public decimal Amount { get; init; }
    }
}
