using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class ProductEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }
}
