using System;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

public record GameActionResult(
	Guid Id,
	Guid RoundId,
	string PlayerCode,
	string ExternalTransactionId,
	Enums.GameActionType Type,
	decimal Amount,
	decimal Turnover,
	decimal WinLoss,
	decimal UnsettledBets,
	decimal Balance,
	string? BetType,
	string ExternalBetId,
	string ExternalGameActionGroupId,
	Guid? ReferencedGameActionId,
	Guid? GameActionBatchId,
	DateTimeOffset RecordedOn,
	DateTimeOffset? SentOn,
	bool IsClosingRound,
	bool? ClosingRoundExplicitRequest,
	string PlayerIp,
	string? ExtraData,
	string? JackpotContribution,
	DateTimeOffset StoredOn);
