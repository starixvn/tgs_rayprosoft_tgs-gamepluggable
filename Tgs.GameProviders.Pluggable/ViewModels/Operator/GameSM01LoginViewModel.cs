using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.Pluggable.ViewModels.Operator;

public record GameSM01LoginViewModel
{
    [Required]
    public string PlayerCode { get; init; } = default!;
}
