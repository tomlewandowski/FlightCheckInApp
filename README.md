# FlightCheckIn API

## Overview

A minimal .NET 9 Web API for a basic flight check-in system. It allows passengers to check in and add baggage, enforcing seat capacity and baggage weight limits.

## Prerequisites

- .NET 9 SDK or later

## Setup & Run

```bash
# Clone the repository
git clone https://github.com/tomlewandowski/FlightCheckInApp.git
cd FlightCheckInApp

# Build and run the API
dotnet run --project FlightCheckIn/FlightCheckIn.csproj
```

By default, the API listens at:

- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

### Seeded Data

An in-memory repository seeds a default flight:

- Flight ID: `11111111-1111-1111-1111-111111111111`
- Capacity: **3** seats

You can use this ID to test the endpoints.

## Running Tests

```bash
dotnet test --project FlightCheckIn.Tests/FlightCheckIn.Tests.csproj
```

## API Endpoints

### 1. Check-in a Passenger

**PUT** `/api/flights/{flightId}/checkin/{passengerId}`

Responses:

- **204 No Content**
  - Location header: `/api/flights/{flightId}/checkin/{passengerId}`
- **404 Not Found**
  - Flight or passenger not found.
- **409 Conflict**
  - Flight capacity reached (overbooking).
- **422 Unprocessable Entity**
  - Validation errors.
- **400 Bad Request**
  - Other domain errors.

### 2. Add Baggage to a Passenger

**POST** `/api/flights/{flightId}/checkin/{passengerId}/baggage`

Request Body (application/json):

```json
{
  "weightKg": 10.5
}
```

Responses:

- **201 Created**
  - Location header: `/api/flights/{flightId}/checkin/{passengerId}/baggage`
- **404 Not Found**
  - Flight or passenger not found.
- **409 Conflict**
  - Concurrency conflict or capacity issue.
- **422 Unprocessable Entity**
  - Baggage weight limit exceeded.
- **400 Bad Request**
  - Other domain errors.

### Error Mapping

Exceptions are mapped to HTTP status codes returning `application/problem+json`:

| Exception                       | Status Code | Title            |
| ------------------------------- | ----------- | ---------------- |
| `FlightNotFoundException`       | 404         | Not found        |
| `PassengerNotFoundException`    | 404         | Not found        |
| `OverbookingException`          | 409         | Conflict         |
| `ConcurrencyException`          | 409         | Conflict         |
| `BaggageLimitExceededException` | 422         | Validation error |
| `ValidationException`           | 422         | Validation error |
| other `AppException`            | 400         | Domain error     |

## Business Rules

- Each flight has a fixed seat capacity.
- Maximum total baggage weight per passenger: **50 kg**.
- Maximum weight per individual bag: **32 kg**.
- Seats are assigned in check-in order (first-come, first-served).
- A passenger must be checked in before adding baggage.

## Project Structure

```
FlightCheckInApp.sln
FlightCheckIn/
├── Api/
│   ├── FlightEndpoints.cs
│   └── ExceptionMappingMiddleware.cs
├── Application/
│   └── Commands/
│       ├── CheckInPassenger.cs
│       ├── AddBaggage.cs
│       └── RegisterPassengers.cs
├── Domain/
│   ├── ValueObjects.cs
│   ├── Flight.cs
│   ├── Passenger.cs
│   ├── BaggageItem.cs
│   ├── Exceptions.cs
│   ├── IFlightRepository.cs
│   ├── IDateTimeProvider.cs
│   └── Policies/
│       ├── IBaggagePolicy.cs
│       └── StandardBaggagePolicy.cs
├── Infrastructure/
│   └── InMemoryFlightRepository.cs
├── Program.cs
└── FlightCheckIn.csproj
FlightCheckIn.Tests/
└── FlightTests.cs
```

## Assumptions

- In-memory storage; data is not persisted across restarts.
- No authentication or authorization.
- Separate booking and check-in contexts: passengers are imported into the check-in subdomain via the `RegisterPassengers` command, typically triggered when check-in opens, enforcing overbooking rules at registration time.
- `CheckInPassenger` operates only on already-registered passengers and does not add new passengers to flights.
- Seat assignment is based on check-in order (first-come, first-served) but it could be designed to be externalized via an `ISeatAssignmentPolicy`.
- Baggage rules are abstracted using `IBaggagePolicy` to support flexible policies per customer tier or external conditions (e.g., route, weather).

## Future Improvements

- Expose passenger registration and listing endpoints to better support client workflows.
- Implement a `CreateFlightCommand` to enable dynamic flight creation.
- Persist data using a relational database with optimistic locking to prevent concurrency conflicts.
- Integrate with an external booking system to asynchronously import booked passengers when check-in opens.
- Delegate seat assignment to an external `ISeatAssignmentPolicy` to support advanced seat selection and dynamic pricing.
- Support dynamic baggage policies via `IBaggagePolicy`, enabling different limits per customer tier, flight route, or environmental factors.
- Add update and delete operations for check-ins and baggage to improve lifecycle management.
- Implement authentication and authorization for secure access control.
- Dockerize the service for consistent deployment.
