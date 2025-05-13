namespace FlightCheckIn.Domain.Policies;

public interface IBaggagePolicy
{
    void EnsureCanAdd(Flight flight, Passenger passenger, BaggageItem bag);
}
