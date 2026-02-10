using Microsoft.Azure.Cosmos;
using Project.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connectionString = builder.Configuration["CosmosDb:ConnectionString"];
builder.Services.AddSingleton(new CosmosClient(connectionString));

builder.Services.AddSingleton<CosmosDbService>();

builder.Services.AddSingleton<BlobStorageService>();

builder.Services.AddSingleton<AIService>();

builder.Services.AddSingleton<ComputerVisionService>();

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Vision}/{action=Upload}/{id?}");

app.Run();
