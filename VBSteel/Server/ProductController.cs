using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VBSteel.Shared;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class ProductController(DatabaseContext databaseContext)  : Controller
{
    
    [HttpGet("productView")]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProductView()
    {
        var allProducts = await databaseContext.Products.ToListAsync();
        if (allProducts.Count > 0)
        {
            var listOfProductViews = allProducts.Select(p => new ProductViewModel()
            {
                Name = p.Name,
                ImagePath = p.ImagePath
            }).ToList();
            
            return Ok(listOfProductViews);
        }

        return BadRequest("A problem loading products occured");
    }
}