using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class DeleteUsersRequest
    {
        public List<Guid> UserIds { get; set; } = new();
    }
}
