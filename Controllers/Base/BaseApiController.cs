using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OfficeTicketSystemBackend.Controllers.Base
{
	[ApiController]
	public class BaseApiController : ControllerBase
	{
		protected string LoggedInUserUserId => User.FindFirstValue("LoggedUserUserId");
		protected string LoggedInUserUserName => User.FindFirstValue(ClaimTypes.NameIdentifier);
		protected string LoggedInUserRole => User.FindFirstValue(ClaimTypes.Role);
	}
}
