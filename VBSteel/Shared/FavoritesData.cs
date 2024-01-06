using System.ComponentModel.DataAnnotations;

namespace VBSteel.Shared;

public class Favorite
{
    [Key]
    public Guid FavoriteId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    public User User { get; set; }
    public Product Product { get; set; }
}
