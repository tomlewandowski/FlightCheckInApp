using FlightCheckIn.Domain;
using MediatR;

namespace FlightCheckIn.Application.Commands;

public record CheckInPassengerCommand(Guid FlightId, Guid PassengerId) : IRequest;

public sealed class CheckInPassengerCommandHandler(IFlightRepository repository, IDateTimeProvider clock) : IRequestHandler<CheckInPassengerCommand>
{
    public async Task Handle(CheckInPassengerCommand cmd, CancellationToken ct)
    {
        var flightId = new FlightId(cmd.FlightId);
        var passengerId = new PassengerId(cmd.PassengerId);
        var flight = await repository.GetAsync(flightId, ct);
        flight.CheckInPassenger(passengerId, clock);
        await repository.SaveAsync(flight, ct);
    }
}
