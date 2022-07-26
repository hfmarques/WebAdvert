using SearchApi.Extensions;
using SearchApi.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddElasticSearch(configuration);
builder.Services.AddTransient<ISearchService, SearchService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddAWSProvider(configuration.GetAWSLoggingConfigSection(),
    formatter: (loglevel, message, exception) =>
        $"[{DateTime.Now} {loglevel} {message} {exception?.Message} {exception?.StackTrace}");

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