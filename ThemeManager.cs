using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;

namespace AppStore
{
    public static class ThemeManager
    {
        public enum ThemeMode
        {
            Light,
            Dark
        }

        private static readonly string ThemeConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "zsyg", "kortapp-z", ".date", "theme.json");

        private static ThemeMode _currentTheme = LoadTheme();

        private static ThemeMode LoadTheme()
        {
            try
            {
                if (File.Exists(ThemeConfigPath))
                {
                    var json = File.ReadAllText(ThemeConfigPath);
                    return JsonSerializer.Deserialize<ThemeMode>(json);
                }
            }
            catch
            {
                // 忽略错误，使用默认主题
            }
            return ThemeMode.Light;
        }

        private static void SaveTheme(ThemeMode theme)
        {
            try
            {
                var dir = Path.GetDirectoryName(ThemeConfigPath);
                if (dir == null) return;
                
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var json = JsonSerializer.Serialize(theme);
                File.WriteAllText(ThemeConfigPath, json);
            }
            catch
            {
                // 忽略错误
            }
        }
        
        // 浅色主题颜色
        private static readonly Color LightBackground = Color.FromArgb(255, 255, 255);
        private static readonly Color LightControlBackground = Color.FromArgb(240, 240, 240);
        private static readonly Color LightText = Color.FromArgb(30, 30, 30);
        private static readonly Color LightButtonHover = Color.FromArgb(230, 230, 230);
        private static readonly Color LightButtonActive = Color.FromArgb(220, 220, 220);
        
        // 深色主题颜色
        private static readonly Color DarkBackground = Color.FromArgb(30, 30, 30);
        private static readonly Color DarkControlBackground = Color.FromArgb(45, 45, 45);
        private static readonly Color DarkText = Color.FromArgb(240, 240, 240);
        private static readonly Color DarkButtonHover = Color.FromArgb(60, 60, 60);
        private static readonly Color DarkButtonActive = Color.FromArgb(70, 70, 70);

        public static event Action<ThemeMode> ThemeChanged = delegate {};

        public static ThemeMode CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    ThemeChanged?.Invoke(value);
                    SaveTheme(value);
                }
            }
        }

        public static Color BackgroundColor => 
            _currentTheme == ThemeMode.Light ? LightBackground : DarkBackground;

        public static Color ControlBackgroundColor => 
            _currentTheme == ThemeMode.Light ? LightControlBackground : DarkControlBackground;

        public static Color TextColor => 
            _currentTheme == ThemeMode.Light ? LightText : DarkText;

        public static Color ButtonHoverColor => 
            _currentTheme == ThemeMode.Light ? LightButtonHover : DarkButtonHover;

        public static Color ButtonActiveColor => 
            _currentTheme == ThemeMode.Light ? LightButtonActive : DarkButtonActive;

        public static void ApplyTheme(Control control)
        {
            ApplyThemeToControl(control);
        }

        private static void ApplyThemeToControl(Control control)
        {
            control.BackColor = BackgroundColor;
            control.ForeColor = TextColor;

            if (control is Button button)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = ButtonHoverColor;
                button.FlatAppearance.MouseDownBackColor = ButtonActiveColor;
            }

            foreach (Control childControl in control.Controls)
            {
                ApplyThemeToControl(childControl);
            }
        }
    }
}
