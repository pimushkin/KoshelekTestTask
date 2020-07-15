using System;
using KoshelekTestTask.Core.Entities;
using KoshelekTestTask.Core.Interfaces;

namespace KoshelekTestTask.Infrastructure.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        public bool CheckMessage(Message message, out string error)
        {
            if (message == null)
            {
                error = "Message object reference not set to an instance of an object.";
                return false;
            }

            if (string.IsNullOrEmpty(message.Text))
            {
                error = "The message cannot be empty.";
                return false;
            }

            if (message.Text.Length > 128)
            {
                error = "The maximum length of a sent message must not exceed 128 characters.";
                return false;
            }

            if (message.SerialNumber <= 0)
            {
                error = "The message serial number must be greater than 0.";
                return false;
            }

            error = null;
            return true;
        }

        public DateTime GetCurrentMoscowTime()
        {
            TimeZoneInfo moscowTimeZone;
            if (Environment.OSVersion.ToString().Contains("Microsoft") ||
                Environment.OSVersion.ToString().Contains("Windows"))
                // works on Windows only
                moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            else
                // works on Linux only
                moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);
        }
    }
}