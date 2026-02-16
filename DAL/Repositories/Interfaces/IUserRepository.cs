using Microsoft.AspNetCore.Identity;
using Entities.DTO;
using System.Linq;
using Entities.Identity;

namespace DAL.Repositories.Interfaces
{
	public interface IUserRepository
	{
		IQueryable<IdentityRole> GetRolesQueryable();
		IQueryable<IdentityUserRole<string>> GetUserRolesQueryable();
		IQueryable<ApplicationUser> GetUsersQueryable();
	}
}
