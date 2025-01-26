using Microsoft.OpenApi.Models;
using SlaveBackend.Controllers;
using SlaveBackend.Services;


SteamCMDHelper _steamcmd = new SteamCMDHelper();
OSHelper _osHelper = new OSHelper();
_osHelper.GetHostInfo();
_steamcmd.SetupSteamCMD();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton<GameController>();
builder.Services.AddSingleton<GameTemplateService>();
builder.Services.AddSingleton<MetricsController>();
builder.Services.AddSingleton<ServerController>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
