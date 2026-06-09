using MyAOS.Domain.Entity;
using System;
using System.Collections.Generic;

namespace MyAOS.Repository
{
    public interface IFavouriteProductRepository
    {
        Task<List<FavouriteProductEntity>> GetByUserIdAsync(
            Guid userId,
            CancellationToken ct = default);

        Task<FavouriteProductEntity?> GetAsync(
            Guid userId,
            int productId,
            CancellationToken ct = default);

        Task AddAsync(FavouriteProductEntity entity, CancellationToken ct = default);

        Task RemoveAsync(FavouriteProductEntity entity, CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
