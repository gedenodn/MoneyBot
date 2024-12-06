using MoneyBot.DataAccsessLayer;
using Microsoft.EntityFrameworkCore;


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

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
