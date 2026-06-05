using MyAOS.Domain.Entity;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyAOS.Service
{
    public class MfaService : IMfaService
    {
        private readonly IMfaCodeRepository _mfaRepo;
        private readonly IEmailWhitelistRepository _whitelistRepo;
        private readonly IEmailSender _emailSender;

        public MfaService(
            IMfaCodeRepository mfaRepo,
            IEmailWhitelistRepository whitelistRepo,
            IEmailSender emailSender)
        {
            _mfaRepo = mfaRepo;
            _whitelistRepo = whitelistRepo;
            _emailSender = emailSender;
        }

        public async Task GenerateAndSendCodeAsync(UserEntity user, CancellationToken ct = default)
        {
            // 1. Verify Whitelist
            var isWhitelisted = await _whitelistRepo.IsEmailWhitelistedAsync(user.TenantId, user.Email, ct);
            if (!isWhitelisted) return; // Silent abort to prevent email spam/bounces

            // 2. Invalidate previous codes to prevent replay attacks
            await _mfaRepo.InvalidateAllPreviousCodesAsync(user.Id, ct);

            // 3. Generate secure 6-digit code
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            var mfaEntity = new MfaCodeEntity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Code = code,
                Used = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(3)
            };
            Console.WriteLine($"MFA DEBUG Id = {mfaEntity.Id}");
            await _mfaRepo.AddAsync(mfaEntity, ct);
            await _mfaRepo.SaveChangesAsync(ct);

            // 4. Send Email via IEmailSender
            await _emailSender.SendAsync(
                user.Email,
                "Your MOS Platform Login Code",
                $"Your login code is: {code}. It expires in 3 minutes.",
                ct);
        }

        public async Task<bool> ValidateCodeAsync(Guid userId, string code, CancellationToken ct = default)
        {
            var validCode = await _mfaRepo.GetValidCodeAsync(userId, code, ct);
            if (validCode == null) return false;

            // Mark as used
            validCode.Used = true;
            await _mfaRepo.SaveChangesAsync(ct);
            return true;
        }
    }
}
