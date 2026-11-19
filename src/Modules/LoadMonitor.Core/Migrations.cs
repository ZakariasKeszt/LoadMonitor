using System;
using System.Collections.Generic;
using System.Text;
using LoadMonitor.Core.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace LoadMonitor.Core;

/*
Structure from https://github.com/OrchardCMS/OrchardCore/blob/a8de5f7dee89b7f7123adeb1768088e6aacb7a5a/src/OrchardCore.Modules/OrchardCore.Demo/Migrations.cs.
Data migrations can be used to alter content type definitions. (Adding new types, parts and fields)
Methods return an int higher from the last run.
 
*/

public sealed class Migrations(IContentDefinitionManager contentDefinitionManager) : DataMigration
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
        await contentDefinitionManager.AlterPartDefinitionAsync(nameof(ReadingPart), part => part
            .WithDisplayName("Reading Part"));

        await contentDefinitionManager.AlterTypeDefinitionAsync("Reading", type => type.DisplayedAs("Reading").Creatable().Listable()
            .WithPart("TitlePart", part => part.WithPosition("0"))
            .WithPart(nameof(ReadingPart), part => part.WithPosition("1")));

        return 2;
    }
}