namespace FlightCheckIn.Domain;
using System.Globalization;

public readonly record struct FlightId(Guid Value)
{
    public static FlightId New() => new(Guid.NewGuid());
};

public readonly record struct PassengerId(Guid Value)
{
    public static PassengerId New() => new(Guid.NewGuid());
};

public readonly record struct BaggageTag(string Code);

public readonly record struct Weight : IComparable<Weight>
{
    private const double PoundsToKilograms = 0.45359237;

    public double Kilograms { get; }
    public double Pounds => Kilograms * 2.20462262;

    public int CompareTo(Weight other) => Kilograms.CompareTo(other.Kilograms);

    public static readonly Weight Zero = new(0);

    private Weight(double kg)
    {
        if (kg < 0) throw new ArgumentOutOfRangeException(nameof(kg));
        Kilograms = kg;
    }

    public static Weight FromKilograms(double kg)
    {
        if (kg < 0) throw new ArgumentOutOfRangeException(nameof(kg));
        return new Weight(kg);
    }
    public static Weight FromPounds(double lb)
    {
        if (lb < 0) throw new ArgumentOutOfRangeException(nameof(lb));
        return new Weight(lb / 2.20462262);
    }

    public static Weight operator +(Weight a, Weight b) => new(a.Kilograms + b.Kilograms);
    public static Weight operator -(Weight a, Weight b) => new(a.Kilograms - b.Kilograms);
    public static bool operator >(Weight a, Weight b) => a.Kilograms > b.Kilograms;
    public static bool operator >=(Weight a, Weight b) => a.Kilograms >= b.Kilograms;
    public static bool operator <(Weight a, Weight b) => a.Kilograms < b.Kilograms;
    public static bool operator <=(Weight a, Weight b) => a.Kilograms <= b.Kilograms;
    public override string ToString() => $"{Kilograms.ToString("N1", CultureInfo.InvariantCulture)} kg";
}
