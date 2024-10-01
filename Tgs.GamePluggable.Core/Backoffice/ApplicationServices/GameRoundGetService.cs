using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameBets.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundGetService(
	TimeProvider timeProvider,
	IGameBetsClient client) : IGetService<GameRoundQuery, GameRoundResult>
{
	public async ValueTask<GameRoundResult> GetAsync(GameRoundQuery query, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var info = await client.GameBetMajorGetAsync(
			externalCreatedOn: currentTime.AddMonths(-6),
			id: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");

		var status = info.BetState switch
		{
			GameBets.Common.Enums.BetState.Waiting => Enums.RoundStatus.New,
			GameBets.Common.Enums.BetState.Running => Enums.RoundStatus.Open,
			_ => Enums.RoundStatus.Closed
		};

		var clientStatus = info.BetState switch
		{
			GameBets.Common.Enums.BetState.Void => Enums.RoundClientStatus.Void,
			GameBets.Common.Enums.BetState.Refund => Enums.RoundClientStatus.Void,
			GameBets.Common.Enums.BetState.Cancel => Enums.RoundClientStatus.Void,
			GameBets.Common.Enums.BetState.Loss => Enums.RoundClientStatus.Loss,
			GameBets.Common.Enums.BetState.Win => Enums.RoundClientStatus.Win,
			GameBets.Common.Enums.BetState.Draw => Enums.RoundClientStatus.Draw,
			GameBets.Common.Enums.BetState.HalfWin => Enums.RoundClientStatus.Win,
			GameBets.Common.Enums.BetState.HalfLoss => Enums.RoundClientStatus.Loss,
			_ => Enums.RoundClientStatus.Open,
		};

		return new GameRoundResult(
			RoundId: info.Id,
			ExternalRoundId: info.ExternalRoundId,
			GameProviderId: info.GameProviderId,
			PlayerCode: info.PlayerCode,
			GameId: info.GameId,
			CurrencyCode: info.CurrencyCode,
			Status: status,
			GameProviderConfigurationId: info.GameProviderConfigurationId,
			WalletId: info.WalletId,
			CreatedOn: info.CreatedOn,
			ClosedOn: info.ClosedOn,
			UpdatedOn: info.UpdatedOn,
			BrandId: info.BrandId,
			ExternalCreatedOn: info.ExternalCreatedOn,
			ExternalClosedOn: info.ExternalClosedOn,
			BetAmount: info.Bet,
			ValidBetAmount: info.ValidBet,
			SettleAmount: info.Settle,
			Commission: info.Commission,
			Turnover: info.Turnover,
			WinLoss: -info.WinLoss,
			TieBetAdjustment: 0.0m,
			ClientStatus: clientStatus);
	}
}
