using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Seed
{
	public static class IdentitySeeder
	{
		public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
		{
			try
			{
				var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

				string[] roles = { "SuperAdmin", "Admin", "Employee" };

				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
					{
						await roleManager.CreateAsync(new IdentityRole(role));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Role seeding failed: " + ex.Message);
			}
		}
	}
}
