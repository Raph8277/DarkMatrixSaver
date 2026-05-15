# Compatibilité VS Code / GitHub Copilot — Agents & Skills

Cette version ajoute les entêtes YAML obligatoires ou recommandés pour que les profils d'agents et les skills soient interprétables par Copilot.

## Agents

Les agents sont placés dans :

```text
.github/agents/*.agent.md
```

Chaque fichier commence par un frontmatter :

```yaml
---
name: "nom-agent"
description: "Description courte utilisée par Copilot pour sélectionner l'agent."
---
```

## Skills

Les skills sont placées dans :

```text
.github/skills/<skill-name>/SKILL.md
```

Chaque `SKILL.md` commence par :

```yaml
---
name: "nom-skill"
description: "Description courte utilisée par Copilot pour décider quand charger la skill."
---
```

## Règle d'optimisation

Ne pas ajouter de `tools` ou `allowed-tools` par défaut tant que l'environnement exact n'est pas stabilisé. Laisser Copilot/VS Code exposer les outils disponibles évite de rendre un agent inutilisable à cause d'un nom d'outil non supporté.
