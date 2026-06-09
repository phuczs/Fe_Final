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
        private readonly IEmailSender _emailSender;

        public MfaService(
            IMfaCodeRepository mfaRepo,
            IEmailSender emailSender)
        {
            _mfaRepo = mfaRepo;
            _emailSender = emailSender;
        }

        public async Task GenerateAndSendCodeAsync(UserEntity user, CancellationToken ct = default)
        {
            // 1. Invalidate previous codes to prevent replay attacks
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
            var htmlBody = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Your Verification Code</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f4f6f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;"">
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #f4f6f9; padding: 40px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""500"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05); overflow: hidden; border: 1px solid #eef2f5;"">
                    <!-- Header -->
                    <tr>
                        <td align=""center"" style=""background: linear-gradient(135deg, #1677ff, #0958d9); padding: 30px 20px;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: 700; letter-spacing: 0.5px;"">MOS Platform</h1>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px; color: #333333;"">
                            <h2 style=""margin: 0 0 16px 0; font-size: 20px; font-weight: 600; color: #1a1a1a;"">Two-Factor Authentication</h2>
                            <p style=""margin: 0 0 24px 0; font-size: 15px; line-height: 1.6; color: #555555;"">To complete your sign-in, please use the following one-time verification code. This code is valid for <strong>3 minutes</strong>.</p>
                            
                            <!-- Code Container -->
                            <div style=""background-color: #f0f5ff; border: 1px dashed #adc6ff; border-radius: 8px; padding: 20px; text-align: center; margin: 30px 0;"">
                                <span style=""font-family: 'Courier New', Courier, monospace; font-size: 36px; font-weight: 700; letter-spacing: 6px; color: #1d39c4; display: inline-block;"">{code}</span>
                            </div>

                            <p style=""margin: 0 0 8px 0; font-size: 13px; line-height: 1.5; color: #8c8c8c;"">If you did not request this code, please ignore this email or secure your account password immediately.</p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td align=""center"" style=""background-color: #fafafa; padding: 20px; border-top: 1px solid #f0f0f0;"">
                            <p style=""margin: 0; font-size: 12px; color: #bfbfbf;"">&copy; {DateTime.UtcNow.Year} MOS System. All rights reserved.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

            await _emailSender.SendAsync(
                user.Email,
                "Your MOS Platform Login Code",
                htmlBody,
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
