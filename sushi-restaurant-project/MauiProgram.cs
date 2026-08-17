using Microsoft.Extensions.Logging;
using sushi_restaurant_project.Services;
using sushi_restaurant_project.Shared.Services;
using Microsoft.Maui.Storage;
using sushi_restaurant_project.Shared.Database;

namespace sushi_restaurant_project
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the sushi_restaurant_project.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<SushiDatabase>(_ =>
            {
                var databasePath = Path.Combine(
                    FileSystem.AppDataDirectory,
                    "sushi.db");

                return new SushiDatabase(databasePath);
            });
            builder.Services.AddSingleton<DatabaseSeeder>();
            builder.Services.AddSingleton<IPlateService, PlateService>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            var database = app.Services.GetRequiredService<SushiDatabase>();
            database.Initialize();

            var databaseSeeder =
            app.Services.GetRequiredService<DatabaseSeeder>();

            databaseSeeder.Seed();

            return app;
        }
    }
}
