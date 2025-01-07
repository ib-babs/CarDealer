using CarDealer.Interfaces;
using CarDealer.Models;
using CarDealer.Repositories;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var carSpotLightConnectionString = builder.Configuration["SqlServerConnectionString"];
var blobContainerConnectionString = builder.Configuration["BlobConnectionString"];

builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddAzureClients(azureConfig =>
{
    azureConfig.AddBlobServiceClient(blobContainerConnectionString);
});

builder.Services.AddSqlServer<CarDbContext>(carSpotLightConnectionString, sqlOptions =>
    sqlOptions.EnableRetryOnFailure());


builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();


var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
//app.Use(async (context, next) =>
//{
//    await next();
//    if (context.Response.StatusCode == 404)
//    {
//        context.Response.Redirect("/");
//    }
//});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthorization();

//app.MapDefaultControllerRoute();

app.MapControllerRoute(name: "default", pattern: "{controller=Car}/{action=Index}/{id?}"
);


app.Run();
