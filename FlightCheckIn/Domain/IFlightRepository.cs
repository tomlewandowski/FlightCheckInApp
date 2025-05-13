namespace FlightCheckIn.Domain;

public interface IFlightRepository
{
    Task<Flight> GetAsync(FlightId id, CancellationToken ct = default);
    Task SaveAsync(Flight flight, CancellationToken ct = default);
}
