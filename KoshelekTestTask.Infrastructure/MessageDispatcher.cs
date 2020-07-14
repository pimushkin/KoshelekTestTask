using System.Threading.Tasks;
using KoshelekTestTask.Core.Entities;
using KoshelekTestTask.Core.Interfaces;
using KoshelekTestTask.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KoshelekTestTask.Infrastructure
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageDispatcher(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToAllUsers(Message message)
        {
            await _hubContext.Clients.All.SendAsync("Send", message.SerialNumber, message.Text,
                message.TimeOfSending);
        }
    }
}