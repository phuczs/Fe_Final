using MyAOS.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public interface IWhitelistService
    {
        Task<WhitelistSettingsDto> GetWhitelistSettingsAsync(
            Guid currentTenantId,
            string currentUserRole,
            CancellationToken ct = default);
        Task<ToggleWhitelistResponseDto> ToggleWhitelistAsync(
    Guid currentTenantId,
    string currentUserRole,
    string actorEmail,
    Guid? actorUserId,
    ToggleWhitelistRequest request,
    CancellationToken ct = default);
        Task<AddWhitelistEmailsResponseDto> AddEmailsAsync(
    Guid currentTenantId,
    string currentUserRole,
    string actorEmail,
    Guid? actorUserId,
    AddWhitelistEmailsRequest request,
    CancellationToken ct = default);
        Task DeleteEmailAsync(
    Guid currentTenantId,
    string currentUserRole,
    string actorEmail,
    Guid? actorUserId,
    Guid whitelistEmailId,
    CancellationToken ct = default);
    }
}
