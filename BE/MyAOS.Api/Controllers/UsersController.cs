using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAOS.Domain.Dto;
using MyAOS.Service;
using System.Security.Claims;

namespace MyAOS.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UsersController(IUserService userService,IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query, CancellationToken ct)
        {
            var tenantIdValue =
                User.FindFirst("TenantId")?.Value
                ?? User.FindFirst("tenantId")?.Value;

            if (!Guid.TryParse(tenantIdValue, out var currentTenantId))
            {
                return Unauthorized(new { message = "Invalid token tenant." });
            }

            var role =
                User.FindFirst(ClaimTypes.Role)?.Value
                ?? User.FindFirst("role")?.Value
                ?? User.FindFirst("Role")?.Value
                ?? string.Empty;

            var result = await _userService.GetUsersAsync(
                currentTenantId,
                role,
                query,
                ct);

            return Ok(result);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUsers([FromBody] CreateUsersRequest request, CancellationToken ct)
        {
            var tenantIdValue =
                User.FindFirst("TenantId")?.Value
                ?? User.FindFirst("tenantId")?.Value
                ?? _configuration["DefaultTenantId"];

            if (!Guid.TryParse(tenantIdValue, out var currentTenantId))
            {
                return Unauthorized(new { message = "Invalid token tenant." });
            }
            Console.WriteLine("=== USER CLAIMS ===");

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            var role =
                User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                ?? User.FindFirst("role")?.Value
                ?? User.FindFirst("Role")?.Value
                ?? string.Empty;

            var actorEmail =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? "Unknown";

            var actorUserIdValue =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            Guid? actorUserId = null;

            if (Guid.TryParse(actorUserIdValue, out var parsedActorUserId))
            {
                actorUserId = parsedActorUserId;
            }

            var result = await _userService.CreateUsersAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                request,
                ct);

            return Created("/api/users", result);
        }
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id,[FromBody] UpdateUserRequest request,CancellationToken ct)
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

            var result = await _userService.UpdateUserAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                id,
                request,
                ct);

            return Ok(result);
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteUsers([FromBody] DeleteUsersRequest request,CancellationToken ct)
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

            await _userService.DeleteUsersAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                request,
                ct);

            return NoContent();
        }

        [HttpPatch("activate")]
        [Authorize]
        public async Task<IActionResult> ActivateUsers([FromBody] ActivateUsersRequest request, CancellationToken ct)
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

            await _userService.ActivateUsersAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                request,
                ct);

            return NoContent();
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        {
            try
            {
                var context = GetCurrentUserContext();

                var result = await _userService.GetMyProfileAsync(
                    context.TenantId,
                    context.UserId,
                    ct);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpPost("me/reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetMyPassword(
    [FromBody] ResetMyPasswordRequest request,
    CancellationToken ct)
        {
            try
            {
                var tenantIdValue =
                    User.FindFirst("TenantId")?.Value
                    ?? User.FindFirst("tenantId")?.Value;

                if (!Guid.TryParse(tenantIdValue, out var currentTenantId))
                {
                    return Unauthorized(new { message = "Invalid token tenant." });
                }

                var userIdValue =
                    User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

                if (!Guid.TryParse(userIdValue, out var currentUserId))
                {
                    return Unauthorized(new { message = "Invalid token user." });
                }

                var actorEmail =
                    User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
                    ?? "Unknown";

                var result = await _userService.ResetMyPasswordAsync(
                    currentTenantId,
                    currentUserId,
                    actorEmail,
                    request,
                    ct);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile(
    [FromBody] UpdateMyProfileRequest request,
    CancellationToken ct)
        {
            try
            {
                var context = GetCurrentUserContext();

                var result = await _userService.UpdateMyProfileAsync(
                    context.TenantId,
                    context.UserId,
                    context.Email,
                    request,
                    ct);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        private (Guid TenantId, Guid UserId, string Email) GetCurrentUserContext()
        {
            var tenantIdValue =
                User.FindFirst("TenantId")?.Value
                ?? User.FindFirst("tenantId")?.Value;

            if (!Guid.TryParse(tenantIdValue, out var tenantId))
            {
                throw new UnauthorizedAccessException("Invalid token tenant.");
            }

            var userIdValue =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid token user.");
            }

            var email =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
                ?? "Unknown";

            return (tenantId, userId, email);
        }

        [HttpPatch("deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateUsers([FromBody] DeactivateUsersRequest request,CancellationToken ct)
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

            await _userService.DeactivateUsersAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                request,
                ct);

            return NoContent();
        }
        [HttpPut("{id:guid}/products")]
        [Authorize]
        public async Task<IActionResult> AssignUserProducts(Guid id,[FromBody] AssignUserProductsRequest request,CancellationToken ct)
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

            var result = await _userService.AssignUserProductsAsync(
                currentTenantId,
                role,
                actorEmail,
                actorUserId,
                id,
                request,
                ct);

            return Ok(result);
        }
    }
}
