namespace AppStore.Tools.IconExtractor
{
    public static class IconExtractorConstants
    {
        public const string FileFilter = "可执行文件|*.exe;*.dll;*.ocx;*.cpl|所有文件|*.*";
        public const string SaveFilter = "图标文件|*.ico|位图文件|*.bmp|PNG文件|*.png";
        
        public static readonly int[] SupportedSizes = { 16, 24, 32, 48, 64, 128, 256, 512 };
        public static readonly int DefaultExtractSize = 256;
        
        public const string ErrorNoIconsFound = "文件不包含任何图标";
        public const string ErrorExtractionFailed = "图标提取失败";
        public const string ErrorInvalidIndex = "无效的图标索引";
        public const string ErrorFileNotFound = "文件未找到";
        
        public const int MaxRecentFiles = 5;
        public const int DefaultPreviewSize = 128;
    }
}
