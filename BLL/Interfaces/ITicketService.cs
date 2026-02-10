using Entities.Common;
using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
	public interface ITicketService
	{
		Task RaiseTicket(CreateTicketDto dto, string userId);
		Task<PagedResponse<TicketResponseDto>> GetMyTicketsAsync(string userId, string role, int page, int pageSize, string? search);
		Task<PagedResponse<TicketResponseDto>> GetAllTicketsAsync(int page = 1, int pageSize = 10, string? search = null);
		Task<ApiResponse> AssignTicketAsync(AssignTicketDto dto);
		Task<List<UserResponseDto>> GetAllAdminsAsync();
		Task<bool> ResolveTicketAsync(int ticketId);
	}
}
