using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
	public class CreateTicketDto
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public TicketPriority Priority { get; set; }
		public TicketCategory Category { get; set; }
		public string AssignedTo { get; set; } = string.Empty;
	}

}
