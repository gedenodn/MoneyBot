using MoneyBot.BotLayer.Actions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLayer.Handlers
{
    internal class MessageUpdateHandler : IBaseUpdateHandler<Message>
    {
        private async Task SendReplyWithInlineKeyboardAsync(
            ITelegramBotClient botClient,
            long chatId,
            string text,
            InlineKeyboardMarkup keyboard,
            CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            if (message == null || string.IsNullOrEmpty(message.Text))
                return;

            var chatId = message.Chat.Id;

            switch (message.Text)
            {
                case BotActions.Start:
                case BotActions.Back:
                    await SendReplyWithInlineKeyboardAsync(
                        botClient,
                        chatId,
                        "Choose an operation:",
                        new InlineKeyboardMarkup(new[]
                        {
                            new[] { InlineKeyboardButton.WithCallbackData("Add Expense", "add_expense") },
                            new[] { InlineKeyboardButton.WithCallbackData("Calculate Expenses", "calculate_expenses") }
                        }),
                        cancellationToken);
                    break;

                default:
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Unknown command. Please use the menu.",
                        cancellationToken: cancellationToken);
                    break;
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error in MessageUpdateHandler: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}