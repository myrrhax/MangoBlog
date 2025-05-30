using WebApi;
using Infrastructure;
using Application.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication(builder.Configuration);

#region Register Infrastructure
builder.Services
    .AddDatabase(builder.Configuration.GetConnectionString("postgres")!)
    .AddServices()
    .AddRepositories();
#endregion

#region Register Application
builder.Services
    .AddValidation()
    .AddUseCases();
#endregion

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