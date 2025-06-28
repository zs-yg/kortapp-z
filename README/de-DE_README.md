# Kortapp-z - Windows App Store  
**Open-Source, kostenlos, werbefrei**  

Grundsatz: Keine Werbung, kontinuierliche Updates  

## Open-Source-Richtlinien  

1. **Open-Source-Code**: Nutzung, Modifikation, Verbreitung und kommerzielle Nutzung erlaubt, mit Pflicht zur Nennung des Originalautors.  
2. **Open-Source-Dokumentation**: Gleiche Bedingungen wie für Code.  
3. **Open-Source-Ressourcen** (Icons, Screenshots): Gleiche Bedingungen.  
4. Alle abgeleiteten Produkte (Websites, Apps, Erweiterungen etc.) müssen diese Lizenz einhalten.  
5. **Keine Werbung** – in keiner Form.  
6. **Keine Spenden** werden angenommen.  
7. **Keine Sponsoring** wird angenommen.  
8. **Pull Requests (PR) sind willkommen**, auch ohne vorheriges Issue.  
9. Eigene Projekte können per PR eingereicht werden, aber solche mit unter **1000 Stars** werden gelöscht.  

## Projektbeschreibung  

Eine einfache Windows-App zum Herunterladen und Verwalten von Software.  

**Hauptfunktionen:**  
- Softwareverwaltung  
- Download-Manager  
- Integrierte Tools  

## Hauptmerkmale  

- Minimalistische Download-Oberfläche  
- Download-Fortschrittsverfolgung  
- Hintergrund-Downloads  
- Visuell ansprechende App-Karten  
- Sauberer und strukturierter Code  

## Build und Packaging  

### Systemanforderungen  
- **.NET 8.0 SDK**  
- **Windows 10/11**  

### Build-Befehle  

#### 32-Bit-Version  
```bash
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true
```  

#### 64-Bit-Version  
```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true
```  

Erstellte Dateien werden unter folgendem Pfad abgelegt:  
```
bin\Release\net8.0-windows\[Plattform]\publish
```  

### Zusätzliche Optionen  
- `--self-contained true` – eigenständige Builds (größere Dateien)  
- `-p:PublishTrimmed=true` – Größenreduzierung (experimentell)  

## Projektstruktur  

```
kortapp-z/  
├── MainForm.cs          # Hauptfenster-Logik  
├── DownloadManager.cs   # Download-Manager  
├── AppCard.cs           # App-Karte  
├── DownloadItem.cs      # Download-Element  
├── img/                 # Grafiken  
│   ├── ico/             # Icons  
│   └── png/             # Screenshots  
└── resource/            # Ressourcen  
    └── aria2c.exe       # Download-Tool  
```  

## Laufzeitanforderungen  

- **.NET 8.0 Runtime** (bei Framework-abhängiger Build)  
- **Windows 10 oder neuer**  

## Lizenz  

**MIT License**  
Copyright (c) 2025 zsyg  

## Andere Plattformen  

**Gitee-Mirror**: [https://gitee.com/chr_super/kortapp-z](https://gitee.com/chr_super/kortapp-z) (nicht mehr gewartet)  

## Wartung  

Der Gitee-Repository wird nicht mehr synchronisiert. Hilfe beim Mirroring ist willkommen:  
- **QQ**: 3872006562  
- **Bilibili (Direktnachricht)**: Zayisynth  

**Wichtig:**  
- Aufgrund eines Konflikts mit Daye wird **windowscleaner** nie hinzugefügt.  
- Benennen Sie `img/png/NET.png` in `.NET.png` um, um Fehler zu vermeiden (GitHub-Beschränkungen).  

## Mitwirken  

Jeder kann legale Software per Pull Request vorschlagen.  

## Kontakt  

📧 **Email**:  
```
3872006562@qq.com
```  

📱 **QQ**:  
```
3872006562
```  

👥 **QQ-Gruppe**:  
```
1043867176
```  

🎥 **Bilibili**:  
```
Zayisynth
```  

---

### Wie sollte die deutsche README benannt werden?  
Empfohlene Optionen:  
1. **de-DE_README.md** (IETF-Standard)  
2. **README_de.md** (kürzere Variante)  
3. **DE_README.md** (explizite Sprachkennung)  

Am besten halten Sie sich an das Format `[Sprachcode]_README.md`, wie bei anderen Übersetzungen.