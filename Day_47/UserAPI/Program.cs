using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserApiApp.Data;
using UserApiApp.Repository;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        opts.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/swagger"), subApp =>
    {
        subApp.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("Content-Encoding");
            context.Response.Headers.Remove("Content-Length");
            await next();
        });
    });

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
