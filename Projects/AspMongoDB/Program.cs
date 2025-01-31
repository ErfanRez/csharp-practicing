using AspMongoDB.Database;
using AspMongoDB.Models;
using AspMongoDB.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
services.AddRazorPages();

services.AddScoped<IUserService, UserService>();

services.AddScoped<MongoDBContext>();

// Configure MongoSettings from appsettings.json
services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));

// Register MongoClient as a singleton
services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var mongoSettings = serviceProvider.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(mongoSettings.ConnectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();