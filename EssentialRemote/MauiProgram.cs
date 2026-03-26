using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace EssentialRemote
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // 3. REGISTRER DEPENDENCY INJECTION HER (Best practice for MVVM)
            // Dette fortæller MAUI, at den automatisk skal oprette disse klasser for dig.
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ViewModels.RobotControllerViewModel>();

            return builder.Build();
        }
    }
}
