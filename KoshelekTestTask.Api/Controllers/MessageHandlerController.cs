using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KoshelekTestTask.Api.Data;
using KoshelekTestTask.Api.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace KoshelekTestTask.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageHandlerController : ControllerBase
    {
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageHandlerController(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // POST api/<MessageHandlerController>
        [HttpPost]
        public async Task<ActionResult<Message>> Post(Message message)
        {
            if (message == null) return BadRequest();

            if (message.Content.Length > 128)
                return BadRequest("The maximum length of a sent message must not exceed 128 characters.");

            if (message.Content.Length == 0) return BadRequest("The message cannot be empty.");

            if (message.SerialNumber <= 0) return BadRequest("The message serial number must be greater than 0.");

            TimeZoneInfo moscowTimeZone;
            if (Environment.OSVersion.ToString().Contains("Microsoft") ||
                Environment.OSVersion.ToString().Contains("Windows"))
                // works on Windows only
                moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            else
                // works on Linux only
                moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");

            message.MoscowDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);

            await using var con = new NpgsqlConnection(Program.ConnectionString);
            con.Open();
            await using var cmd = new NpgsqlCommand
            {
                Connection = con,
                CommandText =
                    $"INSERT INTO koshelek (serial_number, content, moscow_date_time) VALUES ({message.SerialNumber}, '{message.Content}', '{message.MoscowDateTime.ToUniversalTime():O}'::timestamp);"
            };
            cmd.ExecuteNonQuery();

            await _hubContext.Clients.All.SendAsync("Send", message.SerialNumber, message.Content,
                message.MoscowDateTime);

            return Ok(message);
        }

        // GET api/<MessageHandlerController>
        [HttpGet]
        public async Task<ActionResult<List<Message>>> Get()
        {
            TimeZoneInfo moscowTimeZone;
            if (Environment.OSVersion.ToString().Contains("Microsoft") ||
                Environment.OSVersion.ToString().Contains("Windows"))
                // works on Windows only
                moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            else
                // works on Linux only
                moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
            var moscowDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);

            await using var con = new NpgsqlConnection(Program.ConnectionString);
            con.Open();
            await using var cmd = new NpgsqlCommand
            {
                Connection = con,
                CommandText =
                    "SELECT serial_number, content, moscow_date_time FROM koshelek " +
                    $"WHERE moscow_date_time > '{moscowDateTime.ToUniversalTime():O}'::timestamp - INTERVAL '10 minute';"
            };
            await using var rdr = await cmd.ExecuteReaderAsync();
            var messages = new List<Message>();
            while (rdr.Read())
                messages.Add(new Message
                {
                    SerialNumber = rdr.GetInt32(0), Content = rdr.GetString(1),
                    MoscowDateTime = (DateTime) rdr.GetTimeStamp(2)
                });

            return Ok(messages);
        }
    }
}