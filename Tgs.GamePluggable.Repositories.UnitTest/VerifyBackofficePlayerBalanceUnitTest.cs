using NSubstitute;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Exceptions;

namespace Tgs.GamePluggable.Repositories.UnitTest;

public class VerifyBackofficePlayerBalanceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var transferWallet = true;
		var currencyCode = "CNY";

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(brandId),
			currencyCode: Arg.Is(currencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository
			.GetCurrencyExistAsync(
			id: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(currencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		await sut.VerifyBackofficePlayerBalanceAsync(
			brandId: brandId,
			brandCode: brandCode,
			gameProviderId: gameProviderId,
			gameProviderCode: gameProviderCode,
			transferWallet: transferWallet,
			currencyCode: currencyCode);
	}

	[Fact]
	public async Task TransferWallet_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var transferWallet = false;
		var currencyCode = "CNY";

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyBackofficePlayerBalanceException>(
			() => sut.VerifyBackofficePlayerBalanceAsync(
				brandId: brandId,
				brandCode: brandCode,
				gameProviderId: gameProviderId,
				gameProviderCode: gameProviderCode,
				transferWallet: transferWallet,
				currencyCode: currencyCode)
			.AsTask());
	}

	[Fact]
	public async Task GameProvider_Currency_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var transferWallet = true;
		var currencyCode = "CNY";

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(brandId),
			currencyCode: Arg.Is(currencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		_ = fakeGameProviderRepository
			.GetCurrencyExistAsync(
			id: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(currencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyBackofficePlayerBalanceException>(
			() => sut.VerifyBackofficePlayerBalanceAsync(
				brandId: brandId,
				brandCode: brandCode,
				gameProviderId: gameProviderId,
				gameProviderCode: gameProviderCode,
				transferWallet: transferWallet,
				currencyCode: currencyCode)
			.AsTask());
	}

	[Fact]
	public async Task Brand_Currency_Is_False()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandRepository = Substitute.For<IBrandRepository>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

		var brandId = Guid.NewGuid();
		var brandCode = "GPI";
		var gameProviderId = Guid.NewGuid();
		var gameProviderCode = "KY";
		var transferWallet = true;
		var currencyCode = "CNY";

		_ = fakeBrandRepository.GetCurrencyExistAsync(
			brandId: Arg.Is(brandId),
			currencyCode: Arg.Is(currencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		_ = fakeGameProviderRepository
			.GetCurrencyExistAsync(
			id: Arg.Is(gameProviderId),
			currencyCode: Arg.Is(currencyCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new VerificationRepository(
			fakePlayerRepository,
			fakeBrandRepository,
			fakeGameProviderRepository);

		_ = await Assert.ThrowsAsync<VerifyBackofficePlayerBalanceException>(
			() => sut.VerifyBackofficePlayerBalanceAsync(
				brandId: brandId,
				brandCode: brandCode,
				gameProviderId: gameProviderId,
				gameProviderCode: gameProviderCode,
				transferWallet: transferWallet,
				currencyCode: currencyCode)
			.AsTask());
	}
}
