using Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace BLL.Extensions
{
	public static class QueryableExtensions
	{
		public static async Task<PagedResponse<T>> ToPagedResultAsync<T>(
			this IQueryable<T> query,
			int page,
			int pageSize)
		{
			page = page <= 0 ? 1 : page;
			pageSize = pageSize <= 0 ? 10 : pageSize;

			var totalRecords = await query.CountAsync();

			var data = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResponse<T>
			{
				PageNumber = page,
				PageSize = pageSize,
				TotalCount = totalRecords,
				DataList = data,
				IsSuccess = true
				
			};
		}
	}
}
