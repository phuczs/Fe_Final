using MyAOS.Domain.Dto;
using System;
using System.Collections.Generic;

namespace MyAOS.Service
{
    public interface IProductService
    {
        /// <summary>Returns all active products from the catalogue (fixed list).</summary>
        Task<List<ProductDto>> GetAllActiveAsync(CancellationToken ct = default);

        /// <summary>Returns the product IDs favourited by the given user.</summary>
        Task<List<FavouriteProductDto>> GetFavouritesAsync(Guid userId, CancellationToken ct = default);

        /// <summary>
        /// Adds the product to favourites if not already present; removes it otherwise.
        /// Returns true if the product is now a favourite, false if it was removed.
        /// </summary>
        Task<bool> ToggleFavouriteAsync(Guid userId, int productId, CancellationToken ct = default);
    }
}
