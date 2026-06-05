using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class AssignUserProductsRequest
    {
        public List<int> ProductIds { get; set; } = new();
    }
}
