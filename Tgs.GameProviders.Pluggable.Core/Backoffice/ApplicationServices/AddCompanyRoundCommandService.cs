using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class AddCompanyRoundCommandService : ICommandService<AddCompanyRoundCommand>
    {
        private readonly IBrandService _brandService;
        private readonly IGameRoundsClient _gameRoundsClient;

        public AddCompanyRoundCommandService(
            IGameRoundsClient gameRoundsClient,
            IBrandService brandService)
        {
            _gameRoundsClient = gameRoundsClient ?? throw new ArgumentNullException(nameof(gameRoundsClient));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
        }

        public async ValueTask ExecuteAsync(AddCompanyRoundCommand command, CancellationToken cancellationToken = default)
        {
            var brandGameProviderInfo = await _brandService
                .GetBrandGameProviderConfigurationAsync(command.BrandId, command.GameProviderId)
                .ConfigureAwait(false);

            _ = await _gameRoundsClient.AddCompanyRoundAsync(
                new GameRounds.Common.ViewModels.AddCompanyRoundViewModel
                {
                    BrandId = command.BrandId,
                    PlayerCode = command.PlayerCode,
                    WalletId = command.WalletId,
                    GameId = command.GameId,
                    GameProviderId = command.GameProviderId,
                    GameProviderConfigurationId = brandGameProviderInfo.GameProviderConfigurationId,
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
                }, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
