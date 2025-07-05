using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace AppStore.Tools.IconExtractor
{
    public static class IconExtractorHelper
    {
        /// <summary>
        /// 将图标转换为位图
        /// </summary>
        public static Bitmap ConvertIconToBitmap(Icon icon, Size size)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawIcon(icon, new Rectangle(0, 0, size.Width, size.Height));
            }
            return bitmap;
        }

        /// <summary>
        /// 批量提取文件中的所有图标
        /// </summary>
        public static List<Icon> ExtractAllIcons(string filePath)
        {
            List<Icon> icons = new List<Icon>();
            int count = IconExtractor.GetIconCount(filePath);
            
            for (int i = 0; i < count; i++)
            {
                try
                {
                    Icon icon = IconExtractor.ExtractIconFromFile(filePath, i);
                    icons.Add(icon);
                }
                catch
                {
                    // 忽略提取失败的图标
                }
            }
            
            return icons;
        }

        /// <summary>
        /// 将图标保存为PNG格式
        /// </summary>
        public static void SaveIconAsPng(Icon icon, string outputPath)
        {
            using (Bitmap bitmap = icon.ToBitmap())
            {
                bitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>
        /// 检查文件是否包含图标
        /// </summary>
        public static bool HasIcons(string filePath)
        {
            try
            {
                return IconExtractor.GetIconCount(filePath) > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
