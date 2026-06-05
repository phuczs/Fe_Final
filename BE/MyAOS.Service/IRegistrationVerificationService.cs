using MyAOS.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public interface IRegistrationVerificationService
    {
        Task<VerificationCodeResponse> SendVerificationCodeAsync(
            Guid tenantId,
            SendVerificationCodeRequest request,
            CancellationToken ct = default);

        Task<VerificationCodeResponse> VerifyCodeAsync(
            Guid tenantId,
            VerifyRegistrationCodeRequest request,
            CancellationToken ct = default);
    }
}
