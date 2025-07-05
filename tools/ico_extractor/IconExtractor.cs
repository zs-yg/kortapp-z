using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AppStore.Tools.IconExtractor
{
    public static class IconExtractor
    {
        /// <summary>
        /// 获取ICO文件中的实际尺寸列表
        /// </summary>
        public static List<int> GetIconDimensions(string filePath)
        {
            var sizes = new List<int>();
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(fs))
                {
                    // 读取ICO文件头
                    reader.ReadUInt16(); // 保留字段
                    ushort type = reader.ReadUInt16(); // 1=ICO, 2=CUR
                    ushort count = reader.ReadUInt16(); // 图像数量
                    
                    if (type != 1) return sizes; // 不是ICO文件

                    // 读取每个图像目录项
                    for (int i = 0; i < count; i++)
                    {
                        byte width = reader.ReadByte();
                        byte height = reader.ReadByte();
                        reader.ReadBytes(14); // 跳过其他字段

                        // 宽度/高度为0表示256像素
                        int size = width == 0 ? 256 : width;
                        sizes.Add(size);
                    }
                }
            }
            catch
            {
                // 忽略所有错误，返回空列表
            }
            return sizes;
        }
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int ExtractIconEx(
            string lpszFile, 
            int nIconIndex, 
            [AllowNull] IntPtr[] phiconLarge, 
            [AllowNull] IntPtr[] phiconSmall, 
            int nIcons);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        /// <summary>
        /// 从文件中提取图标
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="iconIndex">图标索引</param>
        /// <param name="largeIcon">是否提取大图标</param>
        /// <returns>提取的图标</returns>
        public static Icon ExtractIconFromFile(string filePath, int iconIndex = 0)
        {
            IntPtr[] hIcons = new IntPtr[1];
            int extractedCount = ExtractIconEx(filePath, iconIndex, hIcons, null, 1);

            if (extractedCount <= 0 || hIcons[0] == IntPtr.Zero)
                throw new FileNotFoundException("无法从文件中提取图标");

            // 直接返回原始图标
            Icon icon = (Icon)Icon.FromHandle(hIcons[0]).Clone();
            DestroyIcon(hIcons[0]);
            return icon;
        }

        /// <summary>
        /// 将图标保存为文件
        /// </summary>
        /// <param name="icon">图标对象</param>
        /// <param name="outputPath">输出路径</param>
        public static void SaveIconToFile(Icon icon, string outputPath)
        {
            using (FileStream fs = new FileStream(outputPath, FileMode.Create))
            {
                icon.Save(fs);
            }
        }

        /// <summary>
        /// 获取文件中的图标数量
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>图标数量</returns>
        public static int GetIconCount(string filePath)
        {
            return ExtractIconEx(filePath, -1, null, null, 0);
        }
    }
}
