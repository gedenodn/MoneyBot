using MoneyBot.BotLayer.Actions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLayer.Handlers
{
    public class CallbackQueryHandler : IBaseUpdateHandler<CallbackQuery>
    {
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;

            if (callbackQuery?.Message?.Chat == null || callbackQuery.Data == null)
                return;

            var chatId = callbackQuery.Message.Chat.Id;

            switch (callbackQuery.Data)
            {

                case BotActions.AddExpense:
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Select an expense category:",
                        replyMarkup: new InlineKeyboardMarkup(new[]
                        {
                new[] { InlineKeyboardButton.WithCallbackData("Food", "food") },
                new[] { InlineKeyboardButton.WithCallbackData("Transport", "transport") },
                new[] { InlineKeyboardButton.WithCallbackData("Entertainment", "entertainment") },
                new[] { InlineKeyboardButton.WithCallbackData("Back", BotActions.Back) }
                        }),
                        cancellationToken: cancellationToken);
                    break;

                case BotActions.CalculateExpenses:
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Your expenses:\n- Food: $100\n- Transport: $50\n- Entertainment: $70\nTotal: $220.",
                        replyMarkup: new InlineKeyboardMarkup(new[]
                        {
                new[] { InlineKeyboardButton.WithCallbackData("Back", BotActions.Back) }
                        }),
                        cancellationToken: cancellationToken);
                    break;

                case "food":
                case "transport":
                case "entertainment":
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"You selected: {callbackQuery.Data}. Please enter the expense amount.",
                        cancellationToken: cancellationToken);
                    break;

                case BotActions.Back:
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Returning to the main menu...",
                        replyMarkup: new InlineKeyboardMarkup(new[]
                        {
                new[] { InlineKeyboardButton.WithCallbackData("Add Expense", BotActions.AddExpense) },
                new[] { InlineKeyboardButton.WithCallbackData("Calculate Expenses", BotActions.CalculateExpenses) }
                        }),
                        cancellationToken: cancellationToken);
                    break;

                default:
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Unknown action. Please try again.",
                        cancellationToken: cancellationToken);
                    break;
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error in CallbackQueryHandler: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}