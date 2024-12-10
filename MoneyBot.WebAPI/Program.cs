using Microsoft.EntityFrameworkCore;
using MoneyBot.BotLayer.Extensions;
using MoneyBot.BotLayer.Handlers;
using MoneyBot.BotLayer.Services;
using MoneyBot.DataAccessLayer;
using MoneyBot.DataAccessLayer.Repositories;

namespace MoneyBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MoneyBotDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultNpgConnection")));
                
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ExpenseService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddSingleton<UserStateService>();
            builder.Services.AddScoped<CallbackQueryHandler>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.RegisterTgBotServices(builder.Configuration);

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
