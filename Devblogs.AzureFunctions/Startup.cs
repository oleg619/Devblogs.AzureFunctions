using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devblogs.AzureFunctions;
using Devblogs.AzureFunctions.Configurations;
using Devblogs.AzureFunctions.RssService;
using Devblogs.AzureFunctions.TelegramService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Parsers.Rss;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Devblogs.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton(provider =>
            {
                var token = GetValue("TelegramToken");
                var chatId = GetValue("TelegramChatId");

                return new TelegramConfiguration(token, chatId);
            });

            services.AddHttpClient<TelegramSender>();
            services.AddHttpClient<RssClient>();
            services.AddTransient<RssParser>();


            services
                .AddRssWorker("https://devblogs.microsoft.com/dotnet/feed/")
                .AddRssWorker("https://devblogs.microsoft.com/aspnet/feed/")
                .AddRssWorker(
                    "https://www.upwork.com/ab/feed/jobs/rss?q=Blazor&sort=recency&paging=0%3B10&api_params=1&securityToken=04ec91a6d21914a752497f2d0295672410f25819ed5bd9de039e1b457a72d42dc289a7678315b9e7c1d372d82e0990af65ebf1b226619f7b17083f08620e7a07&userUid=873439866367315968&orgUid=873439866371510273")
                .AddRssWorker("https://jobs.dou.ua/vacancies/feeds/?city=Львів&category=.NET")
                ;
        }

        private static string GetValue(string variable)
        {
            var environmentVariable = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process);

            if (environmentVariable == null)
            {
                throw new ArgumentNullException(variable);
            }

            return environmentVariable;
        }
    }

    public static class Extensions
    {
        public static IServiceCollection AddRssWorker(this IServiceCollection services, string url) =>
            services.AddSingleton(provider =>
            {
                var rssDownloader = provider.GetService<RssClient>();
                var telegramSender = provider.GetService<TelegramSender>();
                return new RssWorker(rssDownloader, telegramSender, url);
            });

        public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);
    }
}