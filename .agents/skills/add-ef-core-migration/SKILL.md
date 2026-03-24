---
name: add-ef-core-migration
description: Adds a new Entity Framework Core migration for Jurupema using Jurupema.Api as the startup project and Jurupema.Api.Infrastructure as the migrations project. Use when the user asks to add an EF migration, scaffold a migration, or run dotnet ef migrations add for this solution.
---

# EF Core: add migration (Jurupema)

## When this applies

The user provides a **migration name** (or you derive one from the task). They want a new migration in this repository.

## Layout (do not guess)

- **Startup project** (`-s`): `src/Jurupema.Api/Jurupema.Api.csproj` — hosts configuration and builds the app for design-time.
- **Migrations project** (`-p` / `--project`): `src/Jurupema.Api.Infrastructure/Jurupema.Api.Infrastructure.csproj` — contains `JurupemaDbContext` and `Data/Migrations/`.
- **Migrations output folder** (`--output-dir` / `-o`): `Data/Migrations` — **relative to the migrations project** (not the repo root). That maps to `src/Jurupema.Api.Infrastructure/Data/Migrations/` in the tree.

Always pass **`-p`**, **`-s`**, and **`--output-dir Data/Migrations`**. Using only the startup project can target the wrong assembly or fail to emit migrations under `Infrastructure`.

## Command

Run from the **repository root** (directory containing `src/`).

Replace `{migrationName}` with the user-supplied name (PascalCase is typical, e.g. `AddOrderStatusColumn`).

```bash
dotnet ef migrations add {migrationName} -p src/Jurupema.Api.Infrastructure/Jurupema.Api.Infrastructure.csproj -s src/Jurupema.Api/Jurupema.Api.csproj --output-dir Data/Migrations
```

## Steps for the agent

1. Confirm the migration name with the user if it was vague; otherwise use the name they gave.
2. Execute the command above from the repo root (use the shell; fix path separators only if the shell requires it on Windows).
3. If the tool is missing, suggest: `dotnet tool install --global dotnet-ef` (or update to a version compatible with the solution’s EF packages).
4. After success, point to new files under `src/Jurupema.Api.Infrastructure/Data/Migrations/`.

## Notes

- Do not hand-edit `#nullable` directives in generated migration files; treat them as tool output (see repo `AGENTS.md`).
- Review the generated migration for correctness before committing.
