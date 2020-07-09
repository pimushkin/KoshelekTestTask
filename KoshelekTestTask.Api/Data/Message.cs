using System;

namespace KoshelekTestTask.Api.Data
{
    public class Message
    {
        public int SerialNumber { get; set; }
        public string Content { get; set; }
        public DateTime MoscowDateTime { get; internal set; }
    }
}