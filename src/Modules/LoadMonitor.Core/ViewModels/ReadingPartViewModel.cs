using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LoadMonitor.Core.ViewModels;

public class ReadingPartViewModel
{
    public string AssetContentItemId { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public double PowerW { get; set; }

    [BindNever]
    public IEnumerable<SelectListItem> Assets { get; set; } = [];
}