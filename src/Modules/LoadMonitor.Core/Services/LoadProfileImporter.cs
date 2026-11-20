using System.Globalization;
using LoadMonitor.Core.Models;
using YesSql;

namespace LoadMonitor.Core.Services;

// Reads CSV load profiles and stores every row as a LoadSample document.
public sealed class LoadProfileImporter(ISession session)
{
    private const string TimestampFormat = "yyyy.MM.dd H:mm";

    public async Task ImportAllAsync()
    {
        var assembly = typeof(LoadProfileImporter).Assembly;
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

        foreach (var resourceName in resourceNames)
        {
            // "...Data.pump-load.csv" -> "pump-load"
            var withoutExtension = resourceName[..^".csv".Length];
            var profile = withoutExtension[(withoutExtension.LastIndexOfAny(['.', '/', '\\', '>']) + 1)..]; //Profile name was Data>load-pump, need to add '>'

            await using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            // Skip header line.
            await reader.ReadLineAsync();

            var sequence = 0;
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var fields = line.Split(';');
                if (fields.Length < 2 || string.IsNullOrWhiteSpace(fields[0]))
                {
                    continue;
                }

                if (!DateTime.TryParseExact(fields[0].Trim(), TimestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var timestamp)
                    || !double.TryParse(fields[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var powerW))
                {
                    continue;
                }

                await session.SaveAsync(new LoadSample
                {
                    Profile = profile,
                    Sequence = sequence++,
                    Timestamp = timestamp,
                    PowerW = powerW,
                });
            }
        }
    }
}