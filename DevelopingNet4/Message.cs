using System;
using System.Text.Json;

namespace DevelopingNet4
{
    public class Message : ICloneable
    {
        public string FromName { get; set; }
        public string ToName { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }

        public string ToJson() => JsonSerializer.Serialize(this);
        public static Message FromJson(string json) => JsonSerializer.Deserialize<Message>(json);

        public object Clone()
        {
            return new Message
            {
                FromName = this.FromName,
                ToName = this.ToName,
                Date = this.Date,
                Text = this.Text
            };
        }
        public class EncryptedMessageDecorator : Message
        {
            private readonly Message _message;

            public EncryptedMessageDecorator(Message message)
            {
                _message = message;
            }

            public new string ToJson()
            { 
                _message.Text = new string(_message.Text.Reverse().ToArray());
                return _message.ToJson();
            }
        }
    }
}
