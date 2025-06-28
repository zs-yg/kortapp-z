# Kortapp-z - Windows App Store -- Advocating for Open Source, Free Software, and No Ads

Setting a small flag: Never accept ads, continuously update.

## Open Source Practices

1. The project code is open source, allowing anyone to use, modify, distribute, or commercialize it, but the original author must be credited.  
2. The project documentation is open source, allowing anyone to use, modify, distribute, or commercialize it, but the original author must be credited.  
3. Project resources such as icons and screenshots are open source, allowing anyone to use, modify, distribute, or commercialize them, but the original author must be credited.  
4. Any derivatives of the project (including but not limited to websites, apps, plugins, etc.) must adhere to the above open-source license.  
5. The project does not accept any form of advertising, and ads must not be placed anywhere.  
6. The project does not accept any form of donations.  
7. The project does not accept any form of sponsorship.  
8. The project welcomes PRs (Pull Requests) in any form. Submitting issues is not required.  
9. You can submit PRs for your own projects. However, if the star count is below 1k, they will be deleted.  

## Project Overview

A simple Windows app store application that provides software download and management features.  
Offers software management, download management, built-in tools, and more.  

## Features

- Clean software download interface  
- Download progress management  
- Supports background downloads  
- Beautiful app card display  
- Functional and structured code handling  

## Build and Packaging

### System Requirements  
- .NET 8.0 SDK  
- Windows 10/11  

### Packaging Commands  

#### 32-bit Version  
```bash  
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true  
```  

#### 64-bit Version  
```bash  
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true  
```  

The packaged executable will include the specified application icon and be output to:  
```
bin\Release\net8.0-windows\[platform]\publish  
```

### Advanced Options  
- Add `--self-contained true` to generate a standalone package (larger size).  
- Add `-p:PublishTrimmed=true` to reduce package size (experimental).  

## Project Structure  

```
kortapp-z/  
├── MainForm.cs          # Main form logic  
├── DownloadManager.cs   # Download management  
├── AppCard.cs           # App card control  
├── DownloadItem.cs      # Download item control  
├── img/                 # Image resources  
│   ├── ico/             # Icon files  
│   └── png/             # App screenshots  
└── resource/            # Resource files  
    └── aria2c.exe       # Download tool  
```  

## Runtime Requirements  

- .NET 8.0 runtime (if using framework-dependent deployment)  
- Windows 10 or later  

## License  

MIT License  

Copyright (c) 2025 zsyg  

## Other Websites  

Gitee Mirror Repository: https://gitee.com/chr_super/kortapp-z (Currently no longer maintained)  

## Maintenance  

Since I rarely use Gitee and find it cumbersome to operate, the Gitee mirror will no longer be synced. If anyone knows how to set up a mirror, please help. Contact me via QQ: 3872006562 or DM me on Bilibili. I will acknowledge your help in the README. Thank you!  

Due to a conflict with Daye, Windowscleaner will never be listed. I will strive on my own.  

Note: Due to GitHub file upload restrictions, rename `img/png/NET.png` to `.NET.png`; otherwise, the program may encounter unexpected issues.  

Anyone can add their own legal software via PR.  

Author's Email:  
```
3872006562@qq.com  
```  

Author's QQ:  
```
3872006562  
```  

QQ Group:  
```
1043867176  
```  

Bilibili Account:  
```
Zayisynth  
```