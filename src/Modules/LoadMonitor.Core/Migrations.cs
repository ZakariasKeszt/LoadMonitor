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
namespace LoadMonitor.Core;

/*
Structure from https://github.com/OrchardCMS/OrchardCore/blob/a8de5f7dee89b7f7123adeb1768088e6aacb7a5a/src/OrchardCore.Modules/OrchardCore.Demo/Migrations.cs.
Data migrations can be used to alter content type definitions. (Adding new types, parts and fields)
Methods return an int higher from the last run.
 
*/

public sealed class Migrations(IContentDefinitionManager contentDefinitionManager, LoadProfileImporter loadProfileImporter) : DataMigration
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
}