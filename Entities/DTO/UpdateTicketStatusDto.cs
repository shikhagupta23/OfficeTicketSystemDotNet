using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
	public class UpdateTicketStatusDto
	{
		public int TicketId { get; set; }
		public TicketStatus Status { get; set; }
	}

}
