using Microsoft.EntityFrameworkCore;
using VBSteel.Shared;

namespace VBSteel.Server;

public class DatabaseContext : DbContext
{
	public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
	{
	}

	public DbSet<User> Users { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Favorite> Favorites { get; set; }
	public DbSet<Form> Forms { get; set; }
}