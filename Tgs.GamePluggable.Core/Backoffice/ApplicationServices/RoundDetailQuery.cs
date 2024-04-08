using System;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record RoundDetailQuery
{
	public Guid RoundId { get; init; }

	public string Language { get; init; } = default!;
}
