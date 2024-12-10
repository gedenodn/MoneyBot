namespace MoneyBot.BotLayer.Services;

public class UserStateService
{
    private readonly Dictionary<long, string> _userStates = new();

    public Task SetUserStateAsync(long chatId, string state)
    {
        if (state == null)
        {
            Console.WriteLine($"Removing state for user {chatId}");
            _userStates.Remove(chatId);
        }
        else
        {
            Console.WriteLine($"Setting state for user {chatId}: {state}");
            _userStates[chatId] = state;
        }
        
        return Task.CompletedTask;
    }

    public Task<string> GetUserStateAsync(long chatId)
    {
        _userStates.TryGetValue(chatId, out var state);
        Console.WriteLine($"Fetching state for user {chatId}: {state}");
        return Task.FromResult(state);
    }
}

