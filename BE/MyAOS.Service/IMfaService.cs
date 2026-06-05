using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Service
{
    public interface IMfaService
    {
        Task GenerateAndSendCodeAsync(UserEntity user, CancellationToken ct = default);
        Task<bool> ValidateCodeAsync(Guid userId, string code, CancellationToken ct = default);
    }
}
