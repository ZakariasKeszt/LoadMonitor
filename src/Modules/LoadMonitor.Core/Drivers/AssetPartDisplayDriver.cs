using System;
using System.Collections.Generic;
using System.Text;
using LoadMonitor.Core.Models;
using LoadMonitor.Core.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.DisplayManagement.ModelBinding;

namespace LoadMonitor.Core.Drivers;

/*
    Display driver for the custom contents form submission 
*/

public sealed class AssetPartDisplayDriver : ContentPartDisplayDriver<AssetPart>
{
    public override IDisplayResult Display(AssetPart part, BuildPartDisplayContext context)
    {
        return Initialize<AssetPartViewModel>(GetDisplayShapeType(context), model => Populate(model, part))
            .Location("Detail", "Content:5")
            .Location("Summary", "Content:5");
    }

    public override IDisplayResult Edit(AssetPart part, BuildPartEditorContext context)
    {
        return Initialize<AssetPartViewModel>(GetEditorShapeType(context), model => Populate(model, part));
    }

    public override async Task<IDisplayResult> UpdateAsync(AssetPart part, UpdatePartEditorContext context)
    {
        var model = new AssetPartViewModel();
        await context.Updater.TryUpdateModelAsync(model, Prefix);

        if (model.RatedPowerKw <= 0)
        {
            context.Updater.ModelState.AddModelError($"{Prefix}.{nameof(model.RatedPowerKw)}","Rated power must be greater than zero.");
        }

        if (model.CurrentPowerW < 0)
        {
            context.Updater.ModelState.AddModelError($"{Prefix}.{nameof(model.CurrentPowerW)}","Current power cannot be negative.");
        }

        part.RatedPowerKw = model.RatedPowerKw;
        part.CurrentPowerW = model.CurrentPowerW;
        part.RunningHours = model.RunningHours;

        return await EditAsync(part, context);
    }

    private static void Populate(AssetPartViewModel model, AssetPart part)
    {
        model.RatedPowerKw = part.RatedPowerKw;
        model.CurrentPowerW = part.CurrentPowerW;
        model.RunningHours = part.RunningHours;
    }
}