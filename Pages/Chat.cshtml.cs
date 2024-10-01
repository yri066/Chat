using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Chat.Pages
{
    public class ChatModel : PageModel
    {
        private ChatConfig _config;
        private readonly ApplicationDbContext _context;
        public string ChatId = string.Empty;

        public List<Message> Messages { get; private set; } = new();

        public ChatModel(ApplicationDbContext context, IOptions<ChatConfig> options)
        {
            _context = context;
            _config = options.Value;
        }

        public async Task<IActionResult> OnGet(string chatId)
        {
            ChatId = chatId;

            if(!_config.ChatList.Contains(ChatId))
            {
                return NotFound();
            }

            var userId = _config.ClientId.ToString();
            Messages = await _context.Messages.Where(x => x.RecipientId == chatId ||
                                                    x.RecipientId == userId && x.SenderId == chatId)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}
