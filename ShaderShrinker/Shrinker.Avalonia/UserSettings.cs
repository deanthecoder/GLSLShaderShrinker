using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Shrinker.Parser;
// ReSharper disable UnusedMember.Global

namespace Shrinker.Avalonia;

/// <summary>
/// Persistent application user settings.
/// </summary>
public class UserSettings : IDisposable
{
    private readonly string m_filePath;

    [JsonIgnore]
    public static UserSettings Instance { get; } = new();

    public string Preset { get; set; } = string.Empty;
    public bool OutputAsGlsl { get; set; } = true;
    public string ShadertoyId { get; set; } = string.Empty;
    public string CustomPresetJson { get; set; } = JsonConvert.SerializeObject(CustomOptions.None());

    private UserSettings()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var companyName = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute))).Company;
        var productName = assembly.GetName().Name;

        // Ensure valid file system characters.
        companyName = RemoveInvalidChars(companyName);
        productName = RemoveInvalidChars(productName);

        var appDirectory = Path.Combine(GetAppDataPath(), companyName, productName);
        Directory.CreateDirectory(appDirectory);
        m_filePath = Path.Combine(appDirectory, "settings.json");

        if (File.Exists(m_filePath))
            JsonConvert.PopulateObject(File.ReadAllText(m_filePath), this);
    }

    private string RemoveInvalidChars(string s)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            s = s.Replace(c.ToString(), string.Empty);
        return s;
    }

    private static string GetAppDataPath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        if (string.IsNullOrEmpty(appDataPath))
        {
            var homePath = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(homePath))
            {
                // Fallback to using ~ if HOME environment variable is not set
                homePath = "~";
            }

            appDataPath = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? Path.Combine(homePath, "Library", "Preferences") : homePath;
        }

        Directory.CreateDirectory(appDataPath);

        return appDataPath;
    }

    public void Dispose() =>
        File.WriteAllText(m_filePath, JsonConvert.SerializeObject(this, Formatting.Indented));
}