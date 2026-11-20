using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LoadMonitor.Core.ViewModels;

//viewmodel for AssetPart
public class AssetPartViewModel
{
    public double RatedPowerKw { get; set; }
    public double CurrentPowerW { get; set; }
    public double RunningHours { get; set; }

    //percentage calculation, not stored in database
    public double LoadPercent => RatedPowerKw > 0 ? CurrentPowerW / (RatedPowerKw * 1000) * 100 : 0;
    public string LoadProfile { get; set; } = "";

    [BindNever]
    public IEnumerable<SelectListItem> Profiles { get; set; } = [];
}