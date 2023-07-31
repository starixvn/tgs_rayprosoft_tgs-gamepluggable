using Tgs.GameCurrency.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Models;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class PlayerGameBalance : IPlayerGameBalance
{
    private readonly IGameCurrencyClient _gameCurrencyClient;
    private readonly IGameProvidersClient _gameProvidersClient;

    public PlayerGameBalance(
        IGameProvidersClient gameProvidersClient,
        IGameCurrencyClient gameCurrencyClient)
    {
        _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
        _gameCurrencyClient = gameCurrencyClient ?? throw new ArgumentNullException(nameof(gameCurrencyClient));
    }

    public async ValueTask<PlatformBalanceInfo> GetBalanceAsync(
        string gameProviderCode,
        string playerCode,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        var response = await _gameProvidersClient
            .PlayerBalanceAsync(
            gameProviderCode: gameProviderCode,
            playerCode: playerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (response.Balance == 0)
        {
            return new PlatformBalanceInfo(
                Balance: 0.0m,
                FreeBalance: 0.0m,
                OutstandingBalance: 0.0m);

        }

        var result = await _gameCurrencyClient
            .ConvertPlatformBalanceAsync(
            gameProviderCode: gameProviderCode,
            currencyCode: currencyCode,
            balance: response.Balance,
            freeBalance: response.FreeBalance,
            outstandingBalance: response.OutstandingBalance,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new PlatformBalanceInfo(
            Balance: result.Balance,
            FreeBalance: result.FreeBalance,
            OutstandingBalance: result.OutstandingBalance);
    }
}
