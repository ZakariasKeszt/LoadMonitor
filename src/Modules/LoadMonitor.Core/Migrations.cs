using System;
using System.Collections.Generic;
using System.Text;
using LoadMonitor.Core.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using LoadMonitor.Core.Index;
using LoadMonitor.Core.Services;
using YesSql.Sql;
using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
using OrchardCore.Taxonomies.Settings;
using OrchardCore.Title.Models;


namespace LoadMonitor.Core;

/*
Structure from https://github.com/OrchardCMS/OrchardCore/blob/a8de5f7dee89b7f7123adeb1768088e6aacb7a5a/src/OrchardCore.Modules/OrchardCore.Demo/Migrations.cs.
Data migrations can be used to alter content type definitions. (Adding new types, parts and fields)
Methods return an int higher from the last run.
 
*/

public sealed class Migrations(IContentDefinitionManager contentDefinitionManager, LoadProfileImporter loadProfileImporter, IContentManager contentManager) : DataMigration
{
    public async Task<int> CreateAsync()
    {
        await contentDefinitionManager.AlterPartDefinitionAsync(nameof(AssetPart), part => part
            .WithDisplayName("Asset Part"));
        //creating the Asset type with TitlePart and AssetPart content parts
        await contentDefinitionManager.AlterTypeDefinitionAsync("Asset", type => type.DisplayedAs("Asset").Creatable().Listable().Draftable()
            .WithPart("TitlePart", part => part.WithPosition("0"))
            .WithPart(nameof(AssetPart), part => part.WithPosition("1")));

        return 1;
    }
    public async Task<int> UpdateFrom1Async()
    {
        await SchemaBuilder.CreateMapIndexTableAsync<LoadSampleIndex>(table => table
            .Column<string>(nameof(LoadSampleIndex.Profile), column => column.WithLength(64))
            .Column<int>(nameof(LoadSampleIndex.Sequence))
            .Column<double>(nameof(LoadSampleIndex.PowerW)));

        await SchemaBuilder.AlterIndexTableAsync<LoadSampleIndex>(table => table
            .CreateIndex("IDX_LoadSampleIndex_Profile_Sequence",
                "DocumentId",
                nameof(LoadSampleIndex.Profile),
                nameof(LoadSampleIndex.Sequence)));

        return 2;
    }
    public async Task<int> UpdateFrom2Async()
    {
        await loadProfileImporter.ImportAllAsync();
        return 3;
    }
    public async Task<int> UpdateFrom3Async()
    {
        // Term content type: every category is a small "Category" content item with a title.
        await contentDefinitionManager.AlterTypeDefinitionAsync("Category", type => type
            .DisplayedAs("Category")
            .WithPart("TitlePart", part => part.WithPosition("0")));

        // The taxonomy is a content item that holds the list of terms.
        var taxonomy = await contentManager.NewAsync("Taxonomy");
        taxonomy.DisplayText = "Asset categories";
        taxonomy.Alter<TitlePart>(part => part.Title = "Asset categories");

        // Build the terms, each one pointing back to its taxonomy.
        var terms = new List<ContentItem>();
        foreach (var name in new[] { "Pump", "Motor" })
        {
            var term = await contentManager.NewAsync("Category");
            term.DisplayText = name;
            term.Alter<TitlePart>(part => part.Title = name);
            term.Alter<TermPart>(part => part.TaxonomyContentItemId = taxonomy.ContentItemId);
            terms.Add(term);
        }

        taxonomy.Alter<TaxonomyPart>(part =>
        {
            part.TermContentType = "Category";
            part.Terms = terms;
        });

        await contentManager.CreateAsync(taxonomy, VersionOptions.Published);

        // Category field on AssetPart: exactly one term, required, from this taxonomy.
        await contentDefinitionManager.AlterPartDefinitionAsync(nameof(AssetPart), part => part
            .WithField("Category", field => field
                .OfType("TaxonomyField")
                .WithDisplayName("Category")
                .WithSettings(new TaxonomyFieldSettings
                {
                    TaxonomyContentItemId = taxonomy.ContentItemId,
                    Unique = true,
                    Required = true,
                })));

        return 4;
    }
}