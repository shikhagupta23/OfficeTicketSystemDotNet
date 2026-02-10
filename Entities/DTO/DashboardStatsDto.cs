using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
	public class DashboardStatsDto
	{
		public int Total { get; set; }
		public int Open { get; set; }
		public int InProgress { get; set; }
		public int Resolved { get; set; }
		public int Closed { get; set; }
	}
}
