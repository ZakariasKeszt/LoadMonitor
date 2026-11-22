using LoadMonitor.Core.Models;
using LoadMonitor.Core.Services;
using LoadMonitor.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Taxonomies.Fields;
using OrchardCore.Taxonomies.Indexing;
using OrchardCore.Taxonomies.Models;
using OrchardCore.Taxonomies.Settings;
using YesSql;

namespace LoadMonitor.Core.Controllers;

public sealed class HomeController(
    ISession session,
    IContentManager contentManager,
    IContentDefinitionManager contentDefinitionManager,
    LoadReplayService replayService) : Controller
{
    private const string CategoryFieldName = "Category";

    public async Task<IActionResult> Index(string? category)
    {
        // Category terms: term content item id -> name 
        var terms = await GetCategoryTermsAsync();

        // Find the id of the selected category, if the name is valid.
        var selectedTermId = terms
            .Where(term => string.Equals(term.Value, category, StringComparison.OrdinalIgnoreCase))
            .Select(term => term.Key)
            .FirstOrDefault();

        // Filtering based on the slected term id
        var assets = selectedTermId is null
            ? await session.Query<ContentItem, ContentItemIndex>(x =>
                    x.ContentType == "Asset" && x.Published)
                .ListAsync()
            : await session.Query<ContentItem, TaxonomyIndex>(x =>
                    x.ContentType == "Asset" && x.Published && x.TermContentItemId == selectedTermId)
                .ListAsync();

        var cards = new List<AssetCardViewModel>();
        foreach (var asset in assets.OrderBy(asset => asset.DisplayText))
        {
            var part = asset.As<AssetPart>();
            if (part is null)
            {
                continue;
            }

            var termId = part.Get<TaxonomyField>(CategoryFieldName)?.TermContentItemIds.FirstOrDefault();

            cards.Add(new AssetCardViewModel
            {
                ContentItemId = asset.ContentItemId,
                DisplayText = asset.DisplayText,
                Category = termId is not null && terms.TryGetValue(termId, out var name) ? name : null,
                RatedPowerKw = part.RatedPowerKw,
                CurrentPowerW = string.IsNullOrEmpty(part.LoadProfile)
                    ? null
                    : await replayService.GetCurrentAsync(part.LoadProfile, asset.ContentItemId),
            });
        }

        return View(new HomeIndexViewModel
        {
            Categories = terms.Values.OrderBy(name => name).ToList(),
            SelectedCategory = selectedTermId is null ? null : terms[selectedTermId],
            Assets = cards,
        });
    }

    // Reads the taxonomy linked to the Category field and returns thefound terms
    private async Task<Dictionary<string, string>> GetCategoryTermsAsync()
    {
        var partDefinition = await contentDefinitionManager.GetPartDefinitionAsync(nameof(AssetPart));
        var settings = partDefinition?.Fields
            .FirstOrDefault(field => field.Name == CategoryFieldName)?
            .GetSettings<TaxonomyFieldSettings>();

        if (string.IsNullOrEmpty(settings?.TaxonomyContentItemId))
        {
            return new Dictionary<string, string>();
        }

        var taxonomy = await contentManager.GetAsync(settings.TaxonomyContentItemId, VersionOptions.Published);
        var terms = taxonomy?.As<TaxonomyPart>()?.Terms ?? [];

        return terms.ToDictionary(term => term.ContentItemId, term => term.DisplayText);
    }
}