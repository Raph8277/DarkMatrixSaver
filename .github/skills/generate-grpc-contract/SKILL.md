---
name: "generate-grpc-contract"
description: "Génère un contrat gRPC .proto, les règles de versioning, les messages, services, erreurs et mappings associés."
---

# Skill — Generate gRPC Contract

## But

Créer un contrat gRPC stable et découplé du modèle Domain.

## Règles

- Messages simples.
- Services orientés cas d'usage.
- Versionner les packages proto.
- Gérer erreurs métier avec status gRPC.
- Mapper explicitement entre DTO et Application.

## Sortie

- fichier `.proto`
- service serveur
- client typé
- mapping
- tests recommandés
