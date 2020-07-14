using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KoshelekTestTask.Infrastructure.Hubs;
using KoshelekTestTask.Core.Entities;
using KoshelekTestTask.Core.Interfaces;
using KoshelekTestTask.Core.Models;
using KoshelekTestTask.Infrastructure;
using KoshelekTestTask.Infrastructure.Data;
using KoshelekTestTask.Infrastructure.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace KoshelekTestTask.Api.Controllers
{
    /// <summary>
    ///     Responsible for working with messages.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<MessageHub> _hubContext;
        /// <summary>
        ///     MessageController constructor.
        /// </summary>
        /// <param name="hubContext">Hub used for real-time messaging with clients.</param>
        public MessageController(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // POST api/<MessageController>/Send
        /// <summary>
        ///     Save the message in the database and send the message to everyone using the hub.
        /// </summary>
        /// <param name="messageToPost">Message to send.</param>
        /// <returns>Result of sending the message. If the message was sent successfully, it returns the sent message.</returns>
        [HttpPost("Send")]
        public async Task<ActionResult<MessageToPost>> SendMessage(MessageToPost messageToPost)
        {
            IMessageHandler messageHandler = new MessageHandler();
            var moscowDateTime = messageHandler.GetCurrentMoscowTime();
            var message = new Message { Text = messageToPost.Text, SerialNumber = messageToPost.SerialNumber, TimeOfSending = moscowDateTime };
            var valid = messageHandler.CheckValidityOfMessage(message, out var error);
            if (!valid)
            {
                return BadRequest(error);
            }

            IPostgreSqlCommand postgreSqlCommand = new PostgreSqlCommand();
            await postgreSqlCommand.SendMessageAsync(message);

            IMessageDispatcher messageDispatcher = new MessageDispatcher(_hubContext);
            await messageDispatcher.SendMessageToAllUsers(message);

            return Ok(messageToPost);
        }

        // POST api/<MessageController>/FindOverPeriodOfTime
        /// <summary>
        ///     Get a list of all messages that were sent over a certain period of time.
        /// </summary>
        /// <param name="interval">The time period in which to find messages.</param>
        /// <returns>A list of all messages that were found.</returns>
        [HttpPost("FindOverPeriodOfTime")]
        public async Task<ActionResult<List<Message>>> GetMessages(Interval interval)
        {
            IPostgreSqlCommand postgreSqlCommand = new PostgreSqlCommand();
            var messages = await postgreSqlCommand.FindMessagesOverPeriodOfTimeAsync(interval);

            return Ok(messages);
        }
    }
}