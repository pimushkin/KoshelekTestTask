using System;
using KoshelekTestTask.Core.Entities.Base;

namespace KoshelekTestTask.Core.Entities
{
    public class Message : BaseMessage
    {
        public DateTime TimeOfSending { get; set; }
    }
}