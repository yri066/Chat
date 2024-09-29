using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chat.Pages
{
    public class ChatModel : PageModel
    {
        public string ChatId;

        public void OnGet(string chatId)
        {
            ChatId = chatId;
        }
    }
}
