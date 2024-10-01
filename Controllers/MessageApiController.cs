using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/[action]")]
    public class MessageApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProducer _producer;
        private readonly ChatConfig _chatConfig;

        public MessageApiController(ApplicationDbContext context, IProducer producer, IOptions<ChatConfig> options)
        {
            _context = context;
            _producer = producer;
            _chatConfig = options.Value;
        }

        [HttpGet]
        public List<Message> GetAllMessages()
        {
            return _context.Messages.AsNoTracking().ToList();
        }

        [HttpGet]
        public async Task<ActionResult> GetMessagesWithUser(string userId)
        {
            if (!_chatConfig.ChatList.Contains(userId))
            {
                return NotFound();
            }

            var currengId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.RecipientId == userId ||
                                                    x.RecipientId == currengId && x.SenderId == userId)
                .AsNoTracking()
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet]
        public async Task<List<Message>> GetIncomingMessages()
        {
            var currengId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.RecipientId == currengId)
                .AsNoTracking()
                .ToListAsync();

            return list;
        }

        [HttpGet]
        public async Task<List<Message>> GetSentMessages()
        {
            var currengId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.SenderId == currengId)
                .AsNoTracking()
                .ToListAsync();

            return list;
        }

        [HttpPost]
        public async Task SendMessageToAll(string messageText)
        {
            await SendMessage(_chatConfig.PublicChatId, messageText);
        }

        [HttpPost]
        public async Task<ActionResult> SendMessageToUser(string userId,string messageText)
        {
            if (!_chatConfig.ChatList.Contains(userId))
            {
                return NotFound();
            }

            await SendMessage(userId, messageText);

            return Ok();
        }

        private async Task SendMessage(string userId, string textMessage)
        {
            var message = new Message
            {
                SenderId = _chatConfig.ClientId.ToString(),
                RecipientId = userId,
                Text = textMessage
            };

            await _producer.SendMessage(message);
        }
    }
}
