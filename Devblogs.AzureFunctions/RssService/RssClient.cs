using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Toolkit.Parsers.Rss;

namespace Devblogs.AzureFunctions.RssService
{
    public class RssClient
    {
        private readonly HttpClient _client;
        private readonly RssParser _parser;

        public RssClient(
            HttpClient client,
            RssParser parser)
        {
            _client = client;
            _parser = parser;
        }

        public async Task<RssSchema[]> GetRss(string url)
        {
            var feed = await _client.GetStringAsync(url);
            var rss = _parser.Parse(feed);

            return rss.OrderByDescending(schema => schema.PublishDate).ToArray();
        }
    }
}