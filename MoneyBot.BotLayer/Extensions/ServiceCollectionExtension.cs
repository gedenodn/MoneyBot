using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyBot.BotLayer.Handlers;

namespace MoneyBot.BotLayer.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterTgBotServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITelegramBotClient>(provider =>
            {
                //var botToken = configuration["BotConfiguration:BotToken"];

                //if (string.IsNullOrEmpty(botToken))
                //{
                //    throw new InvalidOperationException("Bot token is missing in the configuration.");
                //}

                return new TelegramBotClient("7967235930:AAF7TnAKbZQcBZIaBczAYAaO9j3CANGWN28");
            });
            services.AddScoped<IBaseUpdateHandler<Message>, MessageUpdateHandler>();
            services.AddScoped<IBaseUpdateHandler<CallbackQuery>, CallbackQueryHandler>();

            services.AddHostedService<TelegramHostedService>();
        }
    }
}
