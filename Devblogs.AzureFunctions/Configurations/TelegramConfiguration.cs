namespace Devblogs.AzureFunctions.Configurations
{
    public class TelegramConfiguration
    {
        public string Token { get; }
        public string ChatId { get; }

        public TelegramConfiguration(string token, string chatId)
        {
            Token = token;
            ChatId = chatId;
        }
    }
}