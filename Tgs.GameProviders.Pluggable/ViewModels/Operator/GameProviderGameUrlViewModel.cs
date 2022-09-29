using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.Pluggable.ViewModels.Operator;

public record GameProviderGameUrlViewModel
{
    public Uri? BackUrl { get; init; }

    [Required]
    public int GameId { get; init; }

    [Required]
    public string IpAddress { get; init; } = default!;

    [Required]
    public bool IsMobile { get; init; }

    [Required]
    public string Language { get; init; } = default!;

    [Required]
    public Guid LoginRecordId { get; init; }

    [Required]
    public string PlayerCode { get; init; } = default!;
}
