# Politique de budget token

## Objectif

Réduire les coûts et augmenter la précision des agents.

## Règles

| Situation | Politique |
|---|---|
| Bug ciblé | Lire uniquement fichier + tests liés |
| Feature | Lire spec + interfaces + fichiers de couche concernée |
| Architecture | Lire structure solution + dépendances + docs existantes |
| Documentation | Lire uniquement périmètre documenté |
| Review | Lire diff + règles + tests |

## Anti-patterns

- Scanner tout le repo sans objectif.
- Coller plusieurs milliers de lignes dans un prompt.
- Demander à un agent de tout faire.
- Réutiliser une conversation saturée au lieu de repartir d'un contexte propre.
