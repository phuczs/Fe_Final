using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAOS.Domain.Dto;
using MyAOS.Service;

namespace MyAOS.Api.Controllers
{
    [ApiController]
    [Route("api/whitelist")]
    [Authorize]
    public class WhitelistController : ControllerBase
    {
        private readonly IWhitelistService _whitelistService;

        public WhitelistController(IWhitelistService whitelistService)
        {
            _whitelistService = whitelistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWhitelistSettings(CancellationToken ct)
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

            var result = await _whitelistService.GetWhitelistSettingsAsync(
                currentTenantId,
                role,
                ct);

            return Ok(result);
        }
    
    [HttpPut("toggle")]
        [Authorize]
        public async Task<IActionResult> ToggleWhitelist(
    [FromBody] ToggleWhitelistRequest request,
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

            var actorEmail =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
                ?? "Unknown";

            var actorUserIdValue =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            Guid? actorUserId = null;

            if (Guid.TryParse(actorUserIdValue, out var parsedActorUserId))
            {
                actorUserId = parsedActorUserId;
            }

            var result = await _whitelistService.ToggleWhitelistAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                request,
                ct);

            return Ok(result);
        }
        [HttpPost("emails")]
        [Authorize]
        public async Task<IActionResult> AddEmails(
    [FromBody] AddWhitelistEmailsRequest request,
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

            var actorEmail =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
                ?? "Unknown";

            var actorUserIdValue =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            Guid? actorUserId = null;

            if (Guid.TryParse(actorUserIdValue, out var parsedActorUserId))
            {
                actorUserId = parsedActorUserId;
            }

            var result = await _whitelistService.AddEmailsAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                request,
                ct);

            return Ok(result);
        }

        // POST /api/whitelist/emails/send — send an email to all whitelisted addresses
        [HttpPost("emails/send")]
        [Authorize]
        public async Task<IActionResult> SendEmailToWhitelist(
    [FromBody] SendWhitelistEmailRequest request,
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

            var result = await _whitelistService.SendEmailToWhitelistAsync(
                currentTenantId,
                role,
                request,
                ct);

            return Ok(result);
        }
        [HttpDelete("emails/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteEmail(
    Guid id,
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

            var actorEmail =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
                ?? "Unknown";

            var actorUserIdValue =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            Guid? actorUserId = null;

            if (Guid.TryParse(actorUserIdValue, out var parsedActorUserId))
            {
                actorUserId = parsedActorUserId;
            }

            await _whitelistService.DeleteEmailAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                id,
                ct);

            return NoContent();
        }
    }
}
