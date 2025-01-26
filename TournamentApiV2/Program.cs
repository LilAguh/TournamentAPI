

using Config;
using DataAccess.DAOs.Interfaces;
using Services.Implementations;
using Services.Interfaces;
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

var builder = WebApplication.CreateBuilder(args);

// Configurar JWT
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtSettings"));
var jwtConfig = builder.Configuration.GetSection("JwtSettings").Get<JwtConfig>();

// Autenticación JWT
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

// Registrar JwtHelper
builder.Services.AddScoped<JwtHelper>();

// Registrar servicios de autenticación/autorización
builder.Services.AddAuthorization();

// Registrar DAOs y Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<ICountryDao, CountryDao>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();  // Cambiado de IAuthService a IUserService


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tournament API", Version = "v1" });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tournament API v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
