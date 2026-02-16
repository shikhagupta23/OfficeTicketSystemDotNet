using BLL.Extensions;
using BLL.Interfaces;
using DAL.UnitOfWork;
using Entities.Common;
using Entities.DTO;
using Entities.Enum;
using Entities.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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
		private readonly IEmailService _emailService;


		public TicketService(IUnitOfWork unitOfWork, IEmailService emailService)
		{
			_unitOfWork = unitOfWork;
			_emailService = emailService;
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

			try
			{
				var superAdminRoleId = _unitOfWork.Users.GetRolesQueryable()
					.Where(r => r.Name == "SuperAdmin")
					.Select(r => r.Id)
					.FirstOrDefault();

				if (!string.IsNullOrEmpty(superAdminRoleId))
				{
					var superAdmins = (from ur in _unitOfWork.Users.GetUserRolesQueryable()
									   join u in _unitOfWork.Users.GetUsersQueryable()
									   on ur.UserId equals u.Id
									   where ur.RoleId == superAdminRoleId
									   select u.Email).ToList();

					foreach (var email in superAdmins)
					{
						string body = $@"
                    <h3>New Ticket Raised</h3>
                    <p><b>Title:</b> {ticket.Title}</p>
                    <p><b>Description:</b> {ticket.Description}</p>
                    <p><b>Priority:</b> {ticket.Priority}</p>
                ";

						await _emailService.SendEmailAsync(email, "New Ticket Raised", body);
					}
				}
			}
			catch { }
		}

		public async Task<PagedResponse<TicketResponseDto>> GetMyTicketsAsync(
			string userId, string role, int page, int pageSize, string? search)
		{
			var tickets = _unitOfWork.Tickets.GetTicketsQueryable(search);
			var users = _unitOfWork.Users.GetUsersQueryable();

			// Apply role filter
			if (role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
			{
				tickets = tickets.Where(t => t.CreatedBy == userId);
			}
			else if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
			{
				var adminGuid = Guid.Parse(userId);
				tickets = tickets.Where(t => t.AssignedTo == adminGuid);
			}

			var result = tickets
				.OrderByDescending(t => t.CreatedAt)
				.Select(t => new TicketResponseDto
				{
					Id = t.Id,
					Title = t.Title,
					Description = t.Description,
					Status = t.Status,
					Priority = t.Priority,
					CreatedDate = t.CreatedAt,

					CreatedByName = users
						.Where(u => u.Id == t.CreatedBy)
						.Select(u => u.FirstName + " " + u.LastName)
						.FirstOrDefault(),

					AssignedToName = users
						.Where(u => u.Id == t.AssignedTo.ToString())
						.Select(u => u.FirstName + " " + u.LastName)
						.FirstOrDefault()
				});

			return await result.ToPagedResultAsync(page, pageSize);
		}

		public async Task<PagedResponse<TicketResponseDto>> GetTicketsHistoryAsync(int page = 1, int pageSize = 10, string? search = null)
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

			try
			{
				var admin = await _unitOfWork.Users.GetUsersQueryable()
					.Where(u => u.Id == dto.AdminId)
					.Select(u => u.Email)
					.FirstOrDefaultAsync();

				if (!string.IsNullOrEmpty(admin))
				{
					string body = $@"
                <h3>Ticket Assigned To You</h3>
                <p><b>Title:</b> {ticket.Title}</p>
                <p><b>Description:</b> {ticket.Description}</p>
            ";

					await _emailService.SendEmailAsync(admin, "Ticket Assigned", body);
				}
			}
			catch { }


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

			try
			{
				// Employee Email
				var employeeEmail = _unitOfWork.Users.GetUsersQueryable()
					.Where(u => u.Id == ticket.CreatedBy)
					.Select(u => u.Email)
					.FirstOrDefault();

				string body = $@"
            <h3>Ticket Resolved</h3>
            <p><b>Title:</b> {ticket.Title}</p>
            <p>Your issue has been resolved.</p>
        ";

				if (!string.IsNullOrEmpty(employeeEmail))
					await _emailService.SendEmailAsync(employeeEmail, "Ticket Resolved", body);

				// SuperAdmin Email
				var superAdminRoleId = _unitOfWork.Users.GetRolesQueryable()
					.Where(r => r.Name == "SuperAdmin")
					.Select(r => r.Id)
					.FirstOrDefault();

				var superAdmins = (from ur in _unitOfWork.Users.GetUserRolesQueryable()
								   join u in _unitOfWork.Users.GetUsersQueryable()
								   on ur.UserId equals u.Id
								   where ur.RoleId == superAdminRoleId
								   select u.Email).ToList();

				foreach (var email in superAdmins)
				{
					await _emailService.SendEmailAsync(email, "Ticket Resolved", body);
				}
			}
			catch { }

			return true;
		}
	}

}
