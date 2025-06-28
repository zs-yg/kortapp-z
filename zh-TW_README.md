# Kortapp-z - Windows應用商店 --主張軟體開源、免費，拒絕廣告

小立一個flag：從不接受廣告，不停更新

## 專案開源行為

1. 專案程式碼開源，允許任何人使用、修改、分發、商用，但必須註明原作者。
2. 專案文件開源，允許任何人使用、修改、分發、商用，但必須註明原作者。
3. 專案圖示、截圖等資源開源，允許任何人使用、修改、分發、商用，但必須註明原作者。
4. 專案的任何衍生品（包括但不限於網站、APP、外掛等）必須遵循以上開源協議。
5. 專案不接受任何形式的廣告，不得在任何地方投放廣告。
6. 專案不接受任何形式的捐贈。
7. 專案不接受任何形式的贊助。
8. 專案可以進行PR，歡迎任何形式的PR，不提交issue也可以
9. 本專案可以PR一些你自己的專案，如果star數量不到1k，都會被刪除

## 專案簡介

一個簡單的Windows應用商店應用，提供軟體下載和管理功能。
提供軟體管理、下載管理、內建工具使用等功能

## 功能特點

- 簡潔的軟體下載介面
- 下載進度管理
- 支援後台下載
- 美觀的應用卡片展示
- 功能化、結構化的程式碼處理

## 構建與打包

### 系統要求
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

打包後的可執行文件將包含指定的應用程式圖示，輸出路徑為：
```
bin\Release\net8.0-windows\[platform]\publish
```

### 進階選項
- 添加`--self-contained true`可生成獨立包（體積較大）
- 添加`-p:PublishTrimmed=true`可減小包體積（實驗性）

## 專案結構

```
kortapp-z/
├── MainForm.cs          # 主視窗邏輯
├── DownloadManager.cs   # 下載管理
├── AppCard.cs           # 應用卡片控制項
├── DownloadItem.cs      # 下載項控制項
├── img/                 # 圖片資源
│   ├── ico/             # 圖示文件
│   └└── png/             # 應用截圖
└└── resource/            # 資源文件
    └└── aria2c.exe       # 下載工具
```

## 執行要求

- .NET 8.0執行時（如果使用框架依賴發布）
- Windows 10或更高版本

## 授權許可

MIT License

Copyright (c) 2025 zsyg

## 其他網站

gitee鏡像倉庫:https://gitee.com/chr_super/kortapp-z  (目前已停止維護)

## 維護

由於gitee我沒怎麼用，而且操作麻煩，gitee鏡像將不會繼續同步，有懂得人可以幫我搞下鏡像嗎，qq： 3872006562，也可以b站直接私信，我會在readme中鳴謝的，謝謝各位
由於和Daye發生了矛盾，所以windowscleaner將永遠不上架，我要自己努力
提示：由於github上傳文件的限制，img/png/NET.png，請改名為.NET.png，否則程式可能出現無法預料的問題

每一個人都可以通過PR添加屬於自己的合法軟體

作者郵箱:
```
3872006562@qq.com
```

作者qq號:
```
3872006562
```

qq群：
```
1043867176
```

b站帳號：
```
Zayisynth
```