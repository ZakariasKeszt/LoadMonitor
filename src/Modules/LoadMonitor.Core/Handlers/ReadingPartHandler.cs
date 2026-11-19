using LoadMonitor.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;

namespace LoadMonitor.Core.Handlers
{
    public sealed class ReadingPartHandler(IServiceProvider serviceProvider) : ContentPartHandler<ReadingPart>
    {
        public override async Task PublishedAsync(PublishContentContext context, ReadingPart part)
        {
            if (string.IsNullOrEmpty(part.AssetContentItemId))
            {
                return;
            }

            var contentManager = serviceProvider.GetRequiredService<IContentManager>();

            var asset = await contentManager.GetAsync(part.AssetContentItemId, VersionOptions.Latest);
            if (asset == null)
            {
                return;
            }

            asset.Alter<AssetPart>(assetPart => assetPart.CurrentPowerW = part.PowerW);
            await contentManager.UpdateAsync(asset);
        }
    }
}
