using LoadMonitor.Core.Models;
using LoadMonitor.Core.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using LoadMonitor.Core.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrchardCore.Modules;

namespace LoadMonitor.Core.Drivers;

/*
    Display driver for the custom contents form submission 
*/

public sealed class AssetPartDisplayDriver(LoadReplayService replayService, IClock clock) : ContentPartDisplayDriver<AssetPart>
{
    private const int ChartSamples = 60;
    public override IDisplayResult Display(AssetPart part, BuildPartDisplayContext context)
    {
        var details = Initialize<AssetPartViewModel>(GetDisplayShapeType(context), async model =>
        {
            Populate(model, part);

            if (!string.IsNullOrEmpty(part.LoadProfile))
            {
                model.CurrentPowerW = await replayService.GetCurrentAsync(part.LoadProfile, part.ContentItem.ContentItemId);
            }
        })
        .Location("Detail", "Content:5")
        .Location("Summary", "Content:5");

        if (string.IsNullOrEmpty(part.LoadProfile))
        {
            return details;
        }

        var chart = Initialize<AssetChartViewModel>("AssetLoadChart", async model =>
        {
            var values = await replayService.GetRecentAsync(part.LoadProfile, part.ContentItem.ContentItemId, ChartSamples);
            var now = clock.UtcNow;

            model.PowerW = values;
            model.RatedPowerW = part.RatedPowerKw * 1000;

            model.Labels = values
                .Select((_, i) => now.AddSeconds(-(values.Count - 1 - i) * LoadReplayService.StepSeconds)
                    .ToLocalTime()
                    .ToString("HH:mm:ss"))
                .ToList();
        })
        .Location("Detail", "Content:10");

        return Combine(details, chart);
    }

    public override IDisplayResult Edit(AssetPart part, BuildPartEditorContext context)
    {
        return Initialize<AssetPartViewModel>(GetEditorShapeType(context), async model =>
        {
            Populate(model, part);
            model.Profiles = (await replayService.GetProfilesAsync())
                .Select(profile => new SelectListItem(profile, profile));
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(AssetPart part, UpdatePartEditorContext context)
    {
        var model = new AssetPartViewModel();
        await context.Updater.TryUpdateModelAsync(model, Prefix);

        if (model.RatedPowerKw <= 0)
        {
            context.Updater.ModelState.AddModelError($"{Prefix}.{nameof(model.RatedPowerKw)}","Rated power must be greater than zero.");
        }

        part.RatedPowerKw = model.RatedPowerKw;
        part.RunningHours = model.RunningHours;
        part.LoadProfile = model.LoadProfile;

        return await EditAsync(part, context);
    }

    private static void Populate(AssetPartViewModel model, AssetPart part)
    {
        model.RatedPowerKw = part.RatedPowerKw;
        model.RunningHours = part.RunningHours;
        model.LoadProfile = part.LoadProfile;
    }
}