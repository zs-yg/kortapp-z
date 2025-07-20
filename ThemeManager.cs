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
                // 忽略错误,使用默认主题
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
        private static readonly Color LightBackground = Color.FromArgb(250, 250, 250);
        private static readonly Color LightControlBackground = Color.FromArgb(245, 245, 245);
        private static readonly Color LightText = Color.FromArgb(40, 40, 40);
        private static readonly Color LightButtonHover = Color.FromArgb(235, 235, 235);
        private static readonly Color LightButtonActive = Color.FromArgb(225, 225, 225);
        private static readonly Color LightAccent = Color.FromArgb(0, 120, 215);
        private static readonly Color LightAccentLight = Color.FromArgb(0, 150, 245);
        
        // 深色主题颜色
        private static readonly Color DarkBackground = Color.FromArgb(25, 25, 25);
        private static readonly Color DarkControlBackground = Color.FromArgb(40, 40, 40);
        private static readonly Color DarkText = Color.FromArgb(245, 245, 245);
        private static readonly Color DarkButtonHover = Color.FromArgb(55, 55, 55);
        private static readonly Color DarkButtonActive = Color.FromArgb(65, 65, 65);
        private static readonly Color DarkBorder = Color.FromArgb(70, 70, 70);
        private static readonly Color DarkAccent = Color.FromArgb(0, 150, 245);
        private static readonly Color DarkAccentLight = Color.FromArgb(0, 180, 255);
        
        // 浅色主题边框颜色
        private static readonly Color LightBorder = Color.FromArgb(200, 200, 200);

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

        public static Color BorderColor => 
            _currentTheme == ThemeMode.Light ? LightBorder : DarkBorder;

        public static Color AccentColor =>
            _currentTheme == ThemeMode.Light ? LightAccent : DarkAccent;

        public static Color AccentLightColor =>
            _currentTheme == ThemeMode.Light ? LightAccentLight : DarkAccentLight;

        public static int ControlRadius => 8;

        public static int FormRadius => 12;

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
                button.BackColor = ControlBackgroundColor;
                button.Font = new Font(button.Font, FontStyle.Bold);
                button.Padding = new Padding(10, 5, 10, 5);
            }

            foreach (Control childControl in control.Controls)
            {
                ApplyThemeToControl(childControl);
            }
        }
    }
}
