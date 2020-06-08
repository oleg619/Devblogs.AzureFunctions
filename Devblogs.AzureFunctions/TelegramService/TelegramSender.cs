using System.Net.Http;
using System.Threading.Tasks;
using Devblogs.AzureFunctions.Configurations;

namespace Devblogs.AzureFunctions.TelegramService
{
    public class TelegramSender
    {
        private readonly string _chatId;
        private readonly string _token;
        private readonly HttpClient _client;

        public TelegramSender(
            TelegramConfiguration configuration,
            HttpClient client)
        {
            _chatId = configuration.ChatId;
            _token = configuration.Token;
            _client = client;
        }

        public Task Send(string message) =>
            _client.GetAsync($"https://api.telegram.org/bot{_token}/sendMessage?chat_id={_chatId}&text={message}");
    }
}