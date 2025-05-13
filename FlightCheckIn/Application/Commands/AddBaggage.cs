using FlightCheckIn.Domain;
using FlightCheckIn.Domain.Policies;
using MediatR;

namespace FlightCheckIn.Application.Commands;

public record AddBaggageCommand(Guid FlightId, Guid PassengerId, double WeightKg) : IRequest;

public sealed class AddBaggageCommandHandler(IFlightRepository repository, IBaggagePolicy policy) : IRequestHandler<AddBaggageCommand>
{
    public async Task Handle(AddBaggageCommand cmd, CancellationToken ct)
    {
        var flightId = new FlightId(cmd.FlightId);
        var passengerId = new PassengerId(cmd.PassengerId);
        var flight = await repository.GetAsync(flightId, ct);
        var bag = BaggageItem.Create(new BaggageTag(Guid.NewGuid().ToString()), Weight.FromKilograms(cmd.WeightKg));
        flight.AddBaggage(passengerId, bag, policy);
        await repository.SaveAsync(flight, ct);
    }
}
