using Microsoft.EntityFrameworkCore;
using VersionManager.Api.Errors;
using VersionManager.Application;
using VersionManager.Infrastructure;
using VersionManager.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWeb", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "VersionManager.Api v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.UseCors("AllowWeb");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("DbMigration");

    var retries = 10;
    var delay = TimeSpan.FromSeconds(3);

    for (var attempt = 1; attempt <= retries; attempt++)
    {
        try
        {
            logger.LogInformation("Applying migrations... attempt {Attempt}/{Retries}", attempt, retries);
            db.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Migration attempt {Attempt} failed.", attempt);

            if (attempt == retries)
                throw;

            Thread.Sleep(delay);
        }
    }
}


app.MapControllers();
app.Run();
