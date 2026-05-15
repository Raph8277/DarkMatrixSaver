# Règles DDD

## Agrégats fil rouge

- `Livre`
- `Personne`
- `Reservation`

## Value Objects possibles

- `Email`
- `Adresse`
- `Isbn`
- `ReservationPeriod`

## Invariants exemples

- Une réservation doit concerner un livre existant.
- Une réservation doit être associée à une personne.
- Une réservation annulée ne peut pas être confirmée sans réouverture explicite.
- Un email doit être valide au moment de créer une personne.

## Anti-patterns

- Entités avec uniquement getters/setters.
- Règles métier dans EF Core.
- Domain dépendant de gRPC, Blazor ou SQL.
