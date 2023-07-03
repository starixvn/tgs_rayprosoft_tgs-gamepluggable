using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class AddCompanyRoundCommandService : ICommandService<AddCompanyRoundCommand>
{
    private readonly IBrand _brand;
    private readonly IGameRoundsClient _gameRoundsClient;

    public AddCompanyRoundCommandService(
        IGameRoundsClient gameRoundsClient,
        IBrand brand)
    {
        _gameRoundsClient = gameRoundsClient ?? throw new ArgumentNullException(nameof(gameRoundsClient));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
    }

    public async ValueTask ExecuteAsync(AddCompanyRoundCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderConfigurationId = await _brand
            .GetGameProviderConfigurationIdAsync(
                command.BrandId,
                command.GameProviderId,
                cancellationToken)
            .ConfigureAwait(false);

        var addCompanyRound = new AddCompanyRoundViewModel
        {
            BrandId = command.BrandId,
            PlayerCode = command.PlayerCode,
            WalletId = command.WalletId,
            GameId = command.GameId,
            GameProviderId = command.GameProviderId,
            GameProviderConfigurationId = gameProviderConfigurationId,
            ExternalRoundId = command.ExternalRoundId,
            ClientStatus = (GameRoundClientStatus)command.ClientRoundStatus,
            ExternalCreatedOn = command.ExternalCreatedOn,
            ExternalClosedOn = command.ExternalClosedOn,
            CurrencyCode = command.CurrencyCode,
            BetAmount = command.BetAmount,
            SettleAmount = command.SettleAmount,
            ValidBetAmount = command.ValidBetAmount,
            Turnover = command.Turnover,
            Commission = command.Commission,
            CompanyWinLoss = command.CompanyWinLoss,
            OperatorId = command.OperatorId,
            Remark = command.Remark
        };
        await _gameRoundsClient
            .AddCompanyRoundAsync(
            source: addCompanyRound,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
