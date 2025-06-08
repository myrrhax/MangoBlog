﻿using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IIntegrationRepository
{
    Task<Integration?> GetIntegrationByCode(string code, CancellationToken cancellationToken);
    Task<TelegramIntegration?> GetTelegramIntegration(Guid userId, CancellationToken cancellationToken);
    Task<Result> AddTelegramIntegration(TelegramIntegration integration, CancellationToken cancellationToken);
    Task<Result> ConfirmTelegramIntegration(string integrationCode, CancellationToken cancellationToken);
    Task<Result> DeleteTelegramIntegration(Guid userId, CancellationToken cancellationToken);
    Task<Result> DeleteFromTelegramChannel(Guid userId, string channelId, CancellationToken cancellationToken);
}
