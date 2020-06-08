using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devblogs.AzureFunctions.TelegramService;
using Microsoft.Toolkit.Parsers.Rss;

namespace Devblogs.AzureFunctions.RssService
{
    public class RssWorker
    {
        private readonly RssClient _client;
        private readonly TelegramSender _sender;
        private readonly string _url;
        
        // TODO : remove it
        private string? _lastMessageTitle;

        private bool FirstRun => _lastMessageTitle is null;

        public RssWorker(RssClient client, TelegramSender sender, string url)
        {
            _client = client;
            _sender = sender;
            _url = url;
        }

        public async Task Execute()
        {
            var schemata = await _client.GetRss(_url);
            if (!FirstRun)
            {
                await SendMessages(schemata);
            }

            _lastMessageTitle = schemata[0].Title;
        }

        private Task SendMessages(IEnumerable<RssSchema> schemata) => schemata
            .TakeWhile(schema => schema.Title != _lastMessageTitle)
            .Select(SendMessage)
            .WhenAll();

        private Task SendMessage(RssSchema schema) =>
            _sender.Send($"{schema.Title}{Environment.NewLine}{schema.FeedUrl}");
    }
}