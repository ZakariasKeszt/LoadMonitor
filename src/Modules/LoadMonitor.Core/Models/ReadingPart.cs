using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentManagement;

namespace LoadMonitor.Core.Models;

public class ReadingPart : ContentPart
{
    public string AssetContentItemId { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; //needs placeholder or a loop for testing chart later
    public double PowerW { get; set; }
}