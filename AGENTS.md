# Agent / contributor notes (Jurupema)

## .NET projects: nullable reference types

Prefer **not** enabling `<Nullable>enable</Nullable>` in Jurupema .NET project files unless there is a deliberate team decision for a specific project. Keep the default (nullable disabled) for consistency with the existing API / domain / infrastructure style.

Avoid file-level nullable pragmas (`#nullable enable` / `#nullable disable`) in **hand-written code** across the solution when you can. Model optional reference types without `?` when nullable context is off. **Exception:** leave pragmas in **auto-generated** sources as produced by tools (for example **EF Core migrations** and designers); do not edit those files to remove them.

## Application layer: prefer DTOs over domain entities at the boundary

**Application services** (for example `ProductService`) should expose **DTOs or explicit result types** to callers—not raw domain entities (`Product`, etc.)—when the result is consumed by the API, other modules, or anything that serializes or versions a public contract.

### Why

- **Stable contract** — callers depend on shaped, intentional fields, not the aggregate’s internal or persistence-oriented surface.
- **No entity leakage** — avoids exposing EF tracking, proxies, and navigation properties that invite lazy-load mistakes or accidental over-sharing.
- **Consistency** — match existing patterns such as `CreateProductResult` for writes and `ProductListItemResult` / `PagedResult<T>` for reads.

### When returning entities might be acceptable

Returning a domain entity from a service can be reasonable for **strictly in-process** use where another layer in the same application truly needs the aggregate for domain rules—but default to DTOs first, especially for anything that crosses HTTP or module boundaries.

### Implementation hint

Load data via repositories (entities), **map to result/DTO types inside the application service** (or a dedicated mapper if the project grows), then return those types.
