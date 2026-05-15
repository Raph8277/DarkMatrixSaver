# Standards de code .NET

## Général

- C# latest.
- Async/await pour I/O.
- CancellationToken sur méthodes longues ou I/O.
- Nullable reference types activés.
- Exceptions métier explicites.

## Nommage

- Interfaces : `IReservationRepository`.
- Services applicatifs : `ReservationService` ou `ReserveBookHandler`.
- DTO gRPC : suffixe `Request`, `Response`, `Dto`.
- Tests : `Method_Context_ExpectedResult`.

## Interdits

- Logique métier dans les contrôleurs, services gRPC ou composants Razor.
- Accès direct DbContext depuis UI.
- Entités Domain exposées dans gRPC.
