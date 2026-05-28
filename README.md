# TheDinner — API REST Restaurant

[![CI](https://github.com/Shiro-Momon/TheDinner/actions/workflows/ci.yml/badge.svg)](https://github.com/Shiro-Momon/TheDinner/actions/workflows/ci.yml)
[![Coverage](https://img.shields.io/badge/coverage-see_artifacts-blue)](https://github.com/Shiro-Momon/TheDinner/actions)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=your-project-key&metric=alert_status)](https://sonarcloud.io/project/overview?id=your-project-key)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=your-project-key&metric=bugs)](https://sonarcloud.io/project/overview?id=your-project-key)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=your-project-key&metric=code_smells)](https://sonarcloud.io/project/overview?id=your-project-key)

---

## Description

Système de digitalisation de la prise de commande et des paiements pour un restaurant.  
Le backend expose une **API REST en .NET 9 / ASP.NET Core** suivant une **Clean Architecture** en 4 couches.  
Les clients passent commande via une interface web (voir [The-Dinner-Front](../The-Dinner-Front)), les commandes transitent par un cycle de vie géré côté serveur, et les paiements sont traités selon la méthode choisie (carte, espèces, ticket restaurant).

**Objectif pédagogique :** projet académique Ynov M1 Full Stack — évaluation CI/CD & DevOps.

---

## Stack technique

| Couche | Technologie | Justification |
|--------|-------------|---------------|
| Runtime | .NET 9 / ASP.NET Core 9 | LTS, performances élevées, écosystème riche |
| ORM | Entity Framework Core 9 + PostgreSQL (Npgsql) | Migrations versionées, LINQ type-safe |
| Tests | xUnit + NSubstitute + FluentAssertions + Coverlet | Standard .NET, mocking sans dépendances, assertions lisibles |
| Linting | `dotnet format` + StyleCop + `.editorconfig` | Cohérence du style, vérifiable en CI |
| Logging | Serilog (JSON structuré) | Corrélation par propriétés, compatible avec les agrégateurs de logs |
| Monitoring | prometheus-net + Grafana | Métriques exposées sur `/metrics`, dashboard provisionné automatiquement |
| Conteneur | Docker multi-stage Alpine + docker-compose | Images Alpine : runtime API ~100 MB, frontend ~70 MB — orchestration locale complète (API + frontend + BDD + monitoring) |
| Qualité | SonarCloud + Dependabot | Analyse statique continue, alertes de vulnérabilités |
| Sécurité | Trivy (FS + image) | Détection de CVE critiques dans le filesystem et l'image Docker |

---

## Architecture

```
RestaurantOrder.slnx
├── src/
│   ├── RestaurantOrder.Domain          # Entités, enums, events, exceptions — ZÉRO dépendance
│   ├── RestaurantOrder.Application     # Services, interfaces, DTOs, stratégies
│   ├── RestaurantOrder.Infrastructure  # EF Core, repositories, strategies, event dispatching
│   └── RestaurantOrder.API             # Controllers, middleware, Program.cs
└── tests/
    ├── RestaurantOrder.UnitTests       # xUnit — Domain + Application (mocks NSubstitute)
    └── RestaurantOrder.IntegrationTests # xUnit — API end-to-end (WebApplicationFactory + SQLite)
```

**Règle de dépendance (sens unique) :**

```
RestaurantOrder.API
        │
        ▼
RestaurantOrder.Infrastructure  ──────────────────┐
        │                                          │
        ▼                                          │
RestaurantOrder.Application  ◄─────────────────────┘
        │
        ▼
RestaurantOrder.Domain   (aucune dépendance)
```

**Design Patterns implémentés :**

| Pattern | Où | Pourquoi |
|---------|----|----------|
| Repository | `Application/Interfaces/Repositories/` + `Infrastructure/Repositories/` | Découple la logique métier de la persistance ; testable avec des repos in-memory |
| Strategy | `IPricingStrategy` / `PricingStrategyFactory` | Algorithmes de prix interchangeables (Standard, Happy Hour, Group Discount) sans modifier les services |
| Observer | `IDomainEvent` / `IDomainEventHandler<T>` | Déclenche des effets de bord (notif cuisine, logs) sans coupler l'entité Order aux handlers |
| Factory | `PaymentFactory` | Crée le bon processeur de paiement selon la méthode ; centralise la validation |

**Cycle de vie d'une commande :**

```
Pending ──► Confirmed ──► Preparing ──► Ready ──► Served ──► Paid
   │             │              │           │
   └─────────────┴──────────────┴───────────┴────────────────► Cancelled
```

---

## Lancer le projet en local

### Prérequis
- [Docker](https://www.docker.com/) ≥ 24
- [docker-compose](https://docs.docker.com/compose/) v2

### Démarrage complet (API + Frontend + PostgreSQL + Prometheus + Grafana)

```bash
git clone https://github.com/Shiro-Momon/TheDinner.git
cd TheDinner
docker-compose up --build
```

| Service | URL | Image |
|---------|-----|-------|
| Frontend (Next.js) | http://localhost:3001 | `node:20-alpine` ~70 MB |
| API REST | http://localhost:8080 | `aspnet:9.0-alpine` ~100 MB |
| Scalar (docs interactives) | http://localhost:8080/scalar | — |
| Health check | http://localhost:8080/health | — |
| Métriques Prometheus | http://localhost:8080/metrics | — |
| Prometheus UI | http://localhost:9090 | `prom/prometheus:v2.51.0` |
| Grafana | http://localhost:3000 (admin / admin) | `grafana/grafana:10.4.2` |

> **Premier build du frontend :** Docker clone le dépôt [The-Dinner-Front](https://github.com/Shiro-Momon/The-Dinner-Front) et installe les dépendances npm (~3–5 min). Les builds suivants utilisent le cache Docker.  
> Pour forcer une mise à jour du frontend : `docker-compose build --no-cache frontend`

### Sans Docker (développement)

```bash
# Prérequis : .NET 9 SDK + PostgreSQL local
dotnet restore RestaurantOrder.slnx
dotnet run --project src/RestaurantOrder.API
```

Configurer la chaîne de connexion dans `src/RestaurantOrder.API/appsettings.Development.json` :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=restaurantorder;Username=postgres;Password=postgres"
  }
}
```

---

## Lancer les tests

```bash
# Tous les tests avec couverture
dotnet test RestaurantOrder.slnx --collect:"XPlat Code Coverage"

# Tests unitaires uniquement
dotnet test tests/RestaurantOrder.UnitTests

# Tests d'intégration uniquement
dotnet test tests/RestaurantOrder.IntegrationTests
```

Les rapports de couverture sont générés au format Cobertura dans `**/coverage.cobertura.xml`.

### Lint

```bash
dotnet format RestaurantOrder.slnx --verify-no-changes
```

---

## Endpoints principaux

| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/menu` | Liste du menu |
| POST | `/api/menu` | Créer un plat |
| GET | `/api/tables` | Liste des tables |
| POST | `/api/orders` | Créer une commande |
| PATCH | `/api/orders/{id}/confirm` | Confirmer |
| PATCH | `/api/orders/{id}/serve` | Servir |
| POST | `/api/payments` | Traiter le paiement |
| GET | `/health` | Health check |
| GET | `/metrics` | Métriques Prometheus |

Documentation complète disponible sur Scalar : http://localhost:8080/scalar

---

## Pipeline CI/CD

```
push / pull_request → main
           │
           ▼
┌──────────────────────────────────┐
│          build-and-test          │
│  1. dotnet restore               │
│  2. dotnet format --verify       │  ← bloque si le style ne passe pas
│  3. dotnet build (Release)       │
│  4. Unit tests + coverage        │
│  5. Integration tests + coverage │
│  6. Upload coverage artifacts    │
└────────────┬─────────────────────┘
             │ needs: build-and-test
     ┌───────┴────────┐
     │                │
     ▼                ▼
┌──────────────┐  ┌──────────────────────┐
│ docker-build │  │      sonarcloud       │
│              │  │  (continue-on-error)  │
│ docker build │  │  SonarScanner begin  │
│ Trivy FS     │  │  dotnet build        │
│ Trivy image  │  │  SonarScanner end    │
│ (CRITICAL)   │  │                      │
└──────────────┘  └──────────────────────┘
```

**Règles :**
- `build-and-test` bloque les deux jobs suivants — un build ou test cassé empêche tout merge
- `docker-build` échoue si Trivy détecte une CVE critique non corrigée
- `sonarcloud` est en `continue-on-error: true` (token optionnel) — ne bloque pas le pipeline si non configuré
- Dependabot surveille les dépendances NuGet et les actions GitHub

**Secrets requis pour SonarCloud :**

| Secret | Description |
|--------|-------------|
| `SONAR_TOKEN` | Token d'authentification SonarCloud |
| `SONAR_PROJECT_KEY` | Clé du projet SonarCloud |
| `SONAR_ORG` | Organisation SonarCloud |

---

## Variables d'environnement

| Variable | Valeur par défaut | Description |
|----------|-------------------|-------------|
| `ConnectionStrings__DefaultConnection` | (voir docker-compose) | Chaîne PostgreSQL |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Environnement ASP.NET Core |
