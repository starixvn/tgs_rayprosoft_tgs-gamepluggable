using System;

namespace Tgs.GamePluggable.Core.Models;
public record PlayerInfo(
	Guid BrandId,
	string BrandCode,
	string PlayerCode,
	string Username,
	Guid WalletId,
	string CurrencyCode);
