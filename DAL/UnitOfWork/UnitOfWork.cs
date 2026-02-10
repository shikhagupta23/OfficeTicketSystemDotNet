using DAL.Context;
using DAL.Repositories.Implementations;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;

		public ITicketRepository Tickets { get; }
		public IUserRepository Users { get; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			Tickets = new TicketRepository(context);
			Users = new UserRepository(context);
		}

		public async Task<int> CompleteAsync()
			=> await _context.SaveChangesAsync();

		public void Dispose()
			=> _context.Dispose();
	}

}
