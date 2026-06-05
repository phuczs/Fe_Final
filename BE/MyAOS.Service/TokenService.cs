using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyAOS.Domain.Entity;
using MyAOS.Domain.Enum;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyAOS.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUserRepository _userRepo;

        public TokenService(
            IConfiguration config,
            IRefreshTokenRepository refreshTokenRepo,
            IUserRepository userRepo)
        {
            _config = config;
            _refreshTokenRepo = refreshTokenRepo;
            _userRepo = userRepo;
        }

        public string GenerateAccessToken(UserEntity user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("TenantId", user.TenantId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            int expiryMinutes = 60;
            var expirySection = _config.GetSection("Jwt")?.GetSection("ExpiryMinutes")?.Value;

            if (!string.IsNullOrEmpty(expirySection) && int.TryParse(expirySection, out var parsedExpiry))
            {
                expiryMinutes = parsedExpiry;
            }

            return BuildToken(claims, expiryMinutes);
        }

        public string GenerateMfaRequiredToken(UserEntity user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("TenantId", user.TenantId.ToString()),
                new Claim("MfaPending", "true")
            };

            return BuildToken(claims, 5);
        }

        public ClaimsPrincipal ValidateMfaRequiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out _);

            var mfaPending = principal.FindFirst("MfaPending")?.Value;

            if (mfaPending != "true")
            {
                throw new SecurityTokenException("Invalid MFA token.");
            }

            return principal;
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken ct = default)
        {
            var refreshToken = GenerateSecureRandomToken();
            var tokenHash = HashToken(refreshToken);

            var entity = new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepo.AddAsync(entity, ct);
            await _refreshTokenRepo.SaveChangesAsync(ct);

            return refreshToken;
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshAsync(
    string refreshToken,
    CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token is missing.");
            }

            var tokenHash = HashToken(refreshToken);

            var storedToken = await _refreshTokenRepo.GetByTokenHashAsync(tokenHash, ct);

            if (storedToken == null ||
                storedToken.RevokedAt != null ||
                storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = storedToken.User;

            if (user == null || user.Status != StatusType.Active)
            {
                throw new UnauthorizedAccessException("Invalid refresh token user.");
            }

            var newRefreshToken = GenerateSecureRandomToken();
            var newRefreshTokenHash = HashToken(newRefreshToken);

            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByTokenHash = newRefreshTokenHash;

            var newRefreshTokenEntity = new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepo.AddAsync(newRefreshTokenEntity, ct);
            await _refreshTokenRepo.SaveChangesAsync(ct);

            var newAccessToken = GenerateAccessToken(user);

            return (newAccessToken, newRefreshToken);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return;
            }

            var tokenHash = HashToken(refreshToken);

            var storedToken = await _refreshTokenRepo.GetByTokenHashAsync(tokenHash, ct);

            if (storedToken == null)
            {
                return;
            }

            if (storedToken.RevokedAt == null)
            {
                storedToken.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepo.SaveChangesAsync(ct);
            }
        }

        private string BuildToken(Claim[] claims, int expiryMinutes)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateSecureRandomToken()
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private static string HashToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}
