

using Config;
using DataAccess.DAOs.Interfaces;
using Services.Implementations;
using DataAccess.DAOs.Implementations;
using Services.Helpers;
using MySqlConnector;
using DataAccess.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using TournamentApiV2.Middleware;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtSettings"));
var jwtConfig = builder.Configuration.GetSection("JwtSettings").Get<JwtConfig>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtConfig.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
            RoleClaimType = ClaimTypes.Role
        };
});

builder.Services.AddSingleton<IDatabaseConnection>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new MySqlConnectionFactory(connectionString);
});

builder.Services.AddScoped<JwtHelper>();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<ICountryDao, CountryDao>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardDao, CardDao>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<IUserCardDao, UserCardDao>();
builder.Services.AddScoped<UserCardService>();
builder.Services.AddScoped<IDeckDao, DeckDao>();
builder.Services.AddScoped<DeckService>();
builder.Services.AddScoped<ICardDeckDao, CardDeckDao>();
builder.Services.AddScoped<CardDeckService>();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
