namespace LoadMonitor.Core.ViewModels;

public class HomeIndexViewModel
{
    public IReadOnlyList<string> Categories { get; set; } = [];
    public string? SelectedCategory { get; set; }
    public IReadOnlyList<AssetCardViewModel> Assets { get; set; } = [];
}

public class AssetCardViewModel
{
    public string ContentItemId { get; set; } = "";
    public string DisplayText { get; set; } = "";
    public string? Category { get; set; }
    public double RatedPowerKw { get; set; }
    public double? CurrentPowerW { get; set; }

    public double? LoadPercent =>
        CurrentPowerW is double power && RatedPowerKw > 0 ? power / (RatedPowerKw * 1000) * 100 : null;
}