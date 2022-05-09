//[assembly: ExportFont("IBMPlexMono-Regular.ttf", Alias = "IBMPlexMono")]

namespace DopeTestMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("IBMPlexMono-Regular.ttf", "IBMPlexMono");
			});

		return builder.Build();
	}
}
