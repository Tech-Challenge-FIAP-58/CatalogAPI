# Copilot Instructions

## Project Guidelines
- User provided mandatory DDD implementation rules: aggregate roots only have repositories, entities not shared between aggregates, communication via application/events with snapshots, do not alter existing entity properties, and add marker interface IAggregateRoot.
- Use pure names for entities without suffixes like 'Entity'.
- Do not update UpdatedAt in entities; it is set during SaveChanges.
- Catalog is not an entity; it is a context (part of the system), so avoid treating it as an entity/aggregate root.
- Never edit existing EF Core migration files directly. When entity changes require schema updates, always create a new migration using Add-Migration.
- Carts must have status tracking; checkout marks the cart as completed/finalized, and new purchases must use a new active cart (do not reuse completed carts).
- Each cart item must always have a quantity of 1; do not accept quantity from client input when adding a game to the cart.
- Before you develop new features, you should create unit tests for them and validate the existents one.
- You should create unit test every time that occurs changes in my business logical. Never let classes without unit tests.
- In DDD flows, aggregates must remain independent: do not pass full entities like Game between Cart/Order contexts; orchestrate in Application layer and pass only snapshot/simple data (e.g., GameId, Name, Price).
- OrderPlacedEvent must be published inside OrderService.Create during order creation, not via a separate OrderService method invoked by CartService.
- If any classes that you create or modify are missing unit tests, you should create them immediately to ensure code quality and coverage.
- If one class, service or any files is missing unit tests you should create them immediately to ensure code quality and coverage. Never let classes without unit tests.
## Language Standards
- Keep identifiers, messages, and documentation in English (avoid Portuguese).