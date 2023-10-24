using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core.Exceptions;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

public class VerificationRepositoryVerifyBrandCurrencyUnitTest
{
    [Fact]
    public async Task Brand_Currency_Is_Empty()
    {
        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        string currencyCode = "CNY";

        var fakeClinet = Substitute.For<IGameCommandClient>();

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(brandId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(string.Empty);

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyBrandCurrencyException>(
            () => repository.VerifyBrandCurrencyAsync(
                brandId: brandId,
                gameProviderId: gameProviderId,
                currencyCode: currencyCode)
            .AsTask());
    }

    [Fact]
    public async Task Brand_Currency_Is_Null()
    {
        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        string currencyCode = "CNY";

        var fakeClinet = Substitute.For<IGameCommandClient>();

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(brandId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((string?)null);

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyBrandCurrencyException>(
            () => repository.VerifyBrandCurrencyAsync(
                brandId: brandId,
                gameProviderId: gameProviderId,
                currencyCode: currencyCode)
             .AsTask());
    }

    [Fact]
    public async Task GameProvider_Currency_Is_Empty()
    {
        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        string currencyCode = "CNY";

        var fakeClinet = Substitute.For<IGameCommandClient>();

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(brandId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(string.Empty);

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyBrandCurrencyException>(
            () => repository.VerifyBrandCurrencyAsync(
                brandId: brandId,
                gameProviderId: gameProviderId,
                currencyCode: currencyCode)
             .AsTask());
    }

    [Fact]
    public async Task GameProvider_Currency_Is_Null()
    {
        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        string currencyCode = "CNY";

        var fakeClinet = Substitute.For<IGameCommandClient>();

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(brandId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((string?)null);

        var repository = new VerificationRepository(
            fakeClinet);

        _ = await Assert.ThrowsAsync<VerifyBrandCurrencyException>(
            () => repository.VerifyBrandCurrencyAsync(
                brandId: brandId,
                gameProviderId: gameProviderId,
                currencyCode: currencyCode)
            .AsTask());
    }

    [Fact]
    public async Task Normal_Process()
    {
        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        string currencyCode = "CNY";

        var fakeClinet = Substitute.For<IGameCommandClient>();

        _ = fakeClinet
            .BrandGetStatusAsync(
                brandId: Arg.Is(brandId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(BrandStatus.Active);

        _ = fakeClinet
            .GameProviderGetStatusAsync(
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GameProviderStatus.Active);

        _ = fakeClinet
            .BrandGetCurrencyAsync(
                brandId: Arg.Is(brandId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        _ = fakeClinet
            .GameProviderGetCurrencyCodeAsync(
                gameProviderId: Arg.Is(gameProviderId),
                currencyCode: Arg.Is(currencyCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns("CNY");

        var repository = new VerificationRepository(
            fakeClinet);

        await repository.VerifyBrandCurrencyAsync(
            brandId: brandId,
            gameProviderId: gameProviderId,
            currencyCode: currencyCode);
    }
}
