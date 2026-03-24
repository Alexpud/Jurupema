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

## Application unit tests (`tests/Jurupema.Api.Application.Tests`)

### SUT and mocks

- Instantiate the **system under test (SUT)** once in the **test class constructor** (for example `ProductService`, `ProductImageService`).
- Resolve constructor dependencies with **`Moq.AutoMock`**: create an **`AutoMocker`**, then **`CreateInstance<T>()`** for the SUT. Configure collaborators with **`GetMock<TInterface>()`** for `.Setup(...)` and `.Verify(...)` instead of declaring many separate `new Mock<T>()` locals.
- **xUnit** constructs a **new test class instance per `[Fact]`**, so the constructor runs every time: each test gets a **fresh `AutoMocker` and SUT** without shared mock state.

### Identifiers in tests

- Prefer values that come from the **entities under test** (for example **`product.Id`**, **`image.Id`** after `new Product(...)` / `new ProductImage(...)`). With **client-generated `Guid`** keys, each new entity already has a distinct id—**avoid ad-hoc id assignment** (reflection, hardcoded ints) unless a scenario truly requires it.

### Broader test style

For **Arrange / Act / Assert**, **deterministic Bogus seeds**, and **`Assert.Multiple`**, follow [`.cursor/rules/dotnet-testing.mdc`](.cursor/rules/dotnet-testing.mdc) where it applies.
