using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAOS.Domain.Dto;
using MyAOS.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyAOS.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET /api/products
        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken ct)
        {
            var products = await _productService.GetAllActiveAsync(ct);
            return Ok(products);
        }

        // GET /api/products/favourites
        [HttpGet("favourites")]
        public async Task<IActionResult> GetFavourites(CancellationToken ct)
        {
            var userIdValue =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            if (!Guid.TryParse(userIdValue, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user." });
            }

            var favourites = await _productService.GetFavouritesAsync(currentUserId, ct);
            return Ok(favourites);
        }

        // POST /api/products/favourites/{id}/toggle
        [HttpPost("favourites/{id:int}/toggle")]
        public async Task<IActionResult> ToggleFavourite(int id, CancellationToken ct)
        {
            var userIdValue =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            if (!Guid.TryParse(userIdValue, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user." });
            }

            try
            {
                var isFavourite = await _productService.ToggleFavouriteAsync(currentUserId, id, ct);
                return Ok(new { ProductId = id, IsFavourite = isFavourite });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
