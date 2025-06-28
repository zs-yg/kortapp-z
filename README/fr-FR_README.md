# Kortapp-z - Microsoft Store -- Logiciel open source, gratuit et sans publicité

Engagement : aucune publicité acceptée, mises à jour continues

## Politique open source

1. Code open source : utilisation, modification, distribution et usage commercial autorisés, avec mention de l'auteur original obligatoire.
2. Documentation open source : mêmes conditions que le code.
3. Ressources graphiques (icônes, captures d'écran) open source : mêmes conditions.
4. Tout produit dérivé (sites web, apps, extensions...) doit respecter cette licence.
5. Aucune publicité n'est acceptée, sous aucune forme.
6. Aucun don n'est accepté.
7. Aucun sponsoring n'est accepté.
8. Les Pull Requests (PR) sont bienvenues, même sans issue préalable.
9. Vous pouvez soumettre vos projets via PR, mais ceux avec moins de 1k stars seront supprimés.

## Présentation

Une application simple de boutique Windows pour télécharger et gérer des logiciels.  
Fonctionnalités :
- Gestion de logiciels
- Gestion de téléchargements
- Outils intégrés

## Fonctionnalités clés

- Interface épurée pour le téléchargement
- Gestion de la progression des téléchargements
- Téléchargements en arrière-plan
- Présentation sous forme de cartes visuelles
- Code structuré et modulaire

## Compilation et packaging

### Prérequis
- SDK .NET 8.0
- Windows 10/11

### Commandes

#### Version 32-bit
```bash
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true
```

#### Version 64-bit
```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true
```

Fichiers générés :
```
bin\Release\net8.0-windows\[platform]\publish
```

### Options avancées
- `--self-contained true` : package autonome (taille plus importante)
- `-p:PublishTrimmed=true` : réduction de taille (expérimental)

## Structure du projet

```
kortapp-z/
├── MainForm.cs          # Logique de la fenêtre principale
├── DownloadManager.cs   # Gestion des téléchargements
├── AppCard.cs           # Contrôle des cartes d'applications
├── DownloadItem.cs      # Contrôle des éléments de téléchargement
├── img/                 # Ressources visuelles
│   ├── ico/             # Icônes
│   └── png/             # Captures d'écran
└── resource/            # Fichiers ressources
    └── aria2c.exe        # Outil de téléchargement
```

## Prérequis d'exécution

- Runtime .NET 8.0 (si version dépendante du framework)
- Windows 10 ou supérieur

## Licence

MIT License

Copyright (c) 2025 zsyg

## Autres plateformes

Dépôt miroir Gitee : https://gitee.com/chr_super/kortapp-z (maintenance arrêtée)

## Maintenance

Le dépôt Gitee n'étant plus maintenu par manque d'utilisation, toute aide pour la synchronisation est bienvenue.  
Contact : QQ 3872006562 ou message privé sur Bilibili (mention dans le README en remerciement).

Note : Suite à un conflit avec Daye, l'application windowscleaner ne sera jamais publiée ici.

Important : Renommez `img/png/NET.png` en `.NET.png` pour éviter des problèmes (limitations GitHub).

## Contributions

Tout utilisateur peut ajouter des logiciels légaux via PR.

## Contacts

Email :  
```
3872006562@qq.com
```

QQ :  
```
3872006562
```

Groupe QQ :  
```
1043867176
```

Compte Bilibili :  
```
Zayisynth
```