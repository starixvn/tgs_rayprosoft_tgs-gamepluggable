using System;

namespace Tgs.GameProviders.Pluggable.ViewModels;

public record GameActionViewModel(
    Guid Id,
    Guid RoundId,
    string PlayerCode,
    string ExternalTransactionId,
    GameActionViewModelType Type,
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
