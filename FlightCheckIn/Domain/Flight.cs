using FlightCheckIn.Domain.Policies;

namespace FlightCheckIn.Domain;

public sealed class Flight(FlightId id, int capacity)
{
    private readonly List<Passenger> _passengers = [];

    public FlightId Id { get; } = id;
    public int Capacity { get; } = capacity;

    public IReadOnlyList<Passenger> Passengers => _passengers;

    public void RegisterPassengers(List<PassengerId> passengerIds)
    {
        foreach (var passengerId in passengerIds)
        {
            RegisterPassenger(passengerId);
        }
    }

    private void RegisterPassenger(PassengerId passengerId)
    {
        if (_passengers.Any(p => p.Id == passengerId)) return; // idempotency
        if (_passengers.Count >= Capacity) throw new OverbookingException(Id);

        var passenger = new Passenger(passengerId);
        _passengers.Add(passenger);
    }

    public void CheckInPassenger(PassengerId passengerId, IDateTimeProvider clock)
    {
        var passenger = _passengers.SingleOrDefault(p => p.Id == passengerId) ?? throw new PassengerNotFoundException(passengerId);
        var nextSeatNumber = _passengers.Count(p => p.CheckedInAt != null) + 1;
        var seat = nextSeatNumber.ToString();
        passenger.CheckIn(clock.UtcNow, seat);
    }

    public void AddBaggage(PassengerId passengerId, BaggageItem bag, IBaggagePolicy policy)
    {
        var passenger = _passengers.SingleOrDefault(p => p.Id == passengerId) ?? throw new PassengerNotFoundException(passengerId);
        policy.EnsureCanAdd(this, passenger, bag);
        passenger.AddBag(bag);
    }
}
