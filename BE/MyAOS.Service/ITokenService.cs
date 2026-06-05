using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MyAOS.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserEntity user);
        string GenerateMfaRequiredToken(UserEntity user);
        ClaimsPrincipal ValidateMfaRequiredToken(string token);
        Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken ct = default);
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken, CancellationToken ct = default);
    }
}
