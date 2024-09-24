using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace Tgs.GameProviders.Pluggable.Client;

public interface IGameProvidersBackofficePluggableClient
{
	ValueTask<Uri?> TicketDetailUrlAsync(TicketDetailViewModel viewModel, CancellationToken cancellationToken = default);

	ValueTask AddCompanyRoundAsync(AddCompanyRoundViewModel viewModel, CancellationToken cancellationToken = default);

	ValueTask AdjustClosedRoundAsync(Guid RoundId, AdjustClosedRoundViewModel viewModel, CancellationToken cancellationToken = default);

	ValueTask FundInAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameActionViewModel> GameActionQueryAsync(Guid roundId, CancellationToken cancellationToken = default);

	ValueTask GameRoundClosedAsync(Guid RoundId, GameRoundClosedViewModel viewModel, CancellationToken cancellationToken = default);

	ValueTask<JsonDocument?> GameRoundDetailGetAsync(Guid roundId, string language, CancellationToken cancellationToken = default);

	ValueTask<GameRoundViewModel?> GameRoundGetAsync(Guid roundId, CancellationToken cancellationToken = default);

	ValueTask<decimal> GetBalanceAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceInfoViewModel> GetBalanceInfoAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default);

	Task KickPlayerAsync(string playerCode, CancellationToken cancellationToken = default);

	Task KickPlayerAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default);
}
