using AddressParse.Lib;

namespace AddressParse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IParser<AddressInfo>, AddressParser>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware(typeof(ErrorHandlingMiddleware), app.Environment);
            app.UseHttpsRedirection();

            //  app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();

        }
    }
}