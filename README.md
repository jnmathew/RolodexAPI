# Rolodex API

A RESTful contact management API built with ASP.NET Core, Entity Framework Core, and SQLite. Implements CRUD operations and domain-specific query endpoints for tracking personal contacts, upcoming birthdays, and stale relationships.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Scalar (API documentation)

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

### Setup

```bash
git clone https://github.com/jnmathew/RolodexAPI.git
cd RolodexAPI
dotnet ef database update --project RolodexAPI
dotnet run --project RolodexAPI --launch-profile https
```

The API will be available at `https://localhost:7295`. Browse the interactive API docs at `https://localhost:7295/scalar/v1`.

## API Endpoints

### CRUD

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/contacts` | List all contacts (optional filters: `firstName`, `lastName`, `type`) |
| GET | `/contacts/{id}` | Get a contact by ID |
| POST | `/contacts` | Create a new contact |
| PUT | `/contacts/{id}` | Update a contact |
| DELETE | `/contacts/{id}` | Delete a contact |

### Query

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/contacts/upcoming-birthdays?days=30` | Contacts with birthdays in the next N days |
| GET | `/contacts/stale?days=90` | Contacts not reached out to in N days |

## Data Model

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| Id | int | Auto | Primary key |
| FirstName | string | Yes | Contact's first name |
| LastName | string | Yes | Contact's last name |
| Email | string | No | Email address |
| PhoneNumber | string | No | Phone number |
| Type | enum | No | Personal, Professional, Family, Other |
| Address | string | No | Address |
| DateOfBirth | DateOnly | No | Date of birth |
| LastContactedDate | DateOnly | No | Last interaction date |
| Notes | string | No | Freeform notes |
| CreatedAtUtc | DateTime | Auto | Creation timestamp |
| UpdatedAtUtc | DateTime | Auto | Last update timestamp |

## Design Decisions

- **Separate query endpoints** for birthdays and stale contacts return specialized DTOs rather than overloading the main `/contacts` endpoint with query parameters. This keeps response schemas consistent per endpoint.
- **Contacts with null `LastContactedDate` are considered stale**, since an untracked interaction implies the relationship needs attention.
- **Feb 29 birthdays** are treated as Feb 28 in non-leap years.
- **PUT requires the full object**. A PATCH endpoint for partial updates is a potential future enhancement.

## Future Enhancements

- **PATCH endpoint** for partial contact updates (send only changed fields instead of the full object)
- **Unit tests** with xUnit for endpoint validation, birthday logic edge cases, and stale contact filtering
- **Pagination** on GET /contacts for large datasets
