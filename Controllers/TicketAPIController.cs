using BLL.Interfaces;
using Entities.Common;
using Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeTicketSystemBackend.Controllers.Base;
using System.Security.Claims;

namespace OfficeTicketSystemBackend.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	public class TicketAPIController : BaseApiController
	{
		private readonly ITicketService _ticketService;

		public TicketAPIController(ITicketService ticketService)
		{
			_ticketService = ticketService;
		}

		[HttpPost("raise")]
		public async Task<ApiResponse> RaiseTicket(CreateTicketDto dto)
		{
			ApiResponse response = new ApiResponse();
			try
			{
				var userId = LoggedInUserUserId;

				await _ticketService.RaiseTicket(dto, userId);

				response.IsSuccess = true;
				response.Message = "Ticket raised successfully";

				return response;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;

				return response;
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetMyTickets(int page = 1, int pageSize = 10, string? search = null)
		{
			var userId = LoggedInUserUserId;
			var role = LoggedInUserRole;

			var result = await _ticketService.GetMyTicketsAsync(
				userId, role, page, pageSize, search);

			return Ok(result);
		}

		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAllTickets(int page = 1, int pageSize = 10, string? search = null)
		{
			var result = await _ticketService.GetAllTicketsAsync(page, pageSize, search);

			return Ok(result);
		}

		[HttpPost("assign-ticket")]
		public async Task<IActionResult> AssignTicket([FromBody] AssignTicketDto model)
		{
			var userRole = LoggedInUserRole;

			if (!string.Equals(userRole, "SuperAdmin", StringComparison.OrdinalIgnoreCase))
				return Forbid("Only SuperAdmin can assign tickets");

			var response = await _ticketService.AssignTicketAsync(model);

			if (!response.IsSuccess)
				return BadRequest(new { message = response.Message });

			return Ok(new { message = response.Message });
		}

		[HttpGet("admins")]
		public async Task<IActionResult> GetAdmins()
		{
			var userRole = LoggedInUserRole;

			if (!string.Equals(userRole, "SuperAdmin", StringComparison.OrdinalIgnoreCase))
				return Forbid("Only SuperAdmin can view Admins");

			var admins = await _ticketService.GetAllAdminsAsync();

			return Ok(admins);
		}

		[HttpPut("resolve/{ticketId}")]
		public async Task<IActionResult> ResolveTicket(int ticketId)
		{
			var result = await _ticketService.ResolveTicketAsync(ticketId); // backend marks as Resolved
			if (result)
				return Ok(new { message = "Ticket marked as Resolved" });
			else
				return BadRequest(new { message = "Failed to update status" });
		}

	}
}
