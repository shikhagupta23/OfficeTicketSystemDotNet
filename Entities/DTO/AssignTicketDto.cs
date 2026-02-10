using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
	public class AssignTicketDto
	{
		public int TicketId { get; set; }
		public string AdminId { get; set; } = null!;
	}
}
