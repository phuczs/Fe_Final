using MyAOS.Domain.Dto;
using MyAOS.Domain.Entity;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyAOS.Service
{
    public class RegistrationVerificationService : IRegistrationVerificationService
    {
        private readonly IRegistrationVerificationCodeRepository _verificationRepo;

        public RegistrationVerificationService(
            IRegistrationVerificationCodeRepository verificationRepo)
        {
            _verificationRepo = verificationRepo;
        }

        public async Task<VerificationCodeResponse> SendVerificationCodeAsync(
            Guid tenantId,
            SendVerificationCodeRequest request,
            CancellationToken ct = default)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(request.EmailAddress))
            {
                throw new ArgumentException("Email address is required.");
            }

            var email = request.EmailAddress.Trim().ToLowerInvariant();

            await _verificationRepo.InvalidatePreviousCodesAsync(
                tenantId,
                email,
                ct);

            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            var entity = new RegistrationVerificationCodeEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Email = email,
                PhoneNumber = request.PhoneNumber?.Trim(),
                Code = code,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Verified = false
            };

            await _verificationRepo.AddAsync(entity, ct);
            await _verificationRepo.SaveChangesAsync(ct);

            return new VerificationCodeResponse
            {
                Success = true,
                Message = "Verification code generated and saved to database.",
                CodeSent = "database",
                Code = code
            };
        }

        public async Task<VerificationCodeResponse> VerifyCodeAsync(
            Guid tenantId,
            VerifyRegistrationCodeRequest request,
            CancellationToken ct = default)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(request.EmailAddress))
            {
                throw new ArgumentException("Email address is required.");
            }

            if (string.IsNullOrWhiteSpace(request.VerificationCode))
            {
                throw new ArgumentException("Verification code is required.");
            }

            var email = request.EmailAddress.Trim().ToLowerInvariant();
            var code = request.VerificationCode.Trim();

            var validCode = await _verificationRepo.GetValidCodeAsync(
                tenantId,
                email,
                code,
                ct);

            if (validCode == null)
            {
                return new VerificationCodeResponse
                {
                    Success = false,
                    Message = "Invalid or expired verification code.",
                    CodeSent = null,
                    Code = code
                };
            }

            validCode.Verified = true;

            await _verificationRepo.SaveChangesAsync(ct);

            return new VerificationCodeResponse
            {
                Success = true,
                Message = "Verification code verified successfully.",
                CodeSent = null,
                Code = null
            };
        }
    }
}
