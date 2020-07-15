using System.Collections.Generic;
using System.Threading.Tasks;
using KoshelekTestTask.Core.Entities;
using KoshelekTestTask.Core.Models;

namespace KoshelekTestTask.Core.Interfaces
{
    /// <summary>
    ///     Contains SQL commands required for working with PostgreSQL.
    /// </summary>
    public interface IPostgreSqlCommand
    {
        /// <summary>
        ///     Send a message asynchronously to the database.
        /// </summary>
        /// <param name="message">The message that will be sent to the server.</param>
        /// <returns></returns>
        Task SendMessageAsync(Message message);
        /// <summary>
        ///     Get a list of messages for the specified time period.
        /// </summary>
        /// <param name="interval">The time period in which you need to find messages.</param>
        /// <returns></returns>
        Task<List<Message>> FindMessagesOverPeriodOfTimeAsync(Interval interval);
    }
}