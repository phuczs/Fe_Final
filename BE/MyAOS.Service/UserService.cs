using MyAOS.Domain.Dto;
using MyAOS.Domain.Entity;
using MyAOS.Domain.Enum;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace MyAOS.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;
        private readonly IMfaService _mfaService;
        private readonly IAuditService _auditService;
        private readonly IProductRepository _productRepo;
        private readonly IUserProductRepository _userProductRepo;
        private readonly IRegistrationVerificationCodeRepository _verificationCodeRepo;
        private readonly IEmailSender _emailSender;

        public UserService(
            IUserRepository userRepo,
            ITokenService tokenService,
            IMfaService mfaService,
            IAuditService auditService,
            IProductRepository productRepository,
            IUserProductRepository userProductRepository,
            IRegistrationVerificationCodeRepository verificationCodeRepository,
            IEmailSender emailSender)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _mfaService = mfaService;
            _auditService = auditService;
            _productRepo = productRepository;
            _userProductRepo = userProductRepository;
            _verificationCodeRepo = verificationCodeRepository;
            _emailSender = emailSender;
        }
        public async Task<RegisterResponseDto> RegisterAsync(Guid tenantId, RegisterRequest request, CancellationToken ct = default)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(tenantId));
            }

            var email = request.EmailAddress.Trim().ToLowerInvariant();
            var userName = request.UserId.Trim();

            var existingEmail = await _userRepo.GetByEmailAsync(tenantId, email, ct);
            if (existingEmail != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            // TODO: Validate SecurityVerificationCode if you have a separate verification flow.
            // Currently, we accept it to support the form structure.

            var user = new UserEntity
            {
                TenantId = tenantId,
                UserName = userName,
                Email = email,
                FullName = request.FullName.Trim(),
                PhoneCountryCode = request.CountryCode.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = RoleType.TenantUser,
                Status = StatusType.Active,
                SignInMethod = SignInMethodType.Local,
                MfaEnabled = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user, ct);
            await _userRepo.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                tenantId,
                email,
                "User",
                "Create",
                email,
                user.Id,
                "{\"Result\":\"Success\",\"Source\":\"SelfRegister\"}",
                ct);

            return new RegisterResponseDto
            {
                UserId = user.Id,
                TenantId = user.TenantId,
                Email = user.Email,
                FullName = user.FullName
            };
        }

        public async Task<AuthResponseDto> LoginAsync(Guid tenantId, LoginRequest request, CancellationToken ct = default)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(tenantId));
            }
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepo.GetByEmailAsync(tenantId, email, ct);
            // Anti-Enumeration: Generic failure responses
            if (user == null || user.Status != StatusType.Active)
            {
                await _auditService.LogAsync(tenantId, "System", "Session", "SignIn", request.Email, null, "{\"Result\":\"Failed\",\"Reason\":\"UserNotFoundOrInactive\"}", ct);
                throw new UnauthorizedAccessException("User not found for this tenant.");

            }

            // Verify with BCrypt timing-safe comparison
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                await _auditService.LogAsync(tenantId, "System", "Session", "SignIn", request.Email, user.Id, "{\"Result\":\"Failed\",\"Reason\":\"InvalidPassword\"}", ct);
                throw new UnauthorizedAccessException("User has no local password.");
            }

            // Handle MFA
            if (user.MfaEnabled)
            {
                await _mfaService.GenerateAndSendCodeAsync(user, ct);
                return new AuthResponseDto
                {
                    MfaRequired = true,
                    TempToken = _tokenService.GenerateMfaRequiredToken(user)
                };
            }

            // Finalize Login
            await _auditService.LogAsync(tenantId, user.Email, "User", "SignIn", user.Email, user.Id, "{\"Result\":\"Success\"}", ct);

            return new AuthResponseDto
            {
                MfaRequired = false,
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, ct)
            };
        }
        public async Task<CreateUsersResponseDto> CreateUsersAsync(Guid currentTenantId, string currentUserRole,string actorEmail,Guid? actorUserId,CreateUsersRequest request,CancellationToken ct = default)
        {
            if (request.Users == null || request.Users.Count == 0)
            {
                throw new ArgumentException("Users list is required.");
            }

            var isAdmin = IsAdminRole(currentUserRole);

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            var response = new CreateUsersResponseDto();

            foreach (var item in request.Users)
            {
                var userName = item.UserId.Trim();
                var email = item.EmailAddress.Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(userName))
                {
                    throw new ArgumentException("User ID is required.");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new ArgumentException("Email address is required.");
                }

                if (string.IsNullOrWhiteSpace(item.DisplayName))
                {
                    throw new ArgumentException("Display name is required.");
                }

                var targetTenantId = currentTenantId;

                var existingEmail = await _userRepo.GetByEmailAsync(targetTenantId, email, ct);
                if (existingEmail != null)
                {
                    throw new InvalidOperationException($"Email '{email}' already exists.");
                }

                var existingUserName = await _userRepo.GetByUserNameAsync(targetTenantId, userName, ct);
                if (existingUserName != null)
                {
                    throw new InvalidOperationException($"User ID '{userName}' already exists.");
                }

                var temporaryPassword = GenerateTemporaryPassword();

                var user = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = targetTenantId,
                    UserName = userName,
                    Email = email,
                    FullName = item.DisplayName.Trim(),
                    StaffStudentId = item.StaffStudentId?.Trim(),
                    Sex = item.Sex?.Trim(),
                    MobilePhone = item.MobilePhone?.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
                    Role = item.Role,
                    Status = StatusType.Active,
                    SignInMethod = item.SignInMethod,
                    MfaEnabled = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepo.AddAsync(user, ct);

                response.CreatedUsers.Add(new CreatedUserDto
                {
                    Id = user.Id,
                    UserId = user.UserName,
                    EmailAddress = user.Email,
                    DisplayName = user.FullName,
                    TemporaryPassword = temporaryPassword,
                    ProductIds = item.ProductIds.Distinct().ToList()
                });

                await _auditService.LogAsync(
                    targetTenantId,
                    actorEmail,
                    "User",
                    "Create",
                    user.Email,
                    user.Id,
                    "{" +
                    $"\"Result\":\"Success\"," +
                    $"\"UserId\":\"{user.UserName}\"," +
                    $"\"DisplayName\":\"{user.FullName}\"," +
                    $"\"CreatedBy\":\"{actorEmail}\"" +
                    "}",
                    ct);
            }

            await _userRepo.SaveChangesAsync(ct);

            return response;
        }
        public async Task<PagedResultDto<UserListItemDto>> GetUsersAsync(Guid currentTenantId,string currentUserRole,GetUsersQuery query,CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            var isAdmin = IsAdminRole(currentUserRole);

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            Guid? tenantFilter = query.TenantId ?? currentTenantId;

            var result = await _userRepo.GetPagedAsync(
    tenantFilter,
    query,
    ct);

            var items = new List<UserListItemDto>();

            foreach (var u in result.Items)
            {
                var productIds = u.UserProducts?.Select(up => up.ProductId).ToList() ?? new List<int>();

                var dto = new UserListItemDto
                {
                    Id = u.Id,
                    TenantId = u.TenantId,
                    DisplayName = u.FullName,
                    UserId = u.UserName,
                    EmailAddress = u.Email,
                    Role = u.Role,
                    Status = u.Status,
                    SignInMethod = u.SignInMethod,
                    LastLoginAt = u.LastLoginAt,
                    StaffStudentId = u.StaffStudentId,
                    Sex = u.Sex,
                    MobilePhone = u.MobilePhone,
                    ProductIds = productIds,
                    MfaEnabled = u.MfaEnabled,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                };

                items.Add(dto);
            }

            return new PagedResultDto<UserListItemDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                Items = items
            };
        }
        public async Task<ResetMyPasswordResponseDto> ResetMyPasswordAsync(
    Guid currentTenantId,
    Guid currentUserId,
    string actorEmail,
    ResetMyPasswordRequest request,
    CancellationToken ct = default)
        {
            if (currentTenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(currentTenantId));
            }

            if (currentUserId == Guid.Empty)
            {
                throw new ArgumentException("UserId is required.", nameof(currentUserId));
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                throw new ArgumentException("Current password is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ArgumentException("New password is required.");
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                throw new ArgumentException("Confirm new password does not match.");
            }

            if (request.NewPassword.Length < 8)
            {
                throw new ArgumentException("New password must be at least 8 characters.");
            }

            var user = await _userRepo.GetByIdAsync(currentTenantId, currentUserId, ct);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (user.Status != StatusType.Active)
            {
                throw new UnauthorizedAccessException("Inactive user cannot reset password.");
            }

            if (user.SignInMethod != SignInMethodType.Local)
            {
                throw new InvalidOperationException("Only local users can reset password.");
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new InvalidOperationException("User does not have a local password.");
            }

            var currentPasswordValid = BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                user.PasswordHash);

            if (!currentPasswordValid)
            {
                await _auditService.LogAsync(
                    user.TenantId,
                    actorEmail,
                    "User",
                    "Update",
                    user.Email,
                    user.Id,
                    "{\"Result\":\"Failed\",\"Event\":\"SelfResetPassword\",\"Reason\":\"InvalidCurrentPassword\"}",
                    ct);

                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            var sameAsOldPassword = BCrypt.Net.BCrypt.Verify(
                request.NewPassword,
                user.PasswordHash);

            if (sameAsOldPassword)
            {
                throw new ArgumentException("New password must be different from current password.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                user.TenantId,
                actorEmail,
                "User",
                "Update",
                user.Email,
                user.Id,
                "{\"Result\":\"Success\",\"Event\":\"SelfResetPassword\"}",
                ct);

            return new ResetMyPasswordResponseDto
            {
                Success = true,
                Message = "Password reset successfully."
            };
        }
        public async Task<UpdateUserResponseDto> UpdateUserAsync(Guid currentTenantId,string currentUserRole,string actorEmail,Guid? actorUserId,Guid userId,UpdateUserRequest request,CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            var user = await _userRepo.GetByIdAsync(currentTenantId, userId, ct);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var oldEmail = user.Email;
            var oldFullName = user.FullName;
            var oldRole = user.Role;
            var oldStatus = user.Status;
            var oldMfaEnabled = user.MfaEnabled;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var newEmail = request.Email.Trim().ToLowerInvariant();

                if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var existingUser = await _userRepo.GetByEmailAsync(user.TenantId, newEmail, ct);

                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        throw new InvalidOperationException($"User with email '{newEmail}' already exists.");
                    }

                    user.Email = newEmail;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                user.FullName = request.FullName.Trim();
            }

            if (request.Role.HasValue)
            {
                user.Role = request.Role.Value;
            }

            if (request.Status.HasValue)
            {
                user.Status = request.Status.Value;
            }

            if (request.MfaEnabled.HasValue)
            {
                user.MfaEnabled = request.MfaEnabled.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.SaveChangesAsync(ct);

            var changeDetail =
                "{" +
                $"\"OldEmail\":\"{oldEmail}\"," +
                $"\"NewEmail\":\"{user.Email}\"," +
                $"\"OldFullName\":\"{oldFullName}\"," +
                $"\"NewFullName\":\"{user.FullName}\"," +
                $"\"OldRole\":\"{oldRole}\"," +
                $"\"NewRole\":\"{user.Role}\"," +
                $"\"OldStatus\":\"{oldStatus}\"," +
                $"\"NewStatus\":\"{user.Status}\"," +
                $"\"OldMfaEnabled\":{oldMfaEnabled.ToString().ToLower()}," +
                $"\"NewMfaEnabled\":{user.MfaEnabled.ToString().ToLower()}," +
                $"\"UpdatedBy\":\"{actorEmail}\"" +
                "}";

            await _auditService.LogAsync(
                user.TenantId,
                actorEmail,
                "User",
                "Update",
                user.Email,
                user.Id,
                changeDetail,
                ct);

            return new UpdateUserResponseDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Status = user.Status,
                MfaEnabled = user.MfaEnabled,
                UpdatedAt = user.UpdatedAt
            };
        }
        public async Task DeleteUsersAsync(Guid currentTenantId,string currentUserRole,string actorEmail,Guid? actorUserId,DeleteUsersRequest request,CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            if (request.UserIds == null || request.UserIds.Count == 0)
            {
                throw new ArgumentException("UserIds is required.");
            }

            var ids = request.UserIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                throw new ArgumentException("UserIds is invalid.");
            }

            if (actorUserId.HasValue && ids.Contains(actorUserId.Value))
            {
                throw new InvalidOperationException("You cannot delete your own account.");
            }

            var users = await _userRepo.GetByIdsAsync(currentTenantId, ids, ct);

            if (users.Count == 0)
            {
                throw new KeyNotFoundException("No users found.");
            }

            var foundIds = users.Select(u => u.Id).ToHashSet();
            var missingIds = ids.Where(id => !foundIds.Contains(id)).ToList();

            if (missingIds.Count > 0)
            {
                throw new KeyNotFoundException($"Some users were not found: {string.Join(", ", missingIds)}");
            }

            foreach (var user in users)
            {
                await _auditService.LogAsync(
                    user.TenantId,
                    actorEmail,
                    "User",
                    "Delete",
                    user.Email,
                    user.Id,
                    $"{{\"Result\":\"Success\",\"DeletedBy\":\"{actorEmail}\"}}",
                    ct);
            }

            await _userRepo.DeleteRangeAsync(users, ct);
            await _userRepo.SaveChangesAsync(ct);
        }

        public async Task ActivateUsersAsync(Guid currentTenantId,string currentUserRole,string actorEmail,Guid? actorUserId,ActivateUsersRequest request,CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            if (request.UserIds == null || request.UserIds.Count == 0)
            {
                throw new ArgumentException("UserIds is required.");
            }

            var ids = request.UserIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                throw new ArgumentException("UserIds is invalid.");
            }

            var users = await _userRepo.GetByIdsAsync(currentTenantId, ids, ct);

            if (users.Count == 0)
            {
                throw new KeyNotFoundException("No users found.");
            }

            var foundIds = users.Select(u => u.Id).ToHashSet();
            var missingIds = ids.Where(id => !foundIds.Contains(id)).ToList();

            if (missingIds.Count > 0)
            {
                throw new KeyNotFoundException($"Some users were not found: {string.Join(", ", missingIds)}");
            }

            foreach (var user in users)
            {
                var oldStatus = user.Status;

                user.Status = StatusType.Active;
                user.UpdatedAt = DateTime.UtcNow;

                await _auditService.LogAsync(
                    user.TenantId,
                    actorEmail,
                    "User",
                    "Update",
                    user.Email,
                    user.Id,
                    "{" +
                    $"\"Result\":\"Success\"," +
                    $"\"OldStatus\":\"{oldStatus}\"," +
                    $"\"NewStatus\":\"{user.Status}\"," +
                    $"\"ActivatedBy\":\"{actorEmail}\"" +
                    "}",
                    ct);
            }

            await _userRepo.SaveChangesAsync(ct);
        }

        public async Task DeactivateUsersAsync(Guid currentTenantId,string currentUserRole,string actorEmail,Guid? actorUserId,DeactivateUsersRequest request,CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            if (request.UserIds == null || request.UserIds.Count == 0)
            {
                throw new ArgumentException("UserIds is required.");
            }

            var ids = request.UserIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                throw new ArgumentException("UserIds is invalid.");
            }

            if (actorUserId.HasValue && ids.Contains(actorUserId.Value))
            {
                throw new InvalidOperationException("You cannot deactivate your own account.");
            }

            var users = await _userRepo.GetByIdsAsync(currentTenantId, ids, ct);

            if (users.Count == 0)
            {
                throw new KeyNotFoundException("No users found.");
            }

            var foundIds = users.Select(u => u.Id).ToHashSet();
            var missingIds = ids.Where(id => !foundIds.Contains(id)).ToList();

            if (missingIds.Count > 0)
            {
                throw new KeyNotFoundException($"Some users were not found: {string.Join(", ", missingIds)}");
            }

            foreach (var user in users)
            {
                var oldStatus = user.Status;

                user.Status = StatusType.Inactive;
                user.UpdatedAt = DateTime.UtcNow;

                await _auditService.LogAsync(
                    user.TenantId,
                    actorEmail,
                    "User",
                    "Deactivate",
                    user.Email,
                    user.Id,
                    $"{{\"Result\":\"Success\",\"OldStatus\":\"{oldStatus}\",\"NewStatus\":\"{user.Status}\",\"DeactivatedBy\":\"{actorEmail}\"}}",
                    ct);
            }

            await _userRepo.SaveChangesAsync(ct);
        }
        public async Task<AssignUserProductsResponseDto> AssignUserProductsAsync(Guid currentTenantId,string currentUserRole,string actorEmail,Guid? actorUserId,Guid userId,AssignUserProductsRequest request,CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            if (request.ProductIds == null || request.ProductIds.Count == 0)
            {
                throw new ArgumentException("ProductIds is required.");
            }

            var productIds = request.ProductIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (productIds.Count == 0)
            {
                throw new ArgumentException("ProductIds is invalid.");
            }

            var user = await _userRepo.GetByIdAsync(currentTenantId, userId, ct);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (!string.Equals(user.Role.ToString(), "TenantUser", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(user.Role.ToString(), "User", StringComparison.OrdinalIgnoreCase) &&
                user.Role.ToString() != "0")
            {
                throw new InvalidOperationException("Product permissions can only be assigned to Tenant User.");
            }

            var activeProducts = await _productRepo.GetActiveByIdsAsync(productIds, ct);

            var activeProductIds = activeProducts
                .Select(p => p.Id)
                .ToHashSet();

            var invalidProductIds = productIds
                .Where(id => !activeProductIds.Contains(id))
                .ToList();

            if (invalidProductIds.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Some products are invalid or inactive: {string.Join(", ", invalidProductIds)}");
            }

            var oldProducts = await _userProductRepo.GetByUserIdAsync(user.Id, ct);
            var oldProductIds = oldProducts.Select(x => x.ProductId).OrderBy(x => x).ToList();

            await _userProductRepo.ReplaceUserProductsAsync(user.Id, productIds, ct);
            await _userProductRepo.SaveChangesAsync(ct);

            var newProductIds = productIds.OrderBy(x => x).ToList();

            await _auditService.LogAsync(
                user.TenantId,
                actorEmail,
                "UserProduct",
                "Update",
                user.Email,
                user.Id,
                "{" +
                $"\"Result\":\"Success\"," +
                $"\"OldProductIds\":\"{string.Join(",", oldProductIds)}\"," +
                $"\"NewProductIds\":\"{string.Join(",", newProductIds)}\"," +
                $"\"UpdatedBy\":\"{actorEmail}\"" +
                "}",
                ct);

            return new AssignUserProductsResponseDto
            {
                UserId = user.Id,
                ProductIds = newProductIds
            };
        }
        public async Task<MyProfileDto> GetMyProfileAsync(
    Guid currentTenantId,
    Guid currentUserId,
    CancellationToken ct = default)
        {
            if (currentTenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(currentTenantId));
            }

            if (currentUserId == Guid.Empty)
            {
                throw new ArgumentException("UserId is required.", nameof(currentUserId));
            }

            var user = await _userRepo.GetByIdAsync(currentTenantId, currentUserId, ct);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return MapToMyProfileDto(user);
        }
        public async Task<MyProfileDto> UpdateMyProfileAsync(
    Guid currentTenantId,
    Guid currentUserId,
    string actorEmail,
    UpdateMyProfileRequest request,
    CancellationToken ct = default)
        {
            if (currentTenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(currentTenantId));
            }

            if (currentUserId == Guid.Empty)
            {
                throw new ArgumentException("UserId is required.", nameof(currentUserId));
            }

            var user = await _userRepo.GetByIdAsync(currentTenantId, currentUserId, ct);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var oldFullName = user.FullName;
            var oldStaffStudentId = user.StaffStudentId;
            var oldSex = user.Sex;
            var oldMobilePhone = user.MobilePhone;

            if (!string.IsNullOrWhiteSpace(request.DisplayName))
            {
                user.FullName = request.DisplayName.Trim();
            }

            user.StaffStudentId = string.IsNullOrWhiteSpace(request.StaffStudentId)
                ? null
                : request.StaffStudentId.Trim();

            user.Sex = string.IsNullOrWhiteSpace(request.Sex)
                ? null
                : request.Sex.Trim();

            user.MobilePhone = string.IsNullOrWhiteSpace(request.MobilePhone)
                ? null
                : request.MobilePhone.Trim();

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                user.TenantId,
                actorEmail,
                "User",
                "Update",
                user.Email,
                user.Id,
                "{" +
                $"\"Result\":\"Success\"," +
                $"\"Event\":\"SelfUpdateProfile\"," +
                $"\"OldDisplayName\":\"{oldFullName}\"," +
                $"\"NewDisplayName\":\"{user.FullName}\"," +
                $"\"OldStaffStudentId\":\"{oldStaffStudentId}\"," +
                $"\"NewStaffStudentId\":\"{user.StaffStudentId}\"," +
                $"\"OldSex\":\"{oldSex}\"," +
                $"\"NewSex\":\"{user.Sex}\"," +
                $"\"OldMobilePhone\":\"{oldMobilePhone}\"," +
                $"\"NewMobilePhone\":\"{user.MobilePhone}\"" +
                "}",
                ct);

            return MapToMyProfileDto(user);
        }
        private static MyProfileDto MapToMyProfileDto(UserEntity user)
        {
            return new MyProfileDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                DisplayName = user.FullName,
                UserId = user.UserName,
                EmailAddress = user.Email,
                StaffStudentId = user.StaffStudentId,
                Sex = user.Sex,
                MobilePhone = user.MobilePhone,
                MfaEnabled = user.MfaEnabled
            };
        }
        private static bool IsAdminRole(string role)
        {
            return
                string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Administrator", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "TenantAdmin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "SystemAdmin", StringComparison.OrdinalIgnoreCase) ||
                role == "1" ||
                role == "2";
        }

        private static string GenerateTemporaryPassword()
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "23456789";
            const string special = "@#$!";
            const string all = upper + lower + digits + special;

            var random = System.Security.Cryptography.RandomNumberGenerator.Create();

            char GetRandomChar(string source)
            {
                var bytes = new byte[4];
                random.GetBytes(bytes);
                var value = BitConverter.ToUInt32(bytes, 0);
                return source[(int)(value % (uint)source.Length)];
            }

            var chars = new List<char>
    {
        GetRandomChar(upper),
        GetRandomChar(lower),
        GetRandomChar(digits),
        GetRandomChar(special)
    };

            for (var i = chars.Count; i < 12; i++)
            {
                chars.Add(GetRandomChar(all));
            }

            return new string(chars.OrderBy(_ => Guid.NewGuid()).ToArray());
        }
        public async Task<AuthResponseDto> VerifyMfaAsync(MfaVerifyRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.TempToken))
            {
                throw new UnauthorizedAccessException("Temp token is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                throw new ArgumentException("MFA code is required.", nameof(request.Code));
            }

            var principal = _tokenService.ValidateMfaRequiredToken(request.TempToken);

            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"CLAIM: {claim.Type} = {claim.Value}");
            }

            var userIdValue =
                principal.FindFirst("sub")?.Value
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            var tenantIdValue =
                principal.FindFirst("TenantId")?.Value
                ?? principal.FindFirst("tenantId")?.Value
                ?? principal.Claims.FirstOrDefault(c =>
                    string.Equals(c.Type, "TenantId", StringComparison.OrdinalIgnoreCase))?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException($"Invalid MFA token user. userIdValue={userIdValue}");
            }

            if (!Guid.TryParse(tenantIdValue, out var tenantId))
            {
                throw new UnauthorizedAccessException($"Invalid MFA token tenant. tenantIdValue={tenantIdValue}");
            }

            var user = await _userRepo.GetByIdAsync(tenantId, userId, ct);

            if (user == null || user.Status != StatusType.Active)
            {
                throw new UnauthorizedAccessException("Invalid MFA token.");
            }

            var isValidCode = await _mfaService.ValidateCodeAsync(user.Id, request.Code.Trim(), ct);

            if (!isValidCode)
            {
                await _auditService.LogAsync(
                    tenantId,
                    user.Email,
                    "User",
                    "SignIn",
                    user.Email,
                    user.Id,
                    "{\"Result\":\"Failed\",\"Reason\":\"InvalidMfaCode\"}",
                    ct);

                throw new UnauthorizedAccessException("Invalid or expired MFA code.");
            }


            await _auditService.LogAsync(
                tenantId,
                user.Email,
                "User",
                "SignIn",
                user.Email,
                user.Id,
                "{\"Result\":\"Success\",\"Mfa\":true}",
                ct);

            return new AuthResponseDto
            {
                MfaRequired = false,
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, ct)
            };
        }
        public async Task<ToggleUsersMfaResponseDto> ToggleUsersMfaAsync(
    Guid currentTenantId,
    string currentUserRole,
    string actorEmail,
    Guid? actorUserId,
    ToggleUsersMfaRequest request,
    CancellationToken ct = default)
        {
            if (!IsAdminRole(currentUserRole))
            {
                throw new UnauthorizedAccessException("Admin permission is required.");
            }

            if (request.UserIds == null || request.UserIds.Count == 0)
            {
                throw new ArgumentException("UserIds is required.");
            }

            var ids = request.UserIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                throw new ArgumentException("UserIds is invalid.");
            }

            var users = await _userRepo.GetByIdsAsync(currentTenantId, ids, ct);

            if (users.Count == 0)
            {
                throw new KeyNotFoundException("No users found.");
            }

            var foundIds = users.Select(u => u.Id).ToHashSet();
            var missingIds = ids.Where(id => !foundIds.Contains(id)).ToList();

            if (missingIds.Count > 0)
            {
                throw new KeyNotFoundException($"Some users were not found: {string.Join(", ", missingIds)}");
            }

            var updatedUserIds = new List<Guid>();

            foreach (var user in users)
            {
                var oldMfaEnabled = user.MfaEnabled;

                if (oldMfaEnabled == request.Enabled)
                {
                    continue;
                }

                user.MfaEnabled = request.Enabled;
                user.UpdatedAt = DateTime.UtcNow;

                updatedUserIds.Add(user.Id);

                await _auditService.LogAsync(
                    user.TenantId,
                    actorEmail,
                    "User",
                    "Update",
                    user.Email,
                    user.Id,
                    "{" +
                    $"\"Result\":\"Success\"," +
                    $"\"OldMfaEnabled\":{oldMfaEnabled.ToString().ToLowerInvariant()}," +
                    $"\"NewMfaEnabled\":{user.MfaEnabled.ToString().ToLowerInvariant()}," +
                    $"\"UpdatedBy\":\"{actorEmail}\"" +
                    "}",
                    ct);
            }

            await _userRepo.SaveChangesAsync(ct);

            return new ToggleUsersMfaResponseDto
            {
                Enabled = request.Enabled,
                UpdatedCount = updatedUserIds.Count,
                UpdatedUserIds = updatedUserIds
            };
        }
        public async Task<ToggleMyMfaResponseDto> ToggleMyMfaAsync(
    Guid currentTenantId,
    Guid currentUserId,
    string actorEmail,
    ToggleMyMfaRequest request,
    CancellationToken ct = default)
        {
            if (currentTenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(currentTenantId));
            }

            if (currentUserId == Guid.Empty)
            {
                throw new ArgumentException("UserId is required.", nameof(currentUserId));
            }

            var user = await _userRepo.GetByIdAsync(currentTenantId, currentUserId, ct);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var oldMfaEnabled = user.MfaEnabled;

            user.MfaEnabled = request.Enabled;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                user.TenantId,
                actorEmail,
                "User",
                "Update",
                user.Email,
                user.Id,
                "{" +
                $"\"Result\":\"Success\"," +
                $"\"Event\":\"SelfToggleMfa\"," +
                $"\"OldMfaEnabled\":{oldMfaEnabled.ToString().ToLowerInvariant()}," +
                $"\"NewMfaEnabled\":{user.MfaEnabled.ToString().ToLowerInvariant()}" +
                "}",
                ct);

            return new ToggleMyMfaResponseDto
            {
                UserId = user.Id,
                MfaEnabled = user.MfaEnabled
            };
        }

        public async Task<VerificationCodeResponse> SendRegistrationVerificationCodeAsync(
            Guid tenantId,
            SendVerificationCodeRequest request,
            CancellationToken ct = default)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(tenantId));
            }

            var email = request.EmailAddress.Trim().ToLowerInvariant();
            var phoneNumber = request.PhoneNumber.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address is required.");
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            // Check if email already exists
            var existingUser = await _userRepo.GetByEmailAsync(tenantId, email, ct);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is already registered.");
            }

            // Generate 6-digit code
            var code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            // Check if there's an existing verification code for this email
            var existingCode = await _verificationCodeRepo.GetByEmailAsync(tenantId, email, ct);

            if (existingCode != null)
            {
                // Update existing code
                existingCode.Code = code;
                existingCode.PhoneNumber = phoneNumber;
                existingCode.Verified = false;
                existingCode.AttemptCount = 0;
                existingCode.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
            }
            else
            {
                // Create new verification code entity
                var verificationCode = new RegistrationVerificationCodeEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Code = code,
                    AttemptCount = 0,
                    Verified = false,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    CreatedAt = DateTime.UtcNow
                };

                await _verificationCodeRepo.AddAsync(verificationCode, ct);
            }

            await _verificationCodeRepo.SaveChangesAsync(ct);

            // TODO: Uncomment to send email in production
            // Send email with verification code
            // await _emailSender.SendAsync(
            //     email,
            //     "MOS Platform Registration Verification Code",
            //     $"Your verification code is: {code}\n\nThis code will expire in 10 minutes.",
            //     ct);

            await _auditService.LogAsync(
    tenantId,
    "System",
    "User",
    "Update",
    email,
    null,
    "{\"Result\":\"Success\",\"Event\":\"RegistrationCodeGenerated\",\"CodeSentVia\":\"Database\"}",
    ct);

            return new VerificationCodeResponse
            {
                Success = true,
                Message = "Verification code stored in database (for testing).",
                CodeSent = "database",
                Code=code,
            };
        }

        public async Task<VerificationCodeResponse> VerifyRegistrationCodeAsync(
            Guid tenantId,
            VerifyRegistrationCodeRequest request,
            CancellationToken ct = default)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId is required.", nameof(tenantId));
            }

            var email = request.EmailAddress.Trim().ToLowerInvariant();
            var code = request.VerificationCode.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address is required.");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Verification code is required.");
            }

            // Get valid verification code
            var verificationCode = await _verificationCodeRepo.GetValidCodeAsync(tenantId, email, code, ct);

            if (verificationCode == null)
            {
                verificationCode = await _verificationCodeRepo.GetByEmailAsync(tenantId, email, ct);

                if (verificationCode != null)
                {
                    verificationCode.AttemptCount++;
                    await _verificationCodeRepo.SaveChangesAsync(ct);

                    // Lock after 5 failed attempts
                    if (verificationCode.AttemptCount >= 5)
                    {
                        await _auditService.LogAsync(
                            tenantId,
                            "System",
                            "RegistrationVerification",
                            "VerifyCode",
                            email,
                            null,
                            "{\"Result\":\"Failed\",\"Reason\":\"TooManyAttempts\"}",
                            ct);

                        throw new InvalidOperationException("Too many verification attempts. Please request a new code.");
                    }
                }

                await _auditService.LogAsync(
                    tenantId,
                    "System",
                    "RegistrationVerification",
                    "VerifyCode",
                    email,
                    null,
                    "{\"Result\":\"Failed\",\"Reason\":\"InvalidOrExpiredCode\"}",
                    ct);

                throw new InvalidOperationException("Invalid or expired verification code.");
            }

            // Mark as verified
            verificationCode.Verified = true;
            await _verificationCodeRepo.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                tenantId,
                "System",
                "RegistrationVerification",
                "VerifyCode",
                email,
                null,
                "{\"Result\":\"Success\"}",
                ct);

            return new VerificationCodeResponse
            {
                Success = true,
                Message = "Email verified successfully."
            };
        }
    }
}
