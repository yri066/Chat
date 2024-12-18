using Chat.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Chat.Pages
{
    public class IndexModel : PageModel
    {
        public readonly List<string> ChatList;

        public IndexModel(IOptions<ChatConfig> option)
        {
            var options = option.Value;
            ChatList = options.ChatList;
        }

        public void OnGet()
        {

        }
    }
}
