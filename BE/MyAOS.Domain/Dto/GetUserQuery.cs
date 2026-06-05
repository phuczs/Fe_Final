using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class GetUsersQuery
    {
        public string? Search { get; set; }

        public string? Role { get; set; }

        public string? Status { get; set; }

        public string? SignInMethod { get; set; }

        public int? ProductId { get; set; }

        public string SortBy { get; set; } = "displayName";

        public string SortDirection { get; set; } = "asc";

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public Guid? TenantId { get; set; }
    }
}
