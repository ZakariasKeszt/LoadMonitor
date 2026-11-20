using System;
using System.Collections.Generic;
using System.Text;
using LoadMonitor.Core.Models;
using LoadMonitor.Core.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.DisplayManagement.ModelBinding;
using LoadMonitor.Core.Services;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace LoadMonitor.Core.Drivers;

/*
    Display driver for the custom contents form submission 
*/

public sealed class AssetPartDisplayDriver(LoadReplayService replayService) : ContentPartDisplayDriver<AssetPart>
{
    public override IDisplayResult Display(AssetPart part, BuildPartDisplayContext context)
    {
        return Initialize<AssetPartViewModel>(GetDisplayShapeType(context), async model =>
        {
            Populate(model, part);

            if (!string.IsNullOrEmpty(part.LoadProfile))
            {
                model.CurrentPowerW = await replayService.GetCurrentAsync(part.LoadProfile, part.ContentItem.ContentItemId);
            }
        })
        .Location("Detail", "Content:5")
        .Location("Summary", "Content:5");
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