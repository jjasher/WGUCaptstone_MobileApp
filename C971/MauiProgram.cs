using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace C971
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddTransient<TermDetailPage>();
            builder.Services.AddTransient<CourseDetailPage>();
            builder.Services.AddTransient<AssessmentDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
