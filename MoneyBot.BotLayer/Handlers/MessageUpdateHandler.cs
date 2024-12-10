using System.Runtime.ConstrainedExecution;
using MoneyBot.BotLayer.Actions;
using MoneyBot.BotLayer.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoneyBot.BotLayer.Handlers
{
    internal class MessageUpdateHandler : IBaseUpdateHandler<Message>
    {
        private readonly CategoryService _categoryService;
        private readonly ExpenseService _expenseService;
        private readonly UserStateService _userStateService;

        public MessageUpdateHandler(CategoryService categoryService, ExpenseService expenseService, UserStateService userStateService)
        {
            _categoryService = categoryService;
            _expenseService = expenseService;
            _userStateService = userStateService;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            if (message == null || string.IsNullOrEmpty(message.Text))
            {
                Console.WriteLine("Message is null or empty.");
                return;
            }

            var chatId = message.Chat.Id;

            try
            {
                var userState =  _userStateService.GetUserStateAsync(chatId);
                Console.WriteLine($"User state: {userState}");

                if (userState == "WaitingForCategoryName")
                {
                    Console.WriteLine($"Waiting for category name. User input: {message.Text}");
                    var categoryName = message.Text.Trim();

                    if (string.IsNullOrEmpty(categoryName))
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Category name cannot be empty. Please try again.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    var categoryExists = await _categoryService.CategoryExistsAsync(categoryName);
                    if (categoryExists)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Category already exists. Please choose a different name.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    await _categoryService.CreateCategoryAsync(categoryName);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Category \"{categoryName}\" created successfully!",
                        cancellationToken: cancellationToken);

                    await _userStateService.SetUserStateAsync(chatId, BotActions.CreateCategory);
                    return;
                }

                if (userState?.StartsWith("WaitingForExpenseAmount:") == true)
                {
                    var categoryName = userState.Split(':')[1];
                    Console.WriteLine($"Waiting for expense amount. User input: {message.Text} for category {categoryName}");

                    if (!decimal.TryParse(message.Text, out var amount) || amount <= 0)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Invalid amount. Please enter a valid number.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    await _expenseService.AddExpenseAsync(categoryName, amount);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Added expense of {amount:C} to category \"{categoryName}\".",
                        cancellationToken: cancellationToken);

                    await _userStateService.SetUserStateAsync(chatId, null);
                    return;
                }

                switch (message.Text)
                {
                        
                  
                    
                    

                    case BotActions.Start:
                    case BotActions.Back:
                        Console.WriteLine("Start/Back action triggered.");
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Choose an operation:",
                            replyMarkup: new InlineKeyboardMarkup(new[]
                            {
                                new[] { InlineKeyboardButton.WithCallbackData("Add Expense", BotActions.AddExpense) },
                                new[] { InlineKeyboardButton.WithCallbackData("Calculate Expenses", BotActions.CalculateExpenses) }
                            }),
                            cancellationToken: cancellationToken);
                        break;

                    default:                      
                       Console.WriteLine("Dick");
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Unknown command. Please use the menu.",
                            cancellationToken: cancellationToken);
                                       
                   
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MessageUpdateHandler: {ex.Message}");
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"An error occurred: {ex.Message}",
                    cancellationToken: cancellationToken);
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error in MessageUpdateHandler: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
