﻿using CommunityToolkit.Maui;
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
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    //fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    //fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Minecraft.ttf", "McFont");
                });

            // Rejestracja stron
            builder.Services.AddTransient<MainMenuPage>();
            builder.Services.AddTransient<LabyrinthGamePage>();
            builder.Services.AddTransient<LevelSelectionPage>();

            return builder.Build();
        }
    }
}