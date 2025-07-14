using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SmartClient.Core.ViewModels;
using SmartClient.Data.Services;
using SmartClient.Gui.Pages;

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
                .ConfigureLifecycleEvents(events =>
                {
#if WINDOWS
        events.AddWindows(w =>
                {
                    w.OnClosed((window,args) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Window is being destroyed. Time to clean up files!");
                        string folderToWatch = @"C:\CapHotel";
                        var fileCleanerService = new FileCleanerService(folderToWatch);
                        fileCleanerService.CleanUpNow();
                    });
                });
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Raleway-VariableFont.ttf", "Raleway");
                    fonts.AddFont("Inter-VariableFont.ttf", "Inter");
                    fonts.AddFont("Raleway-Bold.ttf", "Raleway-Bold");
                    fonts.AddFont("segoeuithis.ttf", "Segoe UI");
                    fonts.AddFont("LensGroteskW05-Regular.ttf", "LensGrotesk");
                });
            #region Pages and ViewModels
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginViewModel>();
            #endregion

            #region Services
            var appDataPath = FileSystem.AppDataDirectory;
            System.Diagnostics.Debug.WriteLine(appDataPath); 
            builder.Services.AddSingleton<IMemory>(new MemoryService(appDataPath));
            #endregion


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
