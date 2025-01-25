

using Config;
using DataAccess.DAOs.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using DataAccess.DAOs.Implementations;
using Services.Helpers;
using MySqlConnector;
using DataAccess.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDatabaseConnection>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new MySqlConnectionFactory(connectionString);
});
// Registrar DAOs y Services
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();  // Cambiado de IAuthService a IUserService


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
