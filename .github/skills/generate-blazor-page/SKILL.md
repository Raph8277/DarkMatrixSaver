---
name: "generate-blazor-page"
description: "Génère une page ou un composant Blazor/MudBlazor structuré, testable, accessible et aligné avec les conventions projet."
---

# Skill — Generate Blazor Page

## But

Créer une page Blazor avec MudBlazor selon les standards du projet.

## Règles

- ViewModel dédié.
- États loading/error/empty/success.
- Services injectés.
- Pas de logique métier lourde dans Razor.
- Formulaire validé.

## Sortie

- `.razor`
- `.razor.cs` si code-behind utile
- ViewModel
- service client si nécessaire
