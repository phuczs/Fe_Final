using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Entity
{
    public sealed class UserProductEntity
    {
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime GrantedAt { get; set; }
        public Guid? GrantedBy { get; set; }

        // Navigation
        public UserEntity User { get; set; } = null!;
        public ProductEntity Product { get; set; } = null!;
    }
}
