using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VBSteel.Shared;

namespace VBSteel.Server;

[ApiController]
[Route("api/[controller]")]
public class ProductController : Controller
{
    private readonly DatabaseContext _databaseContext;
    private readonly string _imagePath;

    public ProductController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        if (!Directory.Exists(_imagePath))
        {
            Directory.CreateDirectory(_imagePath);
        }
    }

    [HttpPost("CreateProduct")]
    public async Task<IActionResult> CreateProduct(ProductInputModel inputModel)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized("Invalid user ID.");
        }

        if (roleClaim != "Admin")
        {
            return Unauthorized("Only admins can create products.");
        }

        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        var fileName = $"{inputModel.Name}.{inputModel.ImageType}";
        var filePath = Path.Combine(_imagePath, fileName);
        await System.IO.File.WriteAllBytesAsync(filePath, inputModel.ImageData);

        var product = new Product
        {
            ProductId = Guid.NewGuid(),
            UserId = userId,
            Name = inputModel.Name,
            Description = inputModel.Description,
            ImagePath = filePath
        };

        _databaseContext.Products.Add(product);
        await _databaseContext.SaveChangesAsync();

        return Ok(product);

    }

    [HttpGet("productView")]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProductView()
    {
        var allProducts = await _databaseContext.Products.ToListAsync();
        if (allProducts.Count > 0)
        {
            var listOfProductViews = allProducts.Select(p =>
            {
                var imageBytes = System.IO.File.ReadAllBytes(p.ImagePath);
                var imageType = Path.GetExtension(p.ImagePath).Replace(".", "");
                return new ProductViewModel()
                {
                    Name = p.Name,
                    ImageData = Convert.ToBase64String(imageBytes),
                    ImageType = imageType
                };
            }).ToList();

            return Ok(listOfProductViews);
        }

        return BadRequest("A problem loading products occured");
    }
}