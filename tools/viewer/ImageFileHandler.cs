using System;
using System.Drawing;
using System.IO;

namespace KortAppZ.Tools.Viewer
{
    public static class ImageFileHandler
    {
        private static readonly string[] SupportedExtensions = 
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".tif"
        };
        
        public static bool IsImageFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
                
            string extension = Path.GetExtension(filePath).ToLower();
            return Array.Exists(SupportedExtensions, ext => ext == extension);
        }
        
        public static Image LoadImage(string filePath)
        {
            if (!IsImageFile(filePath))
                throw new ArgumentException("不支持的图片格式");
                
            if (!File.Exists(filePath))
                throw new FileNotFoundException("图片文件不存在", filePath);
                
            try
            {
                return Image.FromFile(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"无法加载图片: {ex.Message}", ex);
            }
        }
    }
}
