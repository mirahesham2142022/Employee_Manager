using EmployeeManager.API.Middleware;
using EmployeeManager.Application.Services.Implementation;
using EmployeeManager.Application.Services.Interfaces;
using EmployeeManager.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
namespace EmployeeManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddCors(options => options.AddPolicy("Angular", policy =>
                policy.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfraStrucSture(builder.Configuration);

            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();

            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();   

            app.UseCors("Angular");
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

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

