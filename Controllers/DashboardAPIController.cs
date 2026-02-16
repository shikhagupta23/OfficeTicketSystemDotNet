using DAL.UnitOfWork;
using Entities.DTO;
using Entities.Enum;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeTicketSystemBackend.Controllers.Base;

[Authorize]
[Route("api/dashboard")]
public class DashboardAPIController : BaseApiController
{
	private readonly IUnitOfWork _unitOfWork;

	public DashboardAPIController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	[HttpGet("stats")]
	public IActionResult GetDashboardStats()
	{
		var role = LoggedInUserRole;
		var userId = Guid.Parse(LoggedInUserUserId);

		IQueryable<Ticket> query = _unitOfWork.Tickets.GetTicketsQueryable(null);

		if (role == "Admin")
		{
			query = query.Where(t => t.AssignedTo.HasValue &&
									 t.AssignedTo != Guid.Empty && 
									 t.AssignedTo == userId);
		}
		else if (role == "Employee")
		{
			query = query.Where(t => t.CreatedBy == userId.ToString());
		}

		var result = new DashboardStatsDto
		{
			Total = query.Count(),
			Open = query.Count(t => t.Status == TicketStatus.Open),
			InProgress = query.Count(t => t.Status == TicketStatus.InProgress),
			Resolved = query.Count(t => t.Status == TicketStatus.Resolved),
			Closed = query.Count(t => t.Status == TicketStatus.Closed)
		};

		return Ok(result);
	}
}
