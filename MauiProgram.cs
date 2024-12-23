using Microsoft.Extensions.Logging;
using MobileApp.Pages;

namespace MobileApp
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Rejestracja strony z grą jako domyślnej
            builder.Services.AddTransient<LabyrinthGamePage>();

            return builder.Build();
        }
    }
}
