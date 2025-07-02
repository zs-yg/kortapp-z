 // _              _                             
 //| | _____  _ __| |_ __ _ _ __  _ __       ____
 //| |/ / _ \| '__| __/ _` | '_ \| '_ \ ____|_  /
 //|   | (_) | |  | || (_| | |_) | |_) |_____/ / 
 //|_|\_\___/|_|   \__\__,_| .__/| .__/     /___|
 //                        |_|   |_|             
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AppStore
{
    public static class AppSearch
    {
        /// <summary>
        /// 搜索应用卡片
        /// </summary>
        /// <param name="flowPanel">包含应用卡片的FlowLayoutPanel</param>
        /// <param name="searchText">搜索文本</param>
        public static void SearchApps(FlowLayoutPanel flowPanel, string searchText)
        {
            if (flowPanel == null || string.IsNullOrWhiteSpace(searchText))
            {
                ShowAllApps(flowPanel);
                return;
            }

            foreach (Control control in flowPanel.Controls)
            {
                if (control is AppCard appCard)
                {
                    bool isMatch = IsMatchSearch(appCard.AppName, searchText);
                    control.Visible = isMatch;
                }
            }
        }

        /// <summary>
        /// 显示所有应用卡片
        /// </summary>
        public static void ShowAllApps(FlowLayoutPanel? flowPanel)
        {
            if (flowPanel == null) return;

            foreach (Control control in flowPanel.Controls)
            {
                control.Visible = true;
            }
        }

        /// <summary>
        /// 检查应用名称是否匹配搜索文本
        /// </summary>
        private static bool IsMatchSearch(string appName, string searchText)
        {
            if (string.IsNullOrEmpty(appName)) return false;

            // 不区分大小写比较
            return appName.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }
    }
}
