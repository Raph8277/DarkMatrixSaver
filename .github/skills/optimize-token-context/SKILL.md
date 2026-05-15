---
name: "optimize-token-context"
description: "Réduit le contexte transmis aux agents en appliquant une stratégie de sélection, résumé, découpage et budget token."
---

# Skill — Optimize Token Context

## But

Réduire la consommation token des agents.

## Méthode

1. Identifier le périmètre exact.
2. Lire uniquement les fichiers utiles.
3. Résumer le contexte en 10 lignes maximum.
4. Éviter les réécritures complètes.
5. Produire un diff ou une checklist.
6. Utiliser un contrat de sortie.

## Anti-patterns

- Lire tout le repo.
- Demander une analyse globale sans objectif.
- Mélanger génération et review.
- Produire une documentation longue non demandée.
