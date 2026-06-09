using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Repository
{
    public interface IProductRepository
    {
        Task<List<ProductEntity>> GetAllActiveAsync(CancellationToken ct = default);

        Task<List<ProductEntity>> GetActiveByIdsAsync(
            IEnumerable<int> productIds,
            CancellationToken ct = default);
    }
}
