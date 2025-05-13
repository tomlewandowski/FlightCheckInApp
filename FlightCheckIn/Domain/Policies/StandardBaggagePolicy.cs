namespace FlightCheckIn.Domain.Policies;

public sealed class StandardBaggagePolicy : IBaggagePolicy
{
    private static readonly Weight _maxTotal = Weight.FromKilograms(50);

    public void EnsureCanAdd(Flight flight, Passenger passenger, BaggageItem bag)
    {
        if (passenger.TotalBagsWeight + bag.Weight > _maxTotal)
            throw new BaggageLimitExceededException(passenger.TotalBagsWeight + bag.Weight, _maxTotal);
    }
}
