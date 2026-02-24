# API Contracts and Mapping

## DTO Structure

DTOs are split into two folders:

- `InventoryManagementSystem.Api/DTOs/Requests`
- `InventoryManagementSystem.Api/DTOs/Responses`

Naming convention:

- `CreateXRequest`
- `UpdateXRequest`
- `XResponse`

Controllers must not accept or return EF entities. All requests and responses use DTOs.

## Mapping Decision

Manual mapping is the standard approach.

Mapping boundary:

- Mapping code lives in `InventoryManagementSystem.Api/Mapping`.
- Controllers should not contain mapping logic.
- Services will use the mapping layer to translate between DTOs and entities.

