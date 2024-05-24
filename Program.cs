using AutoMapper;
using BerryMusicV1.Containers;
using BerryMusicV1.Helpers;
using BerryMusicV1.Modals;
using BerryMusicV1.Repos;
using BerryMusicV1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DbContext
//DbContext
builder.Services.AddDbContext<BMContext>(o=> o.UseMySQL(builder.Configuration.GetConnectionString("conn")!));
#endregion

#region Seriolog
//Seriolog
string logpath = builder.Configuration.GetSection("Logging:LogPath").Value!;
var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(logpath!)
    .CreateLogger();
builder.Logging.AddSerilog(logger);
#endregion

#region AutoMapper
//AutoMapper
var autoMapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));

IMapper mapper = autoMapper.CreateMapper();

builder.Services.AddSingleton(mapper);
#endregion

#region RateLimitter
//Rate Limmiter
builder.Services.AddRateLimiter(_ =>
_.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
{
    options.Window = TimeSpan.FromSeconds(5);
    options.PermitLimit = 1;
    options.QueueLimit = 0;
    options.QueueProcessingOrder =
    System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode = 401);
#endregion

#region Authentication
//Authentication
var _key = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
var _issuer = builder.Configuration.GetValue<string>("JwtSettings:issuer");
var _audience = builder.Configuration.GetValue<string>("JwtSettings:audience");

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(item =>
{
    item.SaveToken = true;
    item.RequireHttpsMetadata = false;
    item.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {   
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = _issuer,
        ValidAudience = _audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(30),
    };
});
var settings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(settings);
#endregion

#region Transient
builder.Services.AddTransient<IUnauthorize, UnauthorizeService>();
builder.Services.AddTransient<IAuthorize,AuthorizeService>();
#endregion

#region Cache
builder.Services.AddMemoryCache();
#endregion

var app = builder.Build();
app.UseRateLimiter();

app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
