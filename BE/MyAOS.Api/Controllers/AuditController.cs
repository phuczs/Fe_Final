using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAOS.Domain.Dto;
using MyAOS.Service;

namespace MyAOS.Api.Controllers
{
    [ApiController]
    [Route("api/audit")]
    [Authorize]
    public class AuditController : ControllerBase
    {
        private readonly IAuditQueryService _auditQueryService;

        public AuditController(IAuditQueryService auditQueryService)
        {
            _auditQueryService = auditQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] GetAuditLogsQuery query,
            CancellationToken ct)
        {
            var tenantIdValue =
                User.FindFirst("TenantId")?.Value
                ?? User.FindFirst("tenantId")?.Value;

            if (!Guid.TryParse(tenantIdValue, out var currentTenantId))
            {
                return Unauthorized(new { message = "Invalid token tenant." });
            }
            var role =
    User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
    ?? User.FindFirst("role")?.Value
    ?? User.FindFirst("Role")?.Value
    ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/role"))?.Value
    ?? string.Empty;

            var isAdmin =
                string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Administrator", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "TenantAdmin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "SystemAdmin", StringComparison.OrdinalIgnoreCase) ||
                role == "1" ||
                role == "2";

            if (!isAdmin)
            {
                return Forbid();
            }

            var result = await _auditQueryService.GetAuditLogsAsync(
                currentTenantId,
                query,
                ct);

            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportAuditLogs(
            [FromQuery] GetAuditLogsQuery query,
            CancellationToken ct)
        {
            var tenantIdValue =
                User.FindFirst("TenantId")?.Value
                ?? User.FindFirst("tenantId")?.Value;

            if (!Guid.TryParse(tenantIdValue, out var currentTenantId))
            {
                return Unauthorized(new { message = "Invalid token tenant." });
            }
            var role =
    User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
    ?? User.FindFirst("role")?.Value
    ?? User.FindFirst("Role")?.Value
    ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/role"))?.Value
    ?? string.Empty;

            var isAdmin =
                string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Administrator", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "TenantAdmin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "SystemAdmin", StringComparison.OrdinalIgnoreCase) ||
                role == "1" ||
                role == "2";

            if (!isAdmin)
            {
                return Forbid();
            }

            var fileBytes = await _auditQueryService.ExportAuditLogsCsvAsync(
                currentTenantId,
                query,
                ct);

            var fileName = $"audit-logs-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

            return File(fileBytes, "text/csv", fileName);
        }
    }
}
