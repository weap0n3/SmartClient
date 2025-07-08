using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SmartClient.Core.ViewModels;
using SmartClient.Data.Services;

namespace SmartClient.Gui
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
                    fonts.AddFont("Raleway-VariableFont.ttf", "Raleway");
                    fonts.AddFont("Inter-VariableFont.ttf", "Inter");
                    fonts.AddFont("Raleway-Bold.ttf", "Raleway-Bold");
                    fonts.AddFont("segoeuithis.ttf", "Segoe UI");
                });
            #region Pages and ViewModels
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            #endregion

            #region Services
            builder.Services.AddSingleton<IMemory>(new MemoryService());
            #endregion


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
