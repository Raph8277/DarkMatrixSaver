# Packaging Windows `.scr`

Un écran de veille Windows est un exécutable renommé en `.scr`.

## Commandes standard

| Argument | Usage |
|---|---|
| `/s` | Lancement plein écran |
| `/c` | Configuration |
| `/p <HWND>` | Preview dans la fenêtre Windows |

Le projet gère `/s` et `/c`. `/p` est détectable mais la vraie prévisualisation nécessite d'héberger WPF dans le HWND fourni par Windows.

## Générer

```powershell
cd src/MatrixScreensaver.Packaging
./package-scr.ps1
```

## Installer

Copier :

```text
artifacts/MatrixScreensaver.scr
```

vers :

```text
C:\Windows\System32
```
