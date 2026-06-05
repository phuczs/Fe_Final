using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class DeactivateUsersRequest
    {
        public List<Guid> UserIds { get; set; } = new();
    }
}
