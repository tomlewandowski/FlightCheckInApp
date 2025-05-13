namespace FlightCheckIn.Domain;

public sealed class BaggageItem
{
    private static readonly Weight _maxSingleBag = Weight.FromKilograms(32);
    public BaggageTag Tag { get; }
    public Weight Weight { get; }

    private BaggageItem(BaggageTag tag, Weight weight)
    {
        Tag = tag;
        Weight = weight;
    }

    public static BaggageItem Create(BaggageTag tag, Weight weight)
    {
        if (weight > _maxSingleBag)
            throw new ArgumentOutOfRangeException(nameof(weight), $"Single bag cannot exceed {_maxSingleBag}");
        return new BaggageItem(tag, weight);
    }
}
