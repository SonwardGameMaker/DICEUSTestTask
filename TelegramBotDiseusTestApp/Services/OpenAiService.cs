using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

namespace TelegramBotDiseusTestApp.Services
{
    internal class OpenAiService
    {
        private ChatClient _client;

        public OpenAiService(string model, string apiKey)
        {
            _client = new ChatClient(model, apiKey);
        }

        public async Task<string> TextToChat(string text)
        {
            Console.WriteLine("Texting to chat");
            var result = await _client.CompleteChatAsync(text);
            Console.WriteLine("Done texting to chat");
            return result.Value.Content[0].Text;
        }
        public async Task<string> TextToChat2(string text)
        {
            try
            {
                Console.WriteLine("Texting to chat");
                var result = await _client.CompleteChatAsync(text);
                Console.WriteLine("Done texting to chat");
                return result.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return "ERROR: " + ex.Message;
            }
        }

        public string TextToChatTest(string text)
        {
            StringBuilder builder = new StringBuilder("You texted: ");
            builder.Append(text);
            return builder.ToString();
        }
    }
}
