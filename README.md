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

一个简单的Windows应用商店应用，提供软件下载和管理功能。
提供软件管理、下载管理、内置工具使用等功能

## 功能特点

- 简洁的软件下载界面
- 下载进度管理
- 支持后台下载
- 美观的应用卡片展示
- 功能化、结构化的代码处理

## 构建与打包

### 系统要求
- .NET 8.0 SDK
- Windows 10/11

### 打包指令


#### 32位版本
```bash
dotnet publish AppStore.csproj -c Release -r win-x86 --self-contained false /p:Optimize=true /p:DebugType=None
```

#### 64位版本
```bash
dotnet publish AppStore.csproj -c Release -r win-x64 --self-contained false /p:Optimize=true /p:DebugType=None
```

打包后的可执行文件将包含指定的应用程序图标，输出路径为：
```
bin\Release\net8.0-windows\[platform]\publish
```

### 高级选项
- 使用`--self-contained false`生成框架依赖包（默认）
- 使用`/p:Optimize=true`启用代码优化（默认）
- 使用`/p:DebugType=None`禁用调试符号生成（默认）
- 添加`-p:PublishTrimmed=true`可减小包体积（实验性）

## 项目结构

```
kortapp-z/
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

Copyright (c) 2025 zsyg

## 其他网站

gitee镜像仓库:https://gitee.com/chr_super/kortapp-z  (目前已经停止维护)

## 维护

由于gitee我没怎么用，而且操作麻烦，gitee镜像将不会继续同步，有懂得人可以帮我搞下镜像吗，qq： 3872006562，也可以b站直接私信，我会在readme中鸣谢的，谢谢各位
由于和Daye发生了矛盾，所以windowscleaner将永远不上架，我要自己努力
提示：由于github上传文件的限制，img/png/NET.png，请改名为.NET.png，否则程序可能出现无法预料的问题

每一个人都可以通过PR添加属于自己的合法软件

作者邮箱:
```
3872006562@qq.com
```

作者qq号:
```
3872006562
```

qq群：
```
1043867176
```

b站账号：
```
Zayisynth
```
