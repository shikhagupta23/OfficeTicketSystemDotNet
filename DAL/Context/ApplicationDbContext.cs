using Entities.Identity;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context
{
	public class ApplicationDbContext
		: IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<Ticket> Tickets { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<IdentityRole>()
				.Property(r => r.ConcurrencyStamp)
				.HasColumnType("TEXT");

			builder.Entity<ApplicationUser>()
				.Property(u => u.ConcurrencyStamp)
				.HasColumnType("TEXT");

		}
	}
}
