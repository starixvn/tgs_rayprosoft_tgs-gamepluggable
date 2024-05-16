using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameRounds.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundGetService(
	IGameRoundsClient client) : IGetService<GameRoundQuery, GameRoundResult>
{
	public async ValueTask<GameRoundResult> GetAsync(GameRoundQuery query, CancellationToken cancellationToken = default)
	{
		var data = await client.RoundGetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");

		return new GameRoundResult(
			RoundId: data.RoundId,
			ExternalRoundId: data.ExternalRoundId,
			GameProviderId: data.GameProviderId,
			PlayerCode: data.PlayerCode,
			GameId: data.GameId,
			CurrencyCode: data.CurrencyCode,
			Status: data.Status,
			GameProviderConfigurationId: data.GameProviderConfigurationId,
			WalletId: data.WalletId,
			CreatedOn: data.CreatedOn,
			ClosedOn: data.ClosedOn,
			UpdatedOn: data.UpdatedOn,
			BrandId: data.BrandId,
			ExternalCreatedOn: data.ExternalCreatedOn,
			ExternalClosedOn: data.ExternalClosedOn,
			BetAmount: data.BetAmount,
			ValidBetAmount: data.ValidBetAmount,
			SettleAmount: data.SettleAmount,
			Commission: data.Commission,
			Turnover: data.Turnover,
			WinLoss: data.CompanyWinLoss,
			TieBetAdjustment: 0.0m,
			ClientStatus: data.ClientStatus);
	}
}
