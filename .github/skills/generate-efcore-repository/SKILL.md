---
name: "generate-efcore-repository"
description: "Génère ou révise une couche EF Core avec configurations, requêtes, repositories et bonnes pratiques de performance."
---

# Skill — Generate EF Core Repository

## But

Créer une persistance EF Core claire et performante.

## Règles

- DbContext en Infrastructure.
- Mapping Fluent API explicite.
- Méthodes async.
- CancellationToken.
- Projection en lecture.
- Pas de IQueryable exposé par défaut.

## Sortie

- EntityTypeConfiguration
- Repository
- Méthodes de requête
- Tests d'intégration proposés
