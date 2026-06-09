using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAOS.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(CancellationToken ct)
        {
            var tenantIdValue = User.FindFirst("TenantId")?.Value ?? User.FindFirst("tenantId")?.Value;
            var userIdValue = User.FindFirst("UserId")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (!Guid.TryParse(tenantIdValue, out var currentTenantId) || !Guid.TryParse(userIdValue, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            var result = await _notificationService.GetUserNotificationsAsync(currentTenantId, currentUserId, ct);
            return Ok(result);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct)
        {
            var userIdValue = User.FindFirst("UserId")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (!Guid.TryParse(userIdValue, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            await _notificationService.MarkAsReadAsync(currentUserId, id, ct);
            return Ok();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
        {
            var tenantIdValue = User.FindFirst("TenantId")?.Value ?? User.FindFirst("tenantId")?.Value;
            var userIdValue = User.FindFirst("UserId")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (!Guid.TryParse(tenantIdValue, out var currentTenantId) || !Guid.TryParse(userIdValue, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            await _notificationService.MarkAllAsReadAsync(currentTenantId, currentUserId, ct);
            return Ok();
        }
    }
}
