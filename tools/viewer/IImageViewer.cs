namespace KortAppZ.Tools.Viewer
{
    public interface IImageViewer
    {
        /// <summary>
        /// 加载图片文件
        /// </summary>
        /// <param name="filePath">图片文件路径</param>
        void LoadImage(string filePath);
        
        /// <summary>
        /// 显示图片
        /// </summary>
        void ShowImage();
        
        /// <summary>
        /// 关闭图片查看器
        /// </summary>
        void CloseViewer();
    }
}
