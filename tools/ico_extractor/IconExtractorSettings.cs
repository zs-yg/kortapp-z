using System;
using System.IO;
using System.Text.Json;

namespace AppStore.Tools.IconExtractor
{
    public static class IconExtractorSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "kortapp-z", "icon_extractor_settings.json");

        public static SettingsData CurrentSettings { get; private set; } = new SettingsData();

        public class SettingsData
        {
            public string LastUsedDirectory { get; set; } = string.Empty;
            public string DefaultSaveFormat { get; set; } = "ico";
            public int DefaultIconSize { get; set; } = 128;
            public string[] RecentFiles { get; set; } = Array.Empty<string>();
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        try
                        {
                            using (JsonDocument doc = JsonDocument.Parse(json))
                            {
                                var root = doc.RootElement;
                                CurrentSettings = new SettingsData
                                {
                                    LastUsedDirectory = root.TryGetProperty("LastUsedDirectory", out var dir) ? dir.GetString() ?? string.Empty : string.Empty,
                                    DefaultSaveFormat = root.TryGetProperty("DefaultSaveFormat", out var format) ? format.GetString() ?? "ico" : "ico",
                                    DefaultIconSize = root.TryGetProperty("DefaultIconSize", out var size) ? size.GetInt32() : 128,
                                    RecentFiles = root.TryGetProperty("RecentFiles", out var files) ? 
                                        JsonSerializer.Deserialize<string[]>(files.GetRawText()) ?? Array.Empty<string>() : 
                                        Array.Empty<string>()
                                };
                            }
                        }
                        catch
                        {
                            CurrentSettings = new SettingsData();
                        }
                    }
                }
            }
            catch
            {
                // 加载失败时使用默认设置
                CurrentSettings = new SettingsData();
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public static void SaveSettings()
        {
            try
            {
                if (string.IsNullOrEmpty(SettingsPath))
                    return;

                string directory = Path.GetDirectoryName(SettingsPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(CurrentSettings);
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // 忽略保存错误
            }
        }

        /// <summary>
        /// 添加最近使用的文件
        /// </summary>
        public static void AddRecentFile(string filePath)
        {
            if (CurrentSettings.RecentFiles.Length >= 5)
            {
                var list = new List<string>(CurrentSettings.RecentFiles);
                list.RemoveAt(0);
                CurrentSettings.RecentFiles = list.ToArray();
            }

            var newList = new List<string>(CurrentSettings.RecentFiles);
            if (!newList.Contains(filePath))
            {
                newList.Add(filePath);
                CurrentSettings.RecentFiles = newList.ToArray();
                SaveSettings();
            }
        }

        /// <summary>
        /// 更新最后使用的目录
        /// </summary>
        public static void UpdateLastUsedDirectory(string directory)
        {
            if (Directory.Exists(directory) && CurrentSettings.LastUsedDirectory != directory)
            {
                CurrentSettings.LastUsedDirectory = directory;
                SaveSettings();
            }
        }
    }
}
