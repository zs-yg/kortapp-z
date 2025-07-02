using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Security;
using System.Diagnostics;

namespace AppStore
{
    /// <summary>
    /// 提供Windows自启动项管理功能
    /// </summary>
    public static class SelfStartingManager
    {
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        
        /// <summary>
        /// 获取所有自启动项
        /// </summary>
        public static Dictionary<string, string> GetAllStartupItems()
        {
            var items = new Dictionary<string, string>();
            
            try
            {
                Logger.Log("开始获取自启动项...");
                
                using (var key = Registry.CurrentUser.OpenSubKey(RunKey))
                {
                    if (key != null)
                    {
                        Logger.Log($"正在读取注册表键: {RunKey}");
                        foreach (var valueName in key.GetValueNames())
                        {
                            var value = key.GetValue(valueName)?.ToString() ?? string.Empty;
                            Logger.Log($"找到自启动项: {valueName} = {value}");
                            items.Add(valueName, value);
                        }
                    }
                }
                
                Logger.Log($"共找到 {items.Count} 个自启动项");
            }
            catch (SecurityException ex)
            {
                Logger.LogError("访问注册表时权限不足", ex);
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError("获取自启动项时发生错误", ex);
                throw;
            }
            
            return items;
        }

        /// <summary>
        /// 添加自启动项
        /// </summary>
        public static bool AddStartupItem(string name, string path)
        {
            try
            {
                Logger.Log($"正在添加自启动项: {name} = {path}");
                
                using (var key = Registry.CurrentUser.OpenSubKey(RunKey, true))
                {
                    key?.SetValue(name, path, RegistryValueKind.String);
                    key?.Flush();
                    Logger.Log($"成功添加自启动项: {name}");
                    
                    // 验证是否添加成功
                    var verifyValue = key?.GetValue(name)?.ToString();
                    if (verifyValue != path)
                    {
                        Logger.LogError($"验证失败: 自启动项 {name} 未正确添加", null);
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"添加自启动项失败: {name}", ex);
                return false;
            }
        }

        /// <summary>
        /// 删除自启动项
        /// </summary>
        public static bool RemoveStartupItem(string name)
        {
            try
            {
                Logger.Log($"正在删除自启动项: {name}");
                
                using (var key = Registry.CurrentUser.OpenSubKey(RunKey, true))
                {
                    // 先获取值用于日志记录
                    var value = key?.GetValue(name)?.ToString() ?? "";
                    
                    key?.DeleteValue(name, false);
                    key?.Flush();
                    Logger.Log($"已删除自启动项: {name} = {value}");
                    
                    // 验证是否删除成功
                    if (key?.GetValue(name) != null)
                    {
                        Logger.LogError($"验证失败: 自启动项 {name} 未正确删除", null);
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"删除自启动项失败: {name}", ex);
                return false;
            }
        }
    }
}