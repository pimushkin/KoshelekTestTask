using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KoshelekTestTask.Api.Data;
using KoshelekTestTask.Api.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KoshelekTestTask.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMessageController : ControllerBase
    {
        private readonly IHubContext<MessageHub> _hubContext;
        public SendMessageController(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }
        // POST api/<SendMessageController>
        [HttpPost]
        public async Task<ActionResult<Message>> Post(Message message)
        {
            if (message == null)
            {
                return BadRequest();
            }

            if (message.Content.Length > 128)
            {
                return BadRequest("The maximum length of a sent message must not exceed 128 characters.");
            }

            if (message.Content.Length == 0)
            {
                return BadRequest("The message cannot be empty.");
            }

            TimeZoneInfo moscowTimeZone;
            if (Environment.OSVersion.ToString().Contains("Microsoft") || Environment.OSVersion.ToString().Contains("Windows"))
            {
                // works on Windows only
                moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            }
            else
            {
                // works on Linux only
                moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
            }
            var moscowDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);
            await _hubContext.Clients.All.SendAsync("Send", message.SerialNumber, message.Content, moscowDateTime.ToString(CultureInfo.InvariantCulture));
            return Ok(message);
        }
    }
}
