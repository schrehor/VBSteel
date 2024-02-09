using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;
public class ProductInputModel
{
    [Required]
    [StringLength(100, ErrorMessage = "The product name must be between 3 and 100 characters.", MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "The product description must be between 10 and 1000 characters.", MinimumLength = 10)]
    public string Description { get; set; }

    [Required]
    public byte[] ImageData { get; set; }

    [Required]
    [RegularExpression("(jpeg|jpg|png|webp)", ErrorMessage = "The image type must be jpeg, jpg, png, or webp.")]
    public string ImageType { get; set; }
}

