---
name: "enforce-clean-architecture"
description: "Vérifie et applique les règles de Clean Architecture : dépendances entrantes, séparation des couches et isolation du domaine."
---

# Skill — Enforce Clean Architecture

## But

Contrôler que le code respecte la séparation des couches.

## Règles

- Domain ne référence aucun projet applicatif ou infrastructure.
- Application référence Domain uniquement.
- Infrastructure référence Application et Domain.
- Presentation ne contient pas de logique métier.
- Les dépendances externes sont derrière des interfaces.

## Sortie

Rapport court : conforme / non conforme / correctifs.
