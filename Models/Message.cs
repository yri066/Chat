using System.Text.Json.Serialization;

namespace Chat
{
    public class Message
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
