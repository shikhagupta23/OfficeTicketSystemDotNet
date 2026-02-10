using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
	public interface ITicketRepository : IGenericRepository<Ticket>
	{
		IQueryable<Ticket> GetMyTicketsQueryable(string userId,	string? search);
		IQueryable<Ticket> GetTicketsQueryable(string? search);
	}

}
