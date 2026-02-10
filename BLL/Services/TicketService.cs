using BLL.Extensions;
using BLL.Interfaces;
using DAL.UnitOfWork;
using Entities.Common;
using Entities.DTO;
using Entities.Enum;
using Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
	public class TicketService : ITicketService
	{
		private readonly IUnitOfWork _unitOfWork;

		public TicketService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task RaiseTicket(CreateTicketDto dto, string userId)
		{
			var ticket = new Ticket
			{
				Title = dto.Title,
				Description = dto.Description,
				Priority = dto.Priority,
				Status = TicketStatus.Open,
				CreatedBy = userId,
				AssignedTo = dto.AssignedTo == string.Empty ? Guid.Empty : Guid.Parse(dto.AssignedTo)
			};

			await _unitOfWork.Tickets.AddAsync(ticket);
			await _unitOfWork.CompleteAsync();
		}

		public async Task<PagedResponse<TicketResponseDto>> GetMyTicketsAsync(
			string userId, string role, int page, int pageSize, string? search)
		{
			IQueryable<Ticket> query;

			if (role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
			{
				query = _unitOfWork.Tickets.GetTicketsQueryable(search)
					.Where(t => t.CreatedBy == userId);
			}
			else if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
			{
				query = _unitOfWork.Tickets.GetTicketsQueryable(search)
					.Where(t => t.AssignedTo == Guid.Parse(userId));
			}
			else
			{
				query = _unitOfWork.Tickets.GetTicketsQueryable(search);
			}

			var result = query
				.OrderByDescending(t => t.CreatedAt)
				.Select(t => new TicketResponseDto
				{
					Id = t.Id,
					Title = t.Title,
					Description = t.Description,
					Status = t.Status,
					Priority = t.Priority,
					CreatedDate = t.CreatedAt
				});

			return await result.ToPagedResultAsync(page, pageSize);
		}

		public async Task<PagedResponse<TicketResponseDto>> GetAllTicketsAsync(int page = 1, int pageSize = 10, string? search = null)
		{
			var query = _unitOfWork.Tickets
				.GetTicketsQueryable(search)
				.OrderByDescending(t => t.CreatedAt)
				.Select(t => new TicketResponseDto
				{
					Id = t.Id,
					Title = t.Title,
					Description = t.Description,
					Status = t.Status,
					Priority = t.Priority,
					CreatedDate = t.CreatedAt
				});

			return await query.ToPagedResultAsync(page, pageSize); ;
		}

		public async Task<ApiResponse> AssignTicketAsync(AssignTicketDto dto)
		{
			var ticket = _unitOfWork.Tickets.GetTicketsQueryable(null)
				.FirstOrDefault(t => t.Id == dto.TicketId);

			if (ticket == null)
				return new ApiResponse { IsSuccess = false, Message = "Ticket not found" };

			if (ticket.AssignedTo != Guid.Empty)
				return new ApiResponse { IsSuccess = false, Message = "Ticket is already assigned" };

			ticket.AssignedTo = Guid.Parse(dto.AdminId);
			ticket.Status = TicketStatus.InProgress;

			_unitOfWork.Tickets.Update(ticket); 
			await _unitOfWork.CompleteAsync();

			return new ApiResponse { IsSuccess = true, Message = "Ticket assigned successfully" };
		}

		public async Task<List<UserResponseDto>> GetAllAdminsAsync()
		{
			var adminRoleId = _unitOfWork.Users.GetRolesQueryable()
				.Where(r => r.Name == "Admin")
				.Select(r => r.Id)
				.FirstOrDefault();

			if (string.IsNullOrEmpty(adminRoleId))
				return new List<UserResponseDto>();

			var admins = (from ur in _unitOfWork.Users.GetUserRolesQueryable()
						  join u in _unitOfWork.Users.GetUsersQueryable() on ur.UserId equals u.Id
						  where ur.RoleId == adminRoleId
						  select new UserResponseDto
						  {
							  Id = u.Id,
							  UserName = u.UserName
						  }).ToList();

			return admins;
		}

		public async Task<bool> ResolveTicketAsync(int ticketId)
		{
			var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
			if (ticket == null) return false;

			ticket.Status = TicketStatus.Resolved;
			await _unitOfWork.CompleteAsync();
			return true;
		}


	}

}
