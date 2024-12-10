namespace MoneyBot.BotLayer.Events;

public class CategorySelectedEvent
{
    public Guid UserId { get; set; }
    public string CategoryName { get; set; }
}
