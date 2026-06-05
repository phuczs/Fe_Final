using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class AssignUserProductsResponseDto
    {
        public Guid UserId { get; set; }

        public List<int> ProductIds { get; set; } = new();
    }
}
