namespace Entities.Common
{
	public class PagedResult<T>
	{
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalRecords { get; set; }
		public int TotalPages =>
			(int)Math.Ceiling((double)TotalRecords / PageSize);

		public List<T> Data { get; set; } = new();
	}
}
