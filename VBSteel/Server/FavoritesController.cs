using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VBSteel.Shared;

namespace VBSteel.Server;

//[Authorize(Roles = "Admin,RegularUser")]
[Route("api/[controller]")]
[ApiController]
public class FavoritesController : ControllerBase
{
    private readonly DatabaseContext _context;

    public FavoritesController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet("IsFavorite")]
    public async Task<ActionResult<bool>> IsFavorite(Guid userId, Guid productId)
    {
        return await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId);
    }

    [HttpPost("Add")]
    public async Task<ActionResult> Add(Favorite favorite)
    {
        if (await _context.Favorites.AnyAsync(f => f.UserId == favorite.UserId && f.ProductId == favorite.ProductId))
        {
            return BadRequest("The product is already a favorite.");
        }

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("Delete")]
    public async Task<ActionResult> Delete(Guid userId, Guid productId)
    {
        var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        if (favorite == null)
        {
            return NotFound("The product is not a favorite.");
        }

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("GetUserFavorites")]
    public async Task<ActionResult<List<Product>>> GetUserFavorites([FromBody] Guid userId)
    {
        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Product)
            .Select(f => f.Product)
            .ToListAsync();

        return Ok(favorites);
    }

    [HttpGet("GetFavoriteCounts")]
    public async Task<ActionResult<List<ProductFavoriteCount>>> GetFavoriteCounts()
    {
        var favoriteCounts = await _context.Favorites
            .Include(f => f.Product)
            .GroupBy(f => new { f.ProductId, f.Product.Name })
            .Select(g => new ProductFavoriteCount { ProductId = g.Key.ProductId, ProductName = g.Key.Name, Count = g.Count() })
            .ToListAsync();

        return Ok(favoriteCounts);
    }
}

