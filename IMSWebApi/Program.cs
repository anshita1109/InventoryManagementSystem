
using IMSWebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace IMSWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<InventoryDBaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policyBuilder => policyBuilder.AllowAnyOrigin()
                                                                                 .AllowAnyHeader()
                                                                                 .AllowAnyMethod());
            });
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseCors("AllowAllOrigins");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
