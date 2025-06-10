using WebApi;
using Infrastructure;
using Application.Extentions;
using Infrastructure.Utils;
using WebApi.Middlewares;
using Application.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication(builder.Configuration);

#region Register Infrastructure
var mongoConncetionSettings = builder.Configuration.GetSection("Mongo").Get<MongoConnectionConfig>();
if (mongoConncetionSettings is null)
    throw new ArgumentNullException(nameof(mongoConncetionSettings));

builder.Services.Configure<MongoConnectionConfig>(builder.Configuration.GetSection("Mongo"));

builder.Services
    .AddDatabase(builder.Configuration.GetConnectionString("postgres")!)
    .AddMongoDb(mongoConncetionSettings)
    .AddServices()
    .AddRepositories();
#endregion

#region Register Application
builder.Services
    .AddValidation()
    .AddUseCases();
#endregion

builder.Services.AddBackgroundJobs();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(builder.Configuration["ClientUrl"] 
                ?? throw new ArgumentNullException("Client url"));
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
publisher.PublishMessage("1123", "������!");

app.UseCors("AllowClient");
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BotWhiteListRoutingMiddleware>();
app.MapControllers();
app.Run();