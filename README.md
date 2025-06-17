# Kortapp-z - Windows应用商店 --主张软件开源、免费，拒绝广告

小立一个flag：从不接受广告，不停更新

## 项目开源行为

1. 项目代码开源，允许任何人使用、修改、分发、商用，但必须注明原作者。
2. 项目文档开源，允许任何人使用、修改、分发、商用，但必须注明原作者。
3. 项目图标、截图等资源开源，允许任何人使用、修改、分发、商用，但必须注明原作者。
4. 项目的任何衍生品（包括但不限于网站、APP、插件等）必须遵循以上开源协议。
5. 项目不接受任何形式的广告，不得在任何地方投放广告。
6. 项目不接受任何形式的捐赠。
7. 项目不接受任何形式的赞助。
8. 项目可以进行PR，欢迎任何形式的PR，不提交issue也可以
9. 本项目可以PR一些你自己的项目，如果star数量不到1k，都会被删除

## 项目简介

![image](https://github.com/user-attachments/assets/721c63bb-44aa-444b-9c92-bdf1b919374e)

一个简单的Windows应用商店应用，提供软件下载和管理功能。

## 功能特点

- 简洁的软件下载界面
- 下载进度管理
- 支持后台下载
- 美观的应用卡片展示

## 构建与打包

### 系统要求
- .NET 8.0 SDK
- Windows 10/11

### 打包指令

#### 32位版本
```bash
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true
```

#### 64位版本
```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true
```

打包后的可执行文件将包含指定的应用程序图标，输出路径为：
```
bin\Release\net8.0-windows\[platform]\publish
```

### 高级选项
- 添加`--self-contained true`可生成独立包（体积较大）
- 添加`-p:PublishTrimmed=true`可减小包体积（实验性）

## 项目结构

```
AppStore/
├── MainForm.cs          # 主窗体逻辑
├── DownloadManager.cs   # 下载管理
├── AppCard.cs           # 应用卡片控件
├── DownloadItem.cs      # 下载项控件
├── img/                 # 图片资源
│   ├── ico/             # 图标文件
│   └── png/             # 应用截图
└── resource/            # 资源文件
    └── aria2c.exe       # 下载工具
```

## 运行要求

- .NET 8.0运行时（如果使用框架依赖发布）
- Windows 10或更高版本

## 许可证

MIT License

Copyright (c) 2025 kortapp-z项目组

## 维护

每一个人都可以通过PR添加属于自己的合法软件

qq群：
```
1043867176
```

b站账号：
```
Zayisynth
```
