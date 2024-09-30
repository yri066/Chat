using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Chat.Pages
{
    public class ChatModel : PageModel
    {
        private ChatConfig _config;
        public string ChatId = string.Empty;

        public ChatModel(IOptions<ChatConfig> options)
        {
            _config = options.Value;
        }

        public IActionResult OnGet(string chatId)
        {
            ChatId = chatId;

            if(!_config.ChatList.Contains(ChatId))
            {
                return NotFound();
            }

            return Page();
        }
    }
}
