# Architecture & Design Summary

## Clean Architecture

![Clean Architecture Diagram 1](https://blog.cleancoder.com/uncle-bob/images/2012-08-13-the-clean-architecture/CleanArchitecture.jpg)

### What is Clean Architecture?

Clean Architecture is an approach proposed by Robert C. Martin (“Uncle Bob”) that organises a system into concentric layers (or rings) so that the **core business logic** is isolated from external concerns (UI, frameworks, databases). :contentReference[oaicite:1]{index=1}  
Key idea: Dependencies **point inwards**. Outer layers depend on inner layers; inner layers know nothing about outer layers. :contentReference[oaicite:2]{index=2}

### Why use Clean Architecture?

- Business/domain code becomes independent of UI, persistence, databases or frameworks — making it more maintainable. :contentReference[oaicite:3]{index=3}
- Easier to test (you can test domain logic without database or UI).
- Adaptable: you can change the external layers (e.g., switch UI technology, DB) without touching core logic.

### Layers (example breakdown)

- **Entities / Domain**: The innermost layer; business rules, enterprise-wide concepts.
- **Use Cases / Application**: Orchestrates application-specific business rules, interacts with domain layer.
- **Interface Adapters / Infrastructure**: Implementation of interfaces, DB gateways, UI adapters.
- **Frameworks & Drivers / External**: UI frameworks, databases, external services; the “edge” of the system.

### Key Principles / Rules

- **Dependency Rule**: Source code dependencies only go inward. Nothing in an inner layer can know about or depend on something in an outer layer. :contentReference[oaicite:4]{index=4}
- **Independence of frameworks, UI, DB, external agencies**: The architecture should not be tightly coupled to them. :contentReference[oaicite:5]{index=5}
- The domain layer should be free of concerns regarding persistence, UI, frameworks. :contentReference[oaicite:6]{index=6}

### How this fits a C# project

- Your solution might be structured into separate projects/assemblies: `Core.Domain`, `Core.Application`, `Infrastructure`, `WebAPI` (or `UI`).
- Domain layer contains entities, value objects, domain services; references **no** other project.
- Application layer defines use-case interfaces, command/query handlers.
- Infrastructure implements interfaces (e.g., repository implementations) and depends on Application layer.
- UI or API layer depends on Application (and via DI, Infrastructure) but **not** directly on Domain.
- Interfaces → implementations follow Dependency Inversion Principle (one of SOLID).

---

## Domain-Driven Design (DDD)

![DDD Diagram 1](https://miro.medium.com/1*kGX7BupWVBMhxJwSTsSVKA.jpeg)  
![DDD Diagram 2](https://miro.medium.com/0*Dig5eOh00vMkqOiw.jpg)

### What is DDD?

Domain-Driven Design is an approach to software development that focuses on aligning the software model with the business domain, collaborating with domain experts, and building a rich domain model. :contentReference[oaicite:7]{index=7}  
It emphasises modelling the core domain, using a **ubiquitous language** shared between business/domain experts and developers. :contentReference[oaicite:8]{index=8}  
DDD is often divided into **strategic design** (how domains and contexts are arranged) and **tactical design** (building the model within a bounded context). :contentReference[oaicite:9]{index=9}

### Why use DDD?

- Helps ensure the software reflects business concepts and behaviour — not just data structures.
- Encourages modularity via bounded contexts — reducing coupling and modelling complexity.
- Improves clarity of domain logic (entities, value objects, aggregates) which leads to better maintainability and evolution.

### Key Concepts (Tactical Patterns)

- **Entity**: Has a unique identity and lifecycle; identity is what distinguishes this object, not just its attributes.
- **Value Object**: Immutable, has no identity; equality is based on value, not identity. E.g., Money, Address. :contentReference[oaicite:10]{index=10}
- **Aggregate & Aggregate Root**: An aggregate is a cluster of related entities and value objects treated as a single unit for purposes of data consistency. The aggregate root is the only object accessible from outside the aggregate. :contentReference[oaicite:11]{index=11}
- **Repository**: Provides methods to retrieve and persist aggregates, abstracting away the underlying data store.
- **Domain Service**: Encapsulates domain logic that doesn’t naturally fit within a single entity or value object.
- **Factory**: Responsible for creating complex aggregates in a valid state.
- **Domain Event**: Represents something that happened in the domain that domain experts care about — helps communicate across contexts.

### Strategic Patterns

- **Bounded Context**: A boundary within which a particular model applies; different contexts may have different models. :contentReference[oaicite:12]{index=12}
- **Ubiquitous Language**: The language used by domain experts and developers — model classes, methods, names should reflect it.
- **Context Map / Anti-Corruption Layer**: When integrating multiple bounded contexts or legacy systems, you define translation layers or contracts.

### How DDD fits into your C# Clean Architecture project

- Within your Domain (core) project you model your entities, value objects, aggregates — these are driven by business domain, **not** by persistence.
- Your Application layer (in Clean Architecture) uses these domain elements and defines use-cases.
- Infrastructure layer implements repositories for aggregates, and maps domain objects to persistence (DB, etc) — domain code doesn’t know about this.
- Strategic design: Each micro-service or module may map to a bounded context; within that context you apply the tactical patterns above.

---

## How Clean Architecture and DDD fit together

- Clean Architecture gives you the **structure** of your system, where layers go and how dependencies flow.
- DDD gives you the **modelling techniques** for the core domain: how to think about your domain, aggregates, value objects, entities.
- As noted: “Clean Architecture is about how you structure your source code on a technical level. DDD tells you how to deal with the domain part of your application.” :contentReference[oaicite:13]{index=13}
- So when building your C# project: Use Clean Architecture to organise layers (Domain → Application → Infrastructure → UI) and use DDD inside the Domain layer to model business logic properly.

---

## Summary / Cheat Sheet

| Concept              | In Clean Architecture                     | In DDD                                          |
| -------------------- | ----------------------------------------- | ----------------------------------------------- |
| Core layer           | Domain / Entities (inner circle)          | Your rich domain model: Entities, Value Objects |
| Dependencies         | Always inward                             | Domain layer free of infrastructure             |
| Primary focus        | Separation of concerns, testability       | Aligning software model with business domain    |
| Domain modelling     | Doesn’t specify specifics of domain model | Provides tactical patterns: Aggregates, etc.    |
| Structure of system  | Layers/rings (Onion/Hexagonal)            | Bounded contexts, subdomains                    |
| Implementation in C# | Projects/modules for each layer           | Domain project uses DDD patterns                |

---

## Recommended Folder/Project Structure (for your C# repo)

/src
/MyProject.Domain ← Domain layer: entities, value objects, domain services, events
/MyProject.Application ← Application layer: use-cases (commands/queries), interfaces
/MyProject.Infrastructure← Infrastructure layer: repository implementations, DB context, external integrations
/MyProject.WebApi ← Presentation/API layer: controllers, DTOs, wiring DI
/tests
/MyProject.Domain.Tests
/MyProject.Application.Tests
/MyProject.Infrastructure.Tests

pgsql
Copy code

- Domain project has **no dependencies** on other projects.
- Application depends on Domain.
- Infrastructure depends on Application (and thus Domain).
- WebApi depends on Application & Infrastructure.
- Keep domain types (entities/value objects) free of persistence attributes if possible (or use mapping instead).
- Use interfaces in Application layer and implementations in Infrastructure layer (Dependency Inversion).

---

## Tips & Best Practices

- Model your domain first (DDD) – understand the business rules, invariants, entities, value objects.
- Then impose your architecture (Clean Architecture) – decide projects/layers and dependencies.
- Keep domain logic **pure**: no UI/persistence concerns inside Domain layer.
- Use dependency injection to wire implementations at outer layers.
- Make your domain objects behaviour-rich (not just data holders) – DDD encourages behaviour.
- Enforce invariants inside aggregates: the aggregate root should guard consistency.
- Write unit tests for your domain logic (Domain layer) and integration tests for outer layers.
- Keep bounded contexts in mind if your domain grows large — multiple contexts may each follow the same architecture/patterns.
- Don’t let the “infrastructure” leak into your domain (e.g., avoid having persistence logic inside entities).

---
