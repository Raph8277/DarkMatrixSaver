# Instructions globales GitHub Copilot — Squad IA Enterprise

## Langue

Répondre en français par défaut. Utiliser l'anglais uniquement pour les noms techniques, API, code, conventions de framework ou documentation générée explicitement en anglais.

## Contexte projet

Le projet de référence est une application de réservation de livres basée sur :

- .NET 10
- Clean Architecture
- Domain-Driven Design
- Blazor + MudBlazor
- gRPC
- EF Core
- SQLite
- tests automatisés
- documentation C4 / ADR / Mermaid

## Règles générales

- Ne pas produire de code sans expliciter les hypothèses.
- Ne pas mélanger couche Domain, Application, Infrastructure et Presentation.
- Ne pas injecter EF Core directement dans la couche Domain.
- Ne pas exposer les entités Domain directement dans les contrats gRPC.
- Ne pas créer d'agent généraliste si un agent spécialisé existe.
- Ne pas relire tout le dépôt si un contexte ciblé suffit.
- Toujours privilégier une sortie courte, structurée, vérifiable et actionnable.

## Architecture cible

```text
src/
├── Bookings.Domain/
├── Bookings.Application/
├── Bookings.Infrastructure/
├── Bookings.GrpcServer/
├── Bookings.BlazorClient/
└── Bookings.Worker/

tests/
├── Bookings.Domain.Tests/
├── Bookings.Application.Tests/
├── Bookings.Infrastructure.Tests/
└── Bookings.Architecture.Tests/

docs/
├── adr/
├── c4/
├── decisions/
├── workflows/
└── reviews/
```

## Contrat de réponse par défaut

Toute réponse d'agent doit suivre ce format lorsque pertinent :

```markdown
# Résultat

## Hypothèses

## Proposition

## Impact architecture

## Risques

## Prochaines actions
```

## Politique token

- Lire uniquement les fichiers nécessaires.
- Résumer avant d'étendre.
- Préférer les diff ciblés aux réécritures complètes.
- Utiliser des checklists plutôt que de longs paragraphes.
- Utiliser les skills comme capsules de connaissance spécialisées.
