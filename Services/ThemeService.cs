using System.IO;
using System.Windows;

namespace PharmaDesk.Services;

public class ThemeService : IThemeService
{
    private const string DarkTheme = "Themes/DarkTheme.xaml";
    private const string LightTheme = "Themes/LightTheme.xaml";
    private readonly string settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PharmaDesk",
        "theme.txt");

    public bool IsDark { get; private set; } = true;

    public void LoadSavedTheme()
    {
        if (File.Exists(settingsPath))
        {
            IsDark = !string.Equals(File.ReadAllText(settingsPath).Trim(), "Light", StringComparison.OrdinalIgnoreCase);
        }

        ApplyTheme();
    }

    public void ToggleTheme()
    {
        IsDark = !IsDark;
        ApplyTheme();
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);
        File.WriteAllText(settingsPath, IsDark ? "Dark" : "Light");
    }

    private void ApplyTheme()
    {
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        var existing = dictionaries.FirstOrDefault(x =>
            x.Source is not null &&
            (x.Source.OriginalString.EndsWith(DarkTheme, StringComparison.OrdinalIgnoreCase) ||
             x.Source.OriginalString.EndsWith(LightTheme, StringComparison.OrdinalIgnoreCase)));

        if (existing is not null)
        {
            dictionaries.Remove(existing);
        }

        dictionaries.Add(new ResourceDictionary
        {
            Source = new Uri(IsDark ? DarkTheme : LightTheme, UriKind.Relative)
        });
    }
}
