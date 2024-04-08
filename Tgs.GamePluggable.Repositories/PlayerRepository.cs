using Microsoft.EntityFrameworkCore;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Models;
using Tgs.GamePluggable.Entities;

namespace Tgs.GamePluggable.Repositories;

internal class PlayerRepository(
	TgsContext context) : IPlayerRepository
{
	public async ValueTask<PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await context.Players
		.Where(
			player => player.PlayerCode == playerCode)
		.Join(
			context.Brands,
			player => player.BrandId,
			brand => brand.Id,
			(player, brand) => new
			{
				brand.LicenseeId,
				BrandId = brand.Id,
				BrandCode = brand.Code,
				player.PlayerCode,
				player.Username,
				WalletId = player.DefaultWalletId,
				CurrencyCode = player.DefaultWalletCurrencyCode
			})
		.Select(
			selector: player => new PlayerInfo(
				player.BrandId,
				player.BrandCode,
				player.PlayerCode,
				player.Username,
				player.WalletId,
				player.CurrencyCode))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(paramName: playerCode, message: $"This player does not exist.({playerCode})");

	public async ValueTask<bool> GetStatusAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await context.Players
		.Where(
			player => player.PlayerCode == playerCode)
		.Join(
			context.Brands,
			player => player.BrandId,
			brand => brand.Id,
			(player, brand) => new
			{
				brand.LicenseeId,
				BrandId = brand.Id,
				BrandCode = brand.Code,
				BrandStatus = brand.Status,
				player.PlayerCode,
				PlayerStatus = player.Status
			})
		.Join(
			context.Licensees,
			conform => conform.LicenseeId,
			licensee => licensee.Id,
			(conform, licensee) => new
			{
				licensee.ResellerId,
				conform.LicenseeId,
				LicenseeCode = licensee.Code,
				LicenseeStatus = licensee.Status,
				conform.BrandId,
				conform.BrandCode,
				conform.BrandStatus,
				conform.PlayerCode,
				conform.PlayerStatus
			})
		.Join(
			context.Resellers,
			conform => conform.ResellerId,
			reseller => reseller.Id,
			(conform, reseller) => new
			{
				conform.ResellerId,
				ResellerCode = reseller.Code,
				ResellerStatus = reseller.Status,
				conform.LicenseeId,
				conform.LicenseeCode,
				conform.LicenseeStatus,
				conform.BrandId,
				conform.BrandCode,
				conform.BrandStatus,
				conform.PlayerCode,
				conform.PlayerStatus
			})
		.AnyAsync(
			predicate: player => player.ResellerStatus == 0 && player.LicenseeStatus == 0 && player.BrandStatus == 0 && player.PlayerStatus == 0,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
