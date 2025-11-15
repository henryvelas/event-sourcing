using Ticketing.Query.Application;
using Ticketing.Query.Application.Extensions;
using Ticketing.Query.Infrastructure;
using Ticketing.Query.Infrastructure.Consumers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.RegisterInfrastructureServices(builder.Configuration);
builder.Services.RegisterApplicationServices();
builder.Services.AddControllers();

var app = builder.Build();


await app.ApplyMigration();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("*"));
app.MapControllers();
app.Run();