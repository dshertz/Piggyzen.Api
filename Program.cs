global using Piggyzen.Api.Models;
using Microsoft.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Piggyzen.Api.Data;
using Piggyzen.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PiggyzenContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<CategoryService>();

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

// Add services och formatters
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, new TextPlainInputFormatter());
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Use fully qualified names to avoid duplicate schema IDs
    options.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();



// Seed the database.
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<PiggyzenContext>();
    await context.Database.MigrateAsync();

    // Seed både Categories och Tags
    await SeedData.SeedAll(context); // Anropa SeedAll-metoden för att inkludera allt
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAllOrigins");

app.Run();

// Custom TextPlainInputFormatter class
public class TextPlainInputFormatter : Microsoft.AspNetCore.Mvc.Formatters.TextInputFormatter
{
    public TextPlainInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
        SupportedEncodings.Add(System.Text.Encoding.UTF8);
        SupportedEncodings.Add(System.Text.Encoding.Unicode);
    }

    protected override bool CanReadType(Type type) =>
        type == typeof(string) || base.CanReadType(type);
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        using var reader = new StreamReader(context.HttpContext.Request.Body, encoding);
        var content = await reader.ReadToEndAsync();
        return InputFormatterResult.Success(content);
    }
}