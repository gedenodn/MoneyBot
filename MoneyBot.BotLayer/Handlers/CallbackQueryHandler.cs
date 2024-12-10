using MoneyBot.BotLayer.Actions;
using MoneyBot.BotLayer.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyBot.BotLayer.Handlers
{
    public class CallbackQueryHandler : IBaseUpdateHandler<CallbackQuery>
    {
        private readonly UserStateService _userStateService;
        private readonly CategoryService _categoryService;

        public CallbackQueryHandler(UserStateService userStateService, CategoryService categoryService)
        {
            _userStateService = userStateService;
            _categoryService = categoryService;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;

            if (callbackQuery?.Message?.Chat == null || callbackQuery.Data == null)
            {
                Console.WriteLine("Callback query is missing required data (chat or data).");
                return;
            }

            var chatId = callbackQuery.Message.Chat.Id;

            try
            {
                Console.WriteLine($"Handling callback query: {callbackQuery.Data}");

                switch (callbackQuery.Data)
                {
                    case BotActions.AddExpense:
                        Console.WriteLine("AddExpense action triggered.");
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "What would you like to do?",
                            replyMarkup: new InlineKeyboardMarkup(new[]
                            {
                                new[] { InlineKeyboardButton.WithCallbackData("Create Category", BotActions.CreateCategory) },
                                new[] { InlineKeyboardButton.WithCallbackData("Choose Existing Category", BotActions.ChooseCategory) },
                                new[] { InlineKeyboardButton.WithCallbackData("Back", BotActions.Back) }
                            }),
                            cancellationToken: cancellationToken);
                        break;

                    case BotActions.CreateCategory:
                        Console.WriteLine("CreateCategory action triggered.");
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Please enter the name of the new category:",
                            cancellationToken: cancellationToken);

                        await _userStateService.SetUserStateAsync(chatId, "WaitingForCategoryName");
                        break;

                    case BotActions.ChooseCategory:
                        Console.WriteLine("ChooseCategory action triggered.");
                        var categories = await _categoryService.GetCategoriesAsync();
                        if (categories.Any())
                        {
                            var buttons = categories
                                .Select(c => InlineKeyboardButton.WithCallbackData(c, c))
                                .Chunk(2) 
                                .ToArray();

                            await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Choose a category:",
                                replyMarkup: new InlineKeyboardMarkup(buttons),
                                cancellationToken: cancellationToken);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "No categories available. Please create one first.",
                                cancellationToken: cancellationToken);
                        }
                        break;

                    case BotActions.Back:
                        Console.WriteLine("Back action triggered.");
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
                        if (await _categoryService.CategoryExistsAsync(callbackQuery.Data))
                        {
                            Console.WriteLine($"Category selected: {callbackQuery.Data}");
                            await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: $"You selected the category: {callbackQuery.Data}. Please enter the expense amount.",
                                cancellationToken: cancellationToken);

                            await _userStateService.SetUserStateAsync(chatId, $"WaitingForExpenseAmount:{callbackQuery.Data}");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Unknown action. Please try again.",
                                cancellationToken: cancellationToken);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CallbackQueryHandler: {ex.Message}");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"An error occurred: {ex.Message}",
                    cancellationToken: cancellationToken);
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error in CallbackQueryHandler: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}