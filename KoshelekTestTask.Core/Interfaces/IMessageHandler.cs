using System;
using KoshelekTestTask.Core.Entities;

namespace KoshelekTestTask.Core.Interfaces
{
    /// <summary>
    ///     Contains methods used for checking and configuring the message.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        ///     Check the validity of the message.
        /// </summary>
        /// <param name="message">The message that will be checked.</param>
        /// <param name="error">Error message.</param>
        /// <returns></returns>
        bool CheckMessage(Message message, out string error);
        /// <summary>
        ///     Allows you to determine the current Moscow time.
        /// </summary>
        /// <returns>Current Moscow time.</returns>
        DateTime GetCurrentMoscowTime();
    }
}