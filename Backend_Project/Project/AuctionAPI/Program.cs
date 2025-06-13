using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using AuctionAPI;
using AuctionAPI.Contexts;
using AuctionAPI.Hubs;
using AuctionAPI.Interfaces;
using AuctionAPI.Misc;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using AuctionAPI.Repositories;
using AuctionAPI.Services;
using AuctionAPI.Validation;
using FirstAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3} {Username} {Message:lj}{Exception}{NewLine}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "AuctionAPI")    
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.ApplicationInsights(
        connectionString: builder.Configuration["ApplicationInsights:ConnectionString"],
        telemetryConverter: TelemetryConverter.Traces)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Auction API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddApiVersioning(options => {
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});


builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
        RateLimitPartition.GetTokenBucketLimiter("global", _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 1000,                   
            TokensPerPeriod = 1000,              
            ReplenishmentPeriod = TimeSpan.FromHours(1),
            AutoReplenishment = true,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        }));

    options.RejectionStatusCode = 429; 
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Keys:JwtTokenKey"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<string>
                {
                    Success = false,
                    Message = "Unauthorized access.",
                    Data = null
                };
                var result = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(result);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<string>
                {
                    Success = false,
                    Message = "Forbidden: You do not have permission to access this resource.",
                    Data = null
                };
                var result = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(result);
            }
        };
    });



builder.Services.AddAuthorization();

builder.Services.AddDbContext<AuctionContext>(opt =>
     opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


#region Repository
builder.Services.AddTransient<IRepository<Guid, Seller>, SellerRepository>();
builder.Services.AddTransient<IRepository<Guid, Bidder>, BidderRepository>();
builder.Services.AddTransient<IRepository<string, User>, UserRepository>();
builder.Services.AddTransient<IRepository<Guid, Item>, ItemRepository>();
builder.Services.AddTransient<IRepository<Guid, Bid>, BidRepository>();
builder.Services.AddTransient<IRepository<Guid, ItemDetails>, ItemDetailsRepository>();
builder.Services.AddTransient<IRepository<string, RefreshToken>, RefreshTokenRepository>();
builder.Services.AddTransient<IRepository<Guid, Audit>, AuditRepository>();
#endregion


#region Services
builder.Services.AddTransient<IUserService<Seller>, SellerService>();
builder.Services.AddTransient<IUserService<Bidder>, BidderService>();
builder.Services.AddTransient<IBidService, BidService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IFunctionalities, Functionalities>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddTransient<IValidation, Validation>();
#endregion

#region Misc
builder.Services.AddTransient<IFunctionalities, Functionalities>();
#endregion

builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(User));

builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}


app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.MapHub<AuctionHub>("/auctionHub");

app.Run();
