using Microsoft.EntityFrameworkCore;

namespace VBSteel.Server;

public class DatabaseContext : DbContext
{
	public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
	{
	}

	public DbSet<FormData> FormData { get; set; }
}