using LoadMonitor.Core.Models;
using LoadMonitor.Core.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using YesSql;

namespace LoadMonitor.Core.Drivers;

public sealed class ReadingPartDisplayDriver(ISession session) : ContentPartDisplayDriver<ReadingPart>
{
    public override IDisplayResult Display(ReadingPart part, BuildPartDisplayContext context)
    {
        return Initialize<ReadingPartViewModel>(GetDisplayShapeType(context), model => Populate(model, part))
            .Location("Detail", "Content:5")
            .Location("Summary", "Content:5");
    }

    public override IDisplayResult Edit(ReadingPart part, BuildPartEditorContext context)
    {
        return Initialize<ReadingPartViewModel>(GetEditorShapeType(context), async model =>
        {
            Populate(model, part);

            var assets = await session
                .Query<ContentItem, ContentItemIndex>(x => x.ContentType == "Asset" && x.Published)
                .ListAsync();

            model.Assets = assets.Select(asset => new SelectListItem(asset.DisplayText, asset.ContentItemId));
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(ReadingPart part, UpdatePartEditorContext context)
    {
        var model = new ReadingPartViewModel();
        await context.Updater.TryUpdateModelAsync(model, Prefix);

        if (string.IsNullOrWhiteSpace(model.AssetContentItemId))
        {
            context.Updater.ModelState.AddModelError(
                $"{Prefix}.{nameof(model.AssetContentItemId)}",
                "Please choose an asset.");
        }

        if (model.PowerW < 0)
        {
            context.Updater.ModelState.AddModelError(
                $"{Prefix}.{nameof(model.PowerW)}",
                "Power cannot be negative.");
        }

        part.AssetContentItemId = model.AssetContentItemId;
        part.Timestamp = model.Timestamp;
        part.PowerW = model.PowerW;

        return await EditAsync(part, context);
    }

    private static void Populate(ReadingPartViewModel model, ReadingPart part)
    {
        model.AssetContentItemId = part.AssetContentItemId;
        model.Timestamp = part.Timestamp;
        model.PowerW = part.PowerW;
    }
}