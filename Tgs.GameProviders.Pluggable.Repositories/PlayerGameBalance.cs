using Tgs.GameCurrency.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Core;

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

    public async ValueTask<decimal> GetBalanceAsync(
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

        var result = await _gameCurrencyClient
            .ConvertPlatformBalanceAsync(
                gameProviderCode: gameProviderCode,
                currencyCode: currencyCode,
                balance: response.FreeBalance,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result.Balance;
    }
}
