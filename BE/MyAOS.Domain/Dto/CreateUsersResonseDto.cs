using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class CreateUsersResponseDto
    {
        public List<CreatedUserDto> CreatedUsers { get; set; } = new();
    }

    public class CreatedUserDto
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string TemporaryPassword { get; set; } = string.Empty;

        public List<int> ProductIds { get; set; } = new();
    }
}
