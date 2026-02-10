using DAL.Context;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DAL.Repositories.Implementations
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;

		public UserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public IQueryable<IdentityRole> GetRolesQueryable()
			=> _context.Roles; // DbSet<IdentityRole>

		public IQueryable<IdentityUserRole<string>> GetUserRolesQueryable()
			=> _context.UserRoles; // DbSet<IdentityUserRole<string>>

		public IQueryable<IdentityUser> GetUsersQueryable()
			=> _context.Users; // DbSet<IdentityUser>
	}
}
