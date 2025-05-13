using FlightCheckIn.Domain;
using MediatR;

namespace FlightCheckIn.Application.Commands;

public record RegisterPassengersCommand(Guid FlightId, IReadOnlyList<Guid> PassengerIds) : IRequest;

public sealed class RegisterPassengersCommandHandler(IFlightRepository repository) : IRequestHandler<RegisterPassengersCommand>
{
    public async Task Handle(RegisterPassengersCommand cmd, CancellationToken ct)
    {
        var flightId = new FlightId(cmd.FlightId);
        var flight = await repository.GetAsync(flightId, ct);
        var passengerIds = cmd.PassengerIds.Select(id => new PassengerId(id)).ToList();
        flight.RegisterPassengers(passengerIds);
        await repository.SaveAsync(flight, ct);
    }
}
