namespace FlightCheckIn.Domain;

public interface IDateTimeProvider { DateTime UtcNow { get; } }
public sealed class SystemDateTimeProvider : IDateTimeProvider { public DateTime UtcNow => DateTime.UtcNow; }
