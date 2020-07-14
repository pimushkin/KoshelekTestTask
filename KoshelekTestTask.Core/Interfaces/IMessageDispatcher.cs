using System.Threading.Tasks;
using KoshelekTestTask.Core.Entities;

namespace KoshelekTestTask.Core.Interfaces
{
    /// <summary>
    ///     Contains methods that use SignalR to communicate with clients in real time.
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        ///     Send message to users who have connected to the hub.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <returns></returns>
        Task SendMessageToAllUsers(Message message);
    }
}