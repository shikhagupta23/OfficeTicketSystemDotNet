namespace Entities.Common
{
	public class ApiResponse
	{
		public bool IsSuccess { get; set; }
		public string Message { get; set; }
		public string Id { get; set; } = string.Empty;
	}

	public class ApiResponse<T> : ApiResponse
	{
		public T Data { get; set; }
		public List<T> DataList { get; set; } = new List<T>();
	}

	public class PagedResponse<T> : ApiResponse
	{
		public List<T> DataList { get; set; } = new List<T>();
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public int TotalPages =>
			PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
	}
}
