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
        public enum IconSize
        {
            Large,
            Small
        }

        public static Icon ExtractIconFromFile(string filePath, int iconIndex = 0, IconSize size = IconSize.Large)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            // 增强文件检查
            try
            {
                // 规范化路径
                filePath = Path.GetFullPath(filePath);
                
                // 检查文件属性（比File.Exists更可靠）
                var fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException($"文件不存在或无法访问: {filePath}\n" +
                        $"当前工作目录: {Environment.CurrentDirectory}\n" +
                        $"绝对路径: {Path.GetFullPath(filePath)}");
                }

                // 尝试打开文件测试是否被锁定
                try
                {
                    using (var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        // 文件可访问
                    }
                }
                catch (IOException ioEx)
                {
                    throw new IOException($"文件被锁定或无法访问: {filePath}", ioEx);
                }
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException($"无法访问文件: {filePath}\n" +
                    $"错误详情: {ex.Message}", ex);
            }

            // 尝试从索引0到3提取图标
            for (int i = 0; i < 4; i++)
            {
                IntPtr[] hLargeIcons = new IntPtr[1];
                IntPtr[] hSmallIcons = new IntPtr[1];
                
                int extractedCount = ExtractIconEx(filePath, i, hLargeIcons, hSmallIcons, 1);

                if (extractedCount > 0)
                {
                    IntPtr hIcon = size == IconSize.Large ? hLargeIcons[0] : hSmallIcons[0];
                    if (hIcon == IntPtr.Zero)
                    {
                        // 回退到另一种尺寸
                        hIcon = size == IconSize.Large ? hSmallIcons[0] : hLargeIcons[0];
                    }

                    if (hIcon != IntPtr.Zero)
                    {
                        try
                        {
                            Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
                            return icon;
                        }
                        finally
                        {
                            if (hLargeIcons[0] != IntPtr.Zero) DestroyIcon(hLargeIcons[0]);
                            if (hSmallIcons[0] != IntPtr.Zero) DestroyIcon(hSmallIcons[0]);
                        }
                    }
                }
            }

            // 尝试使用ExtractAssociatedIcon作为后备方案
            try
            {
                return Icon.ExtractAssociatedIcon(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"无法从文件中提取图标(尝试了索引0-3和ExtractAssociatedIcon)", ex);
            }
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
