using BotLayer.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MoneyBot.BotLayer
{
    internal class TelegramHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public TelegramHostedService(IServiceProvider serviceProvider,
                                    ITelegramBotClient botClient)
        {
            _botClient = botClient;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient.StartReceiving(
                async (_botClient,update,cancellationToken) =>
                {
                    using (var serviceScope = _serviceProvider.CreateScope())
                    {
                        switch (update.Type)
                        {
                            case UpdateType.Message:
                                await serviceScope.ServiceProvider.GetRequiredService<IBaseUpdateHandler<Message>>().HandleUpdateAsync(_botClient,update,cancellationToken);
                                break;
                            case UpdateType.CallbackQuery:
                                await serviceScope.ServiceProvider.GetRequiredService<IBaseUpdateHandler<CallbackQuery>>().HandleUpdateAsync(_botClient, update, cancellationToken);
                                break;
                            default:
                                break;
                        }
                    }
                },
                async (_botClient, exception, cancellationToken) =>
                {
                    //Exceptions handler
                }
                ,
                receiverOptions:new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() }, 
                _cancellationTokenSource.Token
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
