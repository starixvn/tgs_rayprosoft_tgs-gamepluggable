using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class GameRoundAddCommandService(
    IGame game,
    IGameProvider gameProvider,
    IGameRecordHandleClient gameRecordHandleClient) : ICommandService<GameRoundAddCommand>
{
    public async ValueTask ExecuteAsync(GameRoundAddCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = command.ClientRoundStatus switch
        {
            GameRoundClientStatus.Win => WagerStates.Win,
            GameRoundClientStatus.Loss => WagerStates.Lose,
            GameRoundClientStatus.Void => WagerStates.Void,
            GameRoundClientStatus.Draw => WagerStates.Draw,
            _ => WagerStates.Running
        };

        var gameProviderInfo = await gameProvider.GetAsync(
            gameProviderId: command.GameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameInfo = await game.GetAsync(
            gameId: command.GameId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await gameRecordHandleClient.MessageAsync(
            gameProviderCode: gameProviderInfo.GameProviderCode,
            playerCode: command.PlayerCode,
            wagerState: wagerState,
            externalRoundId: command.ExternalRoundId,
            externalWagerId: command.ExternalRoundId,
            externalGameId: gameInfo.GameExternalId,
            externalCreatedOn: command.ExternalCreatedOn,
            externalClosedOn: command.ExternalClosedOn,
            bet: command.BetAmount,
            validBet: command.ValidBetAmount,
            settle: command.SettleAmount,
            commission: command.Commission,
            turnover: command.Turnover,
            winLoss: -command.CompanyWinLoss,
            additionalData: JsonNode.Parse("{}")!,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
