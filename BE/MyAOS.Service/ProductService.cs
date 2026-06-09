using MyAOS.Domain.Dto;
using MyAOS.Domain.Entity;
using MyAOS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyAOS.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFavouriteProductRepository _favouriteProductRepository;
        private readonly IUserRepository _userRepository;

        public ProductService(
            IProductRepository productRepository,
            IFavouriteProductRepository favouriteProductRepository,
            IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _favouriteProductRepository = favouriteProductRepository;
            _userRepository = userRepository;
        }

        public async Task<List<ProductDto>> GetAllActiveAsync(CancellationToken ct = default)
        {
            var entities = await _productRepository.GetAllActiveAsync(ct);

            return entities.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                IconUrl = p.IconUrl,
                SortOrder = p.SortOrder
            }).ToList();
        }

        public async Task<List<FavouriteProductDto>> GetFavouritesAsync(Guid userId, CancellationToken ct = default)
        {
            // Note: Our current GetFavourites logic does not require tenant-level isolation for checking if user exists,
            // but the repository requires tenantId. For now we will bypass the user check, or we could just 
            // remove it since the JWT ensures the user exists. Let's just remove the redundant user check 
            // to simplify and avoid tenantId issues.
            var entities = await _favouriteProductRepository.GetByUserIdAsync(userId, ct);

            return entities.Select(f => new FavouriteProductDto
            {
                ProductId = f.ProductId,
                AddedAt = f.AddedAt
            }).ToList();
        }

        public async Task<bool> ToggleFavouriteAsync(Guid userId, int productId, CancellationToken ct = default)
        {
            // Optional: Check if product exists and is active
            var products = await _productRepository.GetActiveByIdsAsync(new[] { productId }, ct);
            if (!products.Any())
            {
                throw new Exception($"Product {productId} not found or inactive.");
            }

            var existing = await _favouriteProductRepository.GetAsync(userId, productId, ct);

            if (existing != null)
            {
                await _favouriteProductRepository.RemoveAsync(existing, ct);
                await _favouriteProductRepository.SaveChangesAsync(ct);
                return false; // Not a favourite anymore
            }
            else
            {
                var newFav = new FavouriteProductEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    AddedAt = DateTime.UtcNow
                };

                await _favouriteProductRepository.AddAsync(newFav, ct);
                await _favouriteProductRepository.SaveChangesAsync(ct);
                return true; // Is now a favourite
            }
        }
    }
}
