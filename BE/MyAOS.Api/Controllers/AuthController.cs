using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAOS.Service;
using MyAOS.Domain.Dto;

namespace MyAOS.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IAuditService _auditService;

        public AuthController(
            IUserService userService,
            ITokenService tokenService,
            IConfiguration configuration,
        IAuditService auditService
         )
        {
            _userService = userService;
            _tokenService = tokenService;
            _configuration = configuration;
            _auditService = auditService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var tenantId = GetDefaultTenantId();

            var result = await _userService.RegisterAsync(tenantId, request, ct);

            return Created(string.Empty, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var tenantId = GetDefaultTenantId();

            var authResult = await _userService.LoginAsync(tenantId, request, ct);

            if (authResult.MfaRequired)
            {
                return Ok(new
                {
                    mfaRequired = true,
                    tempToken = authResult.TempToken
                });
            }

            AppendRefreshTokenCookie(authResult.RefreshToken!);

            return Ok(new
            {
                mfaRequired = false,
                accessToken = authResult.AccessToken
            });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["X-Refresh-Token"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(new
                {
                    message = "Refresh token is missing."
                });
            }

            var result = await _tokenService.RefreshAsync(refreshToken, ct);

            AppendRefreshTokenCookie(result.RefreshToken);

            return Ok(new
            {
                accessToken = result.AccessToken
            });
        }

        [HttpPost("mfa/verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyMfa([FromBody] MfaVerifyRequest request, CancellationToken ct)
        {
            var authResult = await _userService.VerifyMfaAsync(request, ct);

            AppendRefreshTokenCookie(authResult.RefreshToken!);

            return Ok(new
            {
                mfaRequired = false,
                accessToken = authResult.AccessToken
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["X-Refresh-Token"];

            await _tokenService.RevokeRefreshTokenAsync(refreshToken ?? string.Empty, ct);

            var tenantIdValue = User.FindFirst("TenantId")?.Value;

            var userIdValue =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var email =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? "Unknown";

            if (Guid.TryParse(tenantIdValue, out var tenantId))
            {
                Guid? userId = null;

                if (Guid.TryParse(userIdValue, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                await _auditService.LogAsync(
                    tenantId,
                    email,
                    "Session",
                    "SignOut",
                    email,
                    userId,
                    "{\"Result\":\"Success\"}",
                    ct);
            }

            Response.Cookies.Delete("X-Refresh-Token", new CookieOptions
            {
                Path = "/"
            });

            return NoContent();
        }

        [HttpPost("register/verification-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationCodeRequest request, CancellationToken ct)
        {
            var tenantId = GetDefaultTenantId();

            var result = await _userService.SendRegistrationVerificationCodeAsync(tenantId, request, ct);

            return Ok(result);
        }
    //    [HttpPatch("mfa/toggle")]
    //    [Authorize]
    //    public async Task<IActionResult> ToggleUsersMfa(
    //[FromBody] ToggleUsersMfaRequest request,
    //CancellationToken ct)
    //    {
    //        var tenantIdValue =
    //            User.FindFirst("TenantId")?.Value
    //            ?? User.FindFirst("tenantId")?.Value;

    //        if (!Guid.TryParse(tenantIdValue, out var currentTenantId))
    //        {
    //            return Unauthorized(new { message = "Invalid token tenant." });
    //        }

    //        var role =
    //            User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
    //            ?? User.FindFirst("role")?.Value
    //            ?? User.FindFirst("Role")?.Value
    //            ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/role"))?.Value
    //            ?? string.Empty;

    //        var actorEmail =
    //            User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
    //            ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
    //            ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
    //            ?? "Unknown";

    //        var actorUserIdValue =
    //            User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
    //            ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
    //            ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

    //        Guid? actorUserId = null;

    //        if (Guid.TryParse(actorUserIdValue, out var parsedActorUserId))
    //        {
    //            actorUserId = parsedActorUserId;
    //        }

    //        var result = await _userService.ToggleUsersMfaAsync(
    //            currentTenantId,
    //            role,
    //            actorEmail,
    //            actorUserId,
    //            request,
    //            ct);

    //        return Ok(result);
    //    }
        [HttpPatch("me/mfa")]
        [Authorize]
        public async Task<IActionResult> ToggleMyMfa(
    [FromBody] ToggleMyMfaRequest request,
    CancellationToken ct)
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

            var result = await _userService.ToggleMyMfaAsync(
                currentTenantId,
                currentUserId,
                actorEmail,
                request,
                ct);

            return Ok(result);
        }

        //[HttpPost("register/verify-code")]
        //[AllowAnonymous]
        //public async Task<IActionResult> VerifyCode([FromBody] VerifyRegistrationCodeRequest request, CancellationToken ct)
        //{
        //    var tenantId = GetDefaultTenantId();

        //    var result = await _userService.VerifyRegistrationCodeAsync(tenantId, request, ct);

        //    return Ok(result);
        //}

        private Guid GetDefaultTenantId()
        {
            var tenantIdValue = _configuration["Tenant:DefaultTenantId"];

            if (string.IsNullOrWhiteSpace(tenantIdValue))
            {
                throw new InvalidOperationException("Tenant:DefaultTenantId is missing in configuration.");
            }

            if (!Guid.TryParse(tenantIdValue, out var tenantId) || tenantId == Guid.Empty)
            {
                throw new InvalidOperationException("Tenant:DefaultTenantId is invalid.");
            }

            return tenantId;
        }

        private void AppendRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"
            };

            Response.Cookies.Append("X-Refresh-Token", refreshToken, cookieOptions);
        }
    }
}