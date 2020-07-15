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
using Microsoft.AspNetCore.Http;
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
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IMessageHandler _messageHandler;
        private readonly IMessageService _messageService;

        /// <summary>
        ///     MessageController constructor.
        /// </summary>
        /// <param name="hubContext">Hub used for real-time messaging with clients.</param>
        /// <param name="messageDispatcher">Contains methods that use SignalR to communicate with clients in real time.</param>
        /// <param name="messageHandler">Contains methods used for checking and configuring the message.</param>
        /// <param name="messageService">Contains SQL commands required for working with database.</param>
        public MessageController(IMessageDispatcher messageDispatcher, IMessageHandler messageHandler, IMessageService messageService)
        {
            _messageDispatcher = messageDispatcher;
            _messageHandler = messageHandler;
            _messageService = messageService;
        }

        // POST api/<MessageController>/Send
        /// <summary>
        ///     Save the message in the database and send the message to everyone using the hub.
        /// </summary>
        /// <remarks>
        /// Note that the serial number is a natural number. The serial number cannot be equal to 0.
        ///  
        ///     POST
        ///     {
        ///        "serialNumber": 1,
        ///        "text": "Hello, World!"
        ///     }
        /// 
        /// </remarks>
        /// <param name="messageToPost">Message to send.</param>
        /// <returns>Result of sending the message. If the message was sent successfully, it returns the sent message with the time attribute.</returns>
        /// <response code="200">Returns the sent message with the time attribute.</response>
        /// <response code="400">If the item is null or contains attributes that have not passed validation.</response>
        [HttpPost("Send")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Message>> SendMessage(MessageToPost messageToPost)
        {
            var moscowDateTime = _messageHandler.GetCurrentMoscowTime();
            var message = new Message { Text = messageToPost.Text, SerialNumber = messageToPost.SerialNumber, TimeOfSending = moscowDateTime };
            var valid = _messageHandler.CheckMessage(message, out var error);
            if (!valid)
            {
                return BadRequest(error);
            }

            await _messageService.SendMessageAsync(message);

            await _messageDispatcher.SendMessageToAllUsers(message);

            return Ok(message);
        }

        // POST api/<MessageController>/FindOverPeriodOfTime
        /// <summary>
        ///     Get a list of all messages that were sent over a certain period of time.
        /// </summary>
        /// <remarks>
        /// Note that the time format must be in accordance with the ISO 8601 standard.
        /// </remarks>
        /// <param name="interval">The time period in which to find messages.</param>
        /// <returns>A list of all messages that were found.</returns>
        /// <response code="200">Returns all messages for a specific time period that were found in the database.</response>
        /// <response code="400">If the item is null or the entered date format is not supported.</response>
        [HttpPost("FindOverPeriodOfTime")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Message>>> GetMessages(Interval interval)
        {
            var messages = await _messageService.FindMessagesOverPeriodOfTimeAsync(interval);

            return Ok(messages);
        }
    }
}