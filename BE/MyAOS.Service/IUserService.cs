using MyAOS.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public interface IUserService
    {
        Task<AuthResponseDto> LoginAsync(Guid tenantId, LoginRequest request, CancellationToken ct = default);
        Task<RegisterResponseDto> RegisterAsync(Guid tenanId, RegisterRequest request, CancellationToken ct = default);
        Task<AuthResponseDto> VerifyMfaAsync(MfaVerifyRequest request, CancellationToken ct = default);
        Task<VerificationCodeResponse> SendRegistrationVerificationCodeAsync(Guid tenantId, SendVerificationCodeRequest request, CancellationToken ct = default);
        Task<VerificationCodeResponse> VerifyRegistrationCodeAsync(Guid tenantId, VerifyRegistrationCodeRequest request, CancellationToken ct = default);
        Task<PagedResultDto<UserListItemDto>> GetUsersAsync(Guid currentTenantId, string currentUserRole, GetUsersQuery query, CancellationToken ct = default);
        Task<CreateUsersResponseDto> CreateUsersAsync(Guid currentTenantId, string currentUserRole, string actorEmail, Guid? actorUserId, CreateUsersRequest request, CancellationToken ct = default);
        Task<UpdateUserResponseDto> UpdateUserAsync(Guid currentTenantId, string currentUserRole, string actorEmail, Guid? actorUserId, Guid userId, UpdateUserRequest request, CancellationToken ct = default);
        Task ActivateUsersAsync(Guid currentTenantId, string currentUserRole, string actorEmail, Guid? actorUserId, ActivateUsersRequest request, CancellationToken ct = default);
        Task DeactivateUsersAsync(Guid currentTenantId, string currentUserRole, string actorEmail, Guid? actorUserId, DeactivateUsersRequest request, CancellationToken ct = default);
        Task DeleteUsersAsync(Guid currentTenantId, string currentUserRole, string actorEmail, Guid? actorUserId, DeleteUsersRequest request, CancellationToken ct = default);
        Task<ToggleUsersMfaResponseDto> ToggleUsersMfaAsync(Guid currentTenantId, string currentUserRole, string actorEmail, Guid? actorUserId, ToggleUsersMfaRequest request, CancellationToken ct = default);
        Task<AssignUserProductsResponseDto> AssignUserProductsAsync(Guid currentTenantId, string currentUserRole, string actorEmail, Guid? actorUserId, Guid userId, AssignUserProductsRequest request, CancellationToken ct = default);
        Task<ToggleMyMfaResponseDto> ToggleMyMfaAsync(
    Guid currentTenantId,
    Guid currentUserId,
    string actorEmail,
    ToggleMyMfaRequest request,
    CancellationToken ct = default);

        Task<MyProfileDto> GetMyProfileAsync(
        Guid currentTenantId,
        Guid currentUserId,
        CancellationToken ct = default);

        Task<MyProfileDto> UpdateMyProfileAsync(
            Guid currentTenantId,
            Guid currentUserId,
            string actorEmail,
            UpdateMyProfileRequest request,
            CancellationToken ct = default);
        Task<ResetMyPasswordResponseDto> ResetMyPasswordAsync(
    Guid currentTenantId,
    Guid currentUserId,
    string actorEmail,
    ResetMyPasswordRequest request,
    CancellationToken ct = default);
    }
}
