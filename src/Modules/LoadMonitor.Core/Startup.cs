using LoadMonitor.Core.Drivers;
using LoadMonitor.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using LoadMonitor.Core.Index;
using LoadMonitor.Core.Services;
using OrchardCore.Data;

namespace LoadMonitor.Core;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataMigration<Migrations>();
        services.AddContentPart<AssetPart>().UseDisplayDriver<AssetPartDisplayDriver>();
        services.AddIndexProvider<LoadSampleIndexProvider>();
        services.AddScoped<LoadProfileImporter>();
        services.AddScoped<LoadReplayService>();
    }

    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        routes.MapAreaControllerRoute(
            name: "Home",
            areaName: "LoadMonitor.Core",
            pattern: "Home/Index",
            defaults: new { controller = "Home", action = "Index" }
        );
    }
}

