using Chat.Domain.Entities;
using Chat.Domain.Interface;
using Chat.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Chat.Controllers
{
    /// <summary>
    /// Api сообщений
    /// </summary>
    [ApiController]
    [Route("api/[action]")]
    public class MessageApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMessageSend _sendMessage;
        private readonly ChatConfig _chatConfig;

        public MessageApiController(ApplicationDbContext context, IMessageSend sendMessage, IOptions<ChatConfig> options)
        {
            _context = context;
            _sendMessage = sendMessage;
            _chatConfig = options.Value;
        }

        /// <summary>
        /// Получить все сообщения.
        /// </summary>
        /// <returns>Список сообщений.</returns>
        [HttpGet]
        public List<Message> GetAllMessages()
        {
            return _context.Messages.AsNoTracking().ToList();
        }

        /// <summary>
        /// Получить все сообщения с пользователем.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns>Список сообщений.</returns>
        [HttpGet]
        public async Task<ActionResult> GetAllMessagesWithUser(string userId)
        {
            if (!_chatConfig.ChatList.Contains(userId))
            {
                return NotFound();
            }

            var currentId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.RecipientId == userId ||
                                                    x.RecipientId == currentId && x.SenderId == userId)
                .AsNoTracking()
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Получить входящие сообщения.
        /// </summary>
        /// <returns>Список сообщений.</returns>
        [HttpGet]
        public async Task<List<Message>> GetIncomingMessages()
        {
            var currentId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.RecipientId == currentId)
                .AsNoTracking()
                .ToListAsync();

            return list;
        }

        /// <summary>
        /// Получить отправленные сообщения.
        /// </summary>
        /// <returns>Список сообщений.</returns>
        [HttpGet]
        public async Task<List<Message>> GetSentMessages()
        {
            var currentId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.SenderId == currentId)
                .AsNoTracking()
                .ToListAsync();

            return list;
        }

        /// <summary>
        /// Отправить сообщение всем.
        /// </summary>
        /// <param name="messageText">Текст сообщения.</param>
        [HttpPost]
        public async Task SendMessageToAll(string messageText)
        {
            await SendMessage(_chatConfig.PublicChatId, messageText);
        }

        /// <summary>
        /// ОТправить сообщение пользователю.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="messageText">Текст сообщения.</param>
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

        /// <summary>
        /// ОТправить сообщение пользователю.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="messageText">Текст сообщения.</param>
        [HttpPost]
        public ActionResult SendMessageDelayToUser(string userId, string messageText, int delay)
        {
            if(delay < 0)
            {
                return BadRequest();
            }

            if (!_chatConfig.ChatList.Contains(userId))
            {
                return NotFound();
            }

            _sendMessage.SendMessageToDelay(userId, messageText, delay);

            return Ok();
        }

        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="textMessage">Текст сообщения.</param>
        private async Task SendMessage(string userId, string textMessage)
        {
            await _sendMessage.SendMessageTo(userId, textMessage);
        }
    }
}
