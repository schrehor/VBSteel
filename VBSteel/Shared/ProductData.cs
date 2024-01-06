using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;

public class Product
{
    [Key]
    public Guid ProductId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [StringLength(255)]
    public string ImagePath { get; set; }

    [Required, StringLength(1000)]
    public string Description { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; }

    public User User { get; set; }
}
