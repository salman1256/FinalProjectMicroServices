using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Repos;
using PlatformService.SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if(builder.Environment.IsProduction())
{
    Console.WriteLine("***InProduction: Using Sql Server Database ***");
    builder.Services.AddDbContext<AppDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConn")));
}
else
{
Console.WriteLine("InDevelopment: Using In Memory Database");
builder.Services.AddDbContext<AppDbContext>(options=>options.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo,PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient,CommandDataClient>();
 builder.Services.AddSingleton<IMessageBusClient,MessageBusClient>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
System.Console.WriteLine("Command Service Running on Endpoint: "+ builder.Configuration["CommandService"]);

app.UseAuthorization();

app.MapControllers();
PrepDb.PrepData(app,builder.Environment.IsProduction());

app.Run();
