using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;
public class ProductInputModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(1000)]
    public string Description { get; set; }

    [Required]
    public byte[] ImageData { get; set; }

    [Required]
    [RegularExpression("(jpeg|jpg|png|webp)", ErrorMessage = "The image type must be jpeg, jpg, png, or webp.")]
    public string ImageType { get; set; }
}

