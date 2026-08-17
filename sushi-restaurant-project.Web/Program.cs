using sushi_restaurant_project.Shared.Database;
using sushi_restaurant_project.Shared.Services;
using sushi_restaurant_project.Web.Components;
using sushi_restaurant_project.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the sushi_restaurant_project.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

builder.Services.AddSingleton<SushiDatabase>(_ =>
{
    var databasePath = Path.Combine(
        builder.Environment.ContentRootPath,
        "Data",
        "sushi.db");

    return new SushiDatabase(databasePath);
});

builder.Services.AddSingleton<DatabaseSeeder>();
builder.Services.AddSingleton<IPlateService, PlateService>();

var app = builder.Build();

var database =
    app.Services.GetRequiredService<SushiDatabase>();

database.Initialize();

var databaseSeeder =
    app.Services.GetRequiredService<DatabaseSeeder>();

databaseSeeder.Seed();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(sushi_restaurant_project.Shared._Imports).Assembly);

app.Run();
