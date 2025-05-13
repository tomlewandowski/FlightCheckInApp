namespace FlightCheckIn.Domain;

public abstract class AppException(string message) : Exception(message);

public abstract class NotFoundException(string message) : AppException(message);

public abstract class ConflictException(string message) : AppException(message);

public abstract class ValidationException(string message) : AppException(message);

public class FlightNotFoundException(FlightId id)
    : NotFoundException($"Flight {id.Value} not found.");

public class PassengerNotFoundException(PassengerId id)
    : NotFoundException($"Passenger {id.Value} not found.");

public class OverbookingException(FlightId id)
    : ConflictException($"Flight {id.Value} capacity reached.");

public class ConcurrencyException(FlightId id)
    : ConflictException($"Concurrency conflict on Flight {id.Value}.");

public class BaggageLimitExceededException(Weight attempted, Weight limit)
    : ValidationException($"Baggage limit exceeded: {attempted} > {limit}");

public class PassengerNotCheckedInException(PassengerId id)
    : ValidationException($"Passenger {id.Value} must be checked in before adding bags.");
