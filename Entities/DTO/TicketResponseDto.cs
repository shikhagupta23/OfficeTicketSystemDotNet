using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
	public class TicketResponseDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public TicketStatus Status { get; set; }
		public TicketPriority Priority { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedByName { get; set; } = string.Empty;
		public string AssignedToName { get; set; } = string.Empty;

	}

}
