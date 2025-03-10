

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
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardDao, CardDao>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IUserCardDao, UserCardDao>();
builder.Services.AddScoped<IUserCardService, UserCardService>();
builder.Services.AddScoped<IDeckDao, DeckDao>();
builder.Services.AddScoped<IDeckService, DeckService>();
builder.Services.AddScoped<ICardDeckDao, CardDeckDao>();
builder.Services.AddScoped<ICardDeckService, CardDeckService>();
builder.Services.AddScoped<ISerieDao, SerieDao>();
builder.Services.AddScoped<ISerieService, SerieService>();
builder.Services.AddScoped<ICardSeriesDao, CardSeriesDao>();
builder.Services.AddScoped<ICardSeriesService, CardSeriesService>();
builder.Services.AddScoped<ITournamentDao, TournamentDao>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<ITournamentPlayerDao, TournamentPlayerDao>();
builder.Services.AddScoped<ITournamentJudgeDao, TournamentJudgeDao>();
builder.Services.AddScoped<ITournamentJudgeService, TournamentJudgeService>();
builder.Services.AddScoped<IMatchDao, MatchDao>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IDisqualificationDao, DisqualificationDao>();
builder.Services.AddScoped<IDisqualificationService, DisqualificationService>();


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

public partial class Program { }