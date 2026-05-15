# GitHub Copilot Agent Squad — Workshop IA Enterprise

Squad complète pour un workshop GitHub Copilot / agents / skills orienté architectes seniors et équipes .NET.

Objectif : démontrer une approche industrielle de l'IA appliquée au delivery logiciel : orchestration, spécialisation, DDD, Clean Architecture, Blazor, gRPC, EF Core, SQLite, QA, sécurité, documentation C4 et gouvernance.

## Structure

```text
.github/
├── copilot-instructions.md
├── agents/
│   ├── 00-orchestrator.agent.md
│   ├── 01-product-owner.agent.md
│   ├── 02-architecture-ddd.agent.md
│   ├── 03-domain-model.agent.md
│   ├── 04-efcore-data.agent.md
│   ├── 05-grpc-api.agent.md
│   ├── 06-blazor-mudblazor.agent.md
│   ├── 07-qa-review.agent.md
│   ├── 08-security-review.agent.md
│   ├── 09-performance-review.agent.md
│   ├── 10-retrodoc-c4.agent.md
│   ├── 11-devops-packaging.agent.md
│   └── 12-research-advisor.agent.md
├── skills/
│   ├── create-adr/
│   ├── create-c4-documentation/
│   ├── enforce-clean-architecture/
│   ├── enforce-ddd/
│   ├── generate-blazor-page/
│   ├── generate-efcore-repository/
│   ├── generate-grpc-contract/
│   ├── generate-tests/
│   ├── optimize-token-context/
│   └── review-security/
├── workflows/
│   ├── feature-pipeline.md
│   ├── modernization-pipeline.md
│   ├── review-pipeline.md
│   └── token-budget-policy.md
├── templates/
│   ├── agent-template.md
│   ├── skill-template.md
│   ├── adr-template.md
│   ├── c4-component-template.md
│   └── review-report-template.md
└── standards/
    ├── architecture-principles.md
    ├── coding-standards-dotnet.md
    ├── ddd-rules.md
    └── output-contracts.md
```

## App fil rouge du workshop

Application de réservation de livres :

- `TLivre`
- `TPersonne`
- `TAdresse`
- `TReservation`

Stack cible :

- .NET 10
- DDD
- Clean Architecture
- EF Core 10
- SQLite
- gRPC
- Blazor
- MudBlazor
- GitHub Copilot agents + skills

## Principe de conception

Cette squad suit 5 règles :

1. Un orchestrateur coordonne, mais ne code pas tout.
2. Les agents experts ont un périmètre strict.
3. Les skills sont atomiques, réutilisables et peu coûteux en tokens.
4. Les outputs sont contractuels : format, sections, niveau de détail.
5. Les reviews sont séparées de la génération.

## Utilisation recommandée

1. Copier `.github/` à la racine du dépôt cible.
2. Adapter `.github/copilot-instructions.md` au contexte projet.
3. Utiliser `00-orchestrator.agent.md` comme point d'entrée.
4. Déclencher les skills uniquement sur besoin explicite.
5. Faire passer toute génération significative dans le pipeline de review.


## Compatibilité Copilot

Les profils `.github/agents/*.agent.md` et les `.github/skills/*/SKILL.md` contiennent désormais un frontmatter YAML `name` + `description`, nécessaire à la découverte et au routage par GitHub Copilot / VS Code.

Voir `docs/compatibilite-vscode-copilot.md`.
