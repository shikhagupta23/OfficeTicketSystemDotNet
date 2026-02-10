using DAL.Context;
using DAL.Repositories.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Implementations
{
	public class TicketRepository
		: GenericRepository<Ticket>, ITicketRepository
	{
		public TicketRepository(ApplicationDbContext context)
			: base(context)
		{
		}

		public IQueryable<Ticket> GetMyTicketsQueryable(
			string userId,
			string? search)
		{
			var query = _context.Tickets
				.Where(t => t.CreatedBy == userId);

			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(t =>
					t.Title.Contains(search) ||
					t.Description.Contains(search));
			}

			return query;
		}
		public IQueryable<Ticket> GetTicketsQueryable(string? search)
		{
			var query = _context.Tickets.AsQueryable();

			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(t =>
					t.Title.Contains(search) ||
					t.Description.Contains(search) || 
					t.Id.ToString().Contains(search));
			}

			return query;
		}
	}
}
