namespace Chat.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
