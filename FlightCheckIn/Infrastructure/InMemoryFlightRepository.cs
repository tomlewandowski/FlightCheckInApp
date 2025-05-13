using FlightCheckIn.Domain;
namespace FlightCheckIn.Infrastructure;

public sealed class InMemoryFlightRepository : IFlightRepository
{
    private readonly Dictionary<FlightId, Flight> _store = new();

    public InMemoryFlightRepository()
    {
        var id = new FlightId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        _store[id] = new Flight(id, 3);
    }

    public Task<Flight> GetAsync(FlightId id, CancellationToken ct = default)
    {
        if (!_store.TryGetValue(id, out var flight))
        {
            throw new FlightNotFoundException(id);
        }
        
        return Task.FromResult(flight);
    }

    public Task SaveAsync(Flight flight, CancellationToken ct = default)
    {
        _store[flight.Id] = flight; return Task.CompletedTask;
    }
}
