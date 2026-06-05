using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class GetAuditLogsQuery
    {
        public string? Search { get; set; }

        public string? ObjectType { get; set; }

        public string? TargetName { get; set; }

        public Guid? TargetUserId { get; set; }

        public string SortDirection { get; set; } = "desc";

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
