namespace LoadMonitor.Core.ViewModels;

public class AssetChartViewModel
{
    public IReadOnlyList<string> Labels { get; set; } = [];
    public IReadOnlyList<double> PowerW { get; set; } = [];
    public double RatedPowerW { get; set; }
}