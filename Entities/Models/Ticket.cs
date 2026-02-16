using Entities.Base;
using Entities.Enum;
using Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
	public class Ticket : BaseEntity
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public TicketStatus Status { get; set; } = TicketStatus.Open;
		public TicketPriority Priority { get; set; } = TicketPriority.Medium;
		public TicketCategory Category { get; set; } = TicketCategory.IT;
		public Guid? AssignedTo { get; set; } = Guid.Empty;
		public string CreatedBy { get; set; } = string.Empty;

	}
}
