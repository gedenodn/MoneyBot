using Microsoft.EntityFrameworkCore;
using MoneyBot.BotLayer.Extensions;
using MoneyBot.DataAccessLayer;

namespace MoneyBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MoneyBotDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultNpgConnection")));
                
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.RegisterTgBotServices(builder.Configuration);


            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
