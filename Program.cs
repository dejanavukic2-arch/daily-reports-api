using DailyReports.Api.Data;
using DailyReports.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapGet("/", () => "DailyReports API radi.");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate();

    if (!context.Users.Any())
    {
        context.Users.AddRange(
            new User
            {
                FullName = "Sladjan Pantic",
                Email = "sladjan@vodomont.rs",
                PasswordHash = "123456",
                Role = "admin"
            },
            new User
            {
                FullName = "Marko Markovic",
                Email = "marko@vodomont.rs",
                PasswordHash = "123456",
                Role = "worker"
            }
        );

        context.SaveChanges();
    }
}

app.Run();