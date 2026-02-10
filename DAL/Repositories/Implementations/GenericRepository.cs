using DAL.Context;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Implementations
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected readonly ApplicationDbContext _context;

		public GenericRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<T>> GetAllAsync()
			=> await _context.Set<T>().ToListAsync();

		public async Task<T> GetByIdAsync(int id)
			=> await _context.Set<T>().FindAsync(id);

		public async Task AddAsync(T entity)
			=> await _context.Set<T>().AddAsync(entity);

		public void Update(T entity)
			=> _context.Set<T>().Update(entity);

		public void Delete(T entity)
			=> _context.Set<T>().Remove(entity);
	}

}
