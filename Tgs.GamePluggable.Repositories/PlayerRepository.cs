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
		.Join(
			context.Licensees,
			combine => combine.LicenseeId,
			licensee => licensee.Id,
			(combine, licensee) => new
			{
				licensee.ResellerId,
				combine.LicenseeId,
				LicenseeCode = licensee.Code,
				combine.BrandId,
				combine.BrandCode,
				combine.PlayerCode,
				combine.Username,
				combine.WalletId,
				combine.CurrencyCode
			})
		.Join(
			context.Resellers,
			combine => combine.ResellerId,
			resellers => resellers.Id,
			(combine, licensee) => new
			{
				combine.ResellerId,
				ResellerCode = licensee.Code,
				combine.LicenseeId,
				combine.LicenseeCode,
				combine.BrandId,
				combine.BrandCode,
				combine.PlayerCode,
				combine.Username,
				combine.WalletId,
				combine.CurrencyCode
			})
		.Where(
			player => player.PlayerCode == playerCode)
		.Select(
			selector: combine => new PlayerInfo(
				combine.ResellerId,
				combine.ResellerCode,
				combine.LicenseeId,
				combine.LicenseeCode,
				combine.BrandId,
				combine.BrandCode,
				combine.PlayerCode,
				combine.Username,
				combine.WalletId,
				combine.CurrencyCode))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(paramName: playerCode, message: $"This player does not exist.({playerCode})");

	public async ValueTask<bool> GetStatusAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await context.Players
		.Join(
			context.Brands,
			player => player.BrandId,
			brand => brand.Id,
			(player, brand) => new
			{
				brand.LicenseeId,
				BrandId = brand.Id,
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
				LicenseeStatus = licensee.Status,
				conform.BrandId,
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
				ResellerStatus = reseller.Status,
				conform.LicenseeStatus,
				conform.BrandStatus,
				conform.PlayerCode,
				conform.PlayerStatus
			})
		.AnyAsync(
			predicate: player => player.PlayerCode == playerCode &&
			player.ResellerStatus == 0 &&
			player.LicenseeStatus == 0 &&
			player.BrandStatus == 0 &&
			player.PlayerStatus == 0,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
