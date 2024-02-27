using Microsoft.EntityFrameworkCore;
using Serilog;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StargateContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("StarbaseApiDatabase")));

Log.Logger = new LoggerConfiguration()
    .WriteTo.Conditional(e => e.Level == Serilog.Events.LogEventLevel.Error, wt => wt.SQLite(sqliteDbPath: Environment.CurrentDirectory + @"\starbase.db", tableName: "ExceptionLog", storeTimestampInUtc: true))
    .WriteTo.Conditional(e => e.Level == Serilog.Events.LogEventLevel.Information, wt => wt.SQLite(sqliteDbPath: Environment.CurrentDirectory + @"\starbase.db", tableName: "SuccessLog", storeTimestampInUtc: true))
    .CreateLogger();

//builder.Host.UseSerilog((ctx, lc) => lc
//    .WriteTo.Console()
//    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddMediatR(cfg =>
{
    cfg.AddRequestPreProcessor<CreateAstronautDutyPreProcessor>();
    cfg.AddRequestPreProcessor<CreatePersonPreProcessor>();
    cfg.AddRequestPreProcessor<UpdatePersonPreProcessor>();
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


