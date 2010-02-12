using System.Collections.Generic;

namespace ConsoleGrabit.Models
{
    public enum MessageType
    {
        Information, Warning, Error
    }
    public class Message
    {
        public Message()
        {
            _content = "";
            _messagetype = MessageType.Information;
        }
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public MessageType Messagetype
        {
            get { return _messagetype; }
            set { _messagetype = value; }
        }

        private string _content;
        private MessageType _messagetype;
    }
}
