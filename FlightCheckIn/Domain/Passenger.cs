namespace FlightCheckIn.Domain;

public sealed class Passenger(PassengerId id)
{
    private readonly List<BaggageItem> _bags = [];
    public PassengerId Id { get; } = id;
    public string? SeatNumber { get; private set; }
    public DateTime? CheckedInAt { get; private set; }

    public IReadOnlyList<BaggageItem> Bags => _bags;
    public Weight TotalBagsWeight => _bags.Aggregate(Weight.FromKilograms(0), (sum, next) => sum + next.Weight);

    public void CheckIn(DateTime at, string seat)
    {
        CheckedInAt = at;
        SeatNumber = seat;
    }

    public void AddBag(BaggageItem bag)
    {
        if (_bags.Any(b => b == bag)) // idempotency
        {
            return;
        }

        if (CheckedInAt == null)
        {
            throw new PassengerNotCheckedInException(Id);
        }

        _bags.Add(bag);
    }
}
