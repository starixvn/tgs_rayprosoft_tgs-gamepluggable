using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameActionInfo(
        Guid Id,
        Guid RoundId,
        string PlayerCode,
        string ExternalTransactionId,
        GameActionType Type,
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
        DateTimeOffset StoredOn
        );
}
