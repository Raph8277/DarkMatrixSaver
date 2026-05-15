# Matrix Screensaver — WPF .NET 10 + HTML

Package complet pour produire un écran de veille type **Matrix Rain** avec photo configurable, couleur/vitesse personnalisables, slideshow, effets CRT/scanlines/glow, mode multi-écran et preset **DemonBar Hell Matrix**.

## Contenu

```text
MatrixScreensaver/
├── src/
│   ├── MatrixScreensaver.Wpf/        # Screensaver WPF .NET 10
│   ├── MatrixScreensaver.ConfigEditor/ # Éditeur de configuration WPF
│   ├── MatrixScreensaver.Engine/     # Moteur Matrix + configuration
│   └── MatrixScreensaver.Packaging/  # Scripts packaging .scr
├── html/
│   └── matrix-screensaver.html       # Version HTML ultra fluide
├── presets/
│   ├── matrix.default.json
│   └── demonbar.hell.matrix.json
└── docs/
    ├── CONFIGURATION.md
    ├── PACKAGING-SCR.md
    └── ARCHITECTURE.md
```

## Prérequis

- Windows 10/11
- SDK .NET 10
- Visual Studio 2026 ou VS Code + extension C# Dev Kit

## Lancer le screensaver WPF

```bash
cd src/MatrixScreensaver.Wpf
dotnet run -- --config ../../presets/demonbar.hell.matrix.json
```

## Lancer l'éditeur de configuration

```bash
cd src/MatrixScreensaver.ConfigEditor
dotnet run
```

## Produire le `.scr`

```powershell
cd src/MatrixScreensaver.Packaging
./package-scr.ps1
```

Le fichier généré sera :

```text
artifacts/MatrixScreensaver.scr
```

Copie-le ensuite dans :

```text
C:\Windows\System32
```

Puis ouvre :

```text
Paramètres Windows > Personnalisation > Écran de verrouillage > Paramètres de l’écran de veille
```

## Variables principales

```json
{
  "background": {
    "mode": "Slideshow",
    "images": ["Assets/backgrounds/demonbar-cathedral.jpg"],
    "changeIntervalSeconds": 30
  },
  "matrix": {
    "glyphColor": "#00FF66",
    "headColor": "#D8FFE8",
    "fontSize": 22,
    "speed": 1.25,
    "density": 0.85,
    "alphabet": "Demon"
  },
  "effects": {
    "glow": true,
    "scanLines": true,
    "crt": true,
    "vignette": true
  }
}
```

## Notes

- Le mode `.scr` Windows est simplement un exécutable renommé en `.scr`.
- Les arguments standards `/s`, `/c`, `/p` sont prévus.
- Le mode prévisualisation `/p` est stubé proprement, car l’intégration preview Windows demande un host HWND spécifique.
