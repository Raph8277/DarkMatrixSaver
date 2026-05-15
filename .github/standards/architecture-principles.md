# Principes d'architecture

## Principes structurants

1. Le métier prime sur la technique.
2. Les dépendances vont vers le centre.
3. Les contrats externes ne sont pas le modèle interne.
4. Les décisions structurantes sont documentées en ADR.
5. Les reviews sont systématiques pour les changements transverses.

## Architecture cible

```mermaid
flowchart TD
    UI[Blazor Client] --> API[gRPC API]
    API --> APP[Application]
    APP --> DOMAIN[Domain]
    INFRA[Infrastructure EF Core] --> APP
    INFRA --> DOMAIN
```
