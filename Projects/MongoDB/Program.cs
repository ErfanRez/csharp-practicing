using AspProMongoDb.Web.Models;
using AspProMongoDb.Web.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddRazorPages();
services.AddScoped<IUserService, UserService>();

services.Configure<MongoSettings>(
      builder.Configuration.GetSection("MongoSettings"));

services.AddSingleton<MongoSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoSettings>>().Value);

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
