namespace OfficeTicketSystemBackend.Entities.DTO
{
	public class EmailSettingsDto
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public string SenderName { get; set; }
		public string SenderDefaultEmail { get; set; }
		public string Password { get; set; }
	}
}
