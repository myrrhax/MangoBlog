using WebApi;
using Infrastructure;
using Application.Extentions;
using Infrastructure.Utils;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();