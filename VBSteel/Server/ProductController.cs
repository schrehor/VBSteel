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
    //[Authorize(Roles = "Admin")]
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
			    var imageType = Path.GetExtension(p.ImagePath).Replace(".", "");
			    var imageData = System.IO.File.ReadAllBytes(p.ImagePath);
			    return new ProductViewModel()
			    {
				    Name = p.Name,
				    ImageData = imageData,
				    ImageType = imageType
			    };
		    }).ToList();

		    return Ok(listOfProductViews);
	    }

	    return BadRequest("A problem loading products occurred");
    }

	[HttpGet("getImage/{id}")]
	public async Task<IActionResult> GetImage(Guid id)
	{
		var product = await _databaseContext.Products.FindAsync(id);
		if (product == null)
		{
			return NotFound();
		}

		var imageType = Path.GetExtension(product.ImagePath).Replace(".", "");
		var imageBytes = await System.IO.File.ReadAllBytesAsync(product.ImagePath);

		var contentType = imageType switch
		{
			"jpeg" => "image/jpeg",
			"jpg" => "image/jpeg",
			"png" => "image/png",
			"webp" => "image/webp",
			_ => "image/jpeg"
		};

		return File(imageBytes, contentType);
	}


}