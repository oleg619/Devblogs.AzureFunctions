using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devblogs.AzureFunctions.RssService;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Devblogs.AzureFunctions
{
    public class TimerFunction
    {
        private readonly IEnumerable<RssWorker> _workers;

        public TimerFunction(IEnumerable<RssWorker> workers)
        {
            _workers = workers;
        }

        [FunctionName("Rss")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await Task.WhenAll(_workers.Select(worker => worker.Execute()));
        }
    }
}