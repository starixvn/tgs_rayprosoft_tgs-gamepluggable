using System;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameProviders.Pluggable.ViewModels
{
    public record GameProviderGameUrlViewModel
    {
        [Required]
        public string PlayerCode { get; init; } = default!;
        [Required]
        public Guid LoginRecordId { get; init; }
        [Required]
        public bool IsMobile { get; init; }
        [Required]
        public int GameId { get; init; }
        [Required]
        public string Language { get; init; } = default!;
        [Required]
        public string IpAddress { get; init; } = default!;
        public Uri? BackUrl { get; init; }
    }
}
