using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LoadMonitor.Core.ViewModels;

//viewmodel for AssetPart
public class AssetPartViewModel
{
    public double RatedPowerKw { get; set; }
    public double RunningHours { get; set; }
    [BindNever]
    public double? CurrentPowerW { get; set; }

    //percentage calculation, not stored in database
    public double? LoadPercent => CurrentPowerW is double power && RatedPowerKw > 0 ? power / (RatedPowerKw * 1000) * 100 : 0;
    public string LoadProfile { get; set; } = "";

    [BindNever]
    public IEnumerable<SelectListItem> Profiles { get; set; } = [];
}