namespace Chat.Domain.Entities
{
    public class ChatConfig
    {
        public const string Position = "Chat";
        public int ClientId { get; init; }
        public string PublicChatId { get; init; } = string.Empty;
        public int ChatCount { get; init; }

        public List<string> ChatList
        {
            get
            {
                var chatList = new List<string>() { PublicChatId };
                const int minChatId = 1;
                var temp = Enumerable.Range(minChatId, ChatCount).Where(x => x != ClientId).Select(x => x.ToString());
                chatList.AddRange(temp);

                return chatList;
            }
        }
    }
}
