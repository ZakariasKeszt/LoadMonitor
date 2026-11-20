using LoadMonitor.Core.Models;
using YesSql.Indexes;

namespace LoadMonitor.Core.Index
{
    public class LoadSampleIndex : MapIndex
    {
        public string Profile { get; set; } = "";
        public int Sequence { get; set; }
        public double PowerW { get; set; }
    }

    public sealed class LoadSampleIndexProvider : IndexProvider<LoadSample>
    {
        public override void Describe(DescribeContext<LoadSample> context) => context.For<LoadSampleIndex>().Map(sample => new LoadSampleIndex
        {
            Profile = sample.Profile,
            Sequence = sample.Sequence,
            PowerW = sample.PowerW
        });
    }
}
