using FlightCheckIn.Application.Commands;
using MediatR;

namespace FlightCheckIn.Api;

public static class FlightEndpoints
{
    private const string ApiBase = "/api/flights";

    public static void MapFlightEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup(ApiBase).WithTags("Flights");

        group.MapPut("/{flightId:guid}/checkin/{passengerId:guid}", async (Guid flightId, Guid passengerId, IMediator mediator, CancellationToken ct) =>
            {
                var cmd = new CheckInPassengerCommand(flightId, passengerId);
                await mediator.Send(cmd, ct);
                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{flightId:guid}/checkin/{passengerId:guid}/baggage", async (Guid flightId, Guid passengerId, AddBagRequest body, IMediator mediator, CancellationToken ct) =>
            {
                var cmd = new AddBaggageCommand(flightId, passengerId, body.WeightKg);
                await mediator.Send(cmd, ct);
                return Results.Created($"{ApiBase}/{flightId}/checkin/{passengerId}/baggage", null);
            })
            .Accepts<AddBagRequest>("application/json")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{flightId:guid}/passengers", async (Guid flightId, RegisterPassengersRequest body, IMediator mediator, CancellationToken ct) =>
            {
                var cmd = new RegisterPassengersCommand(flightId, body.PassengerIds);
                await mediator.Send(cmd, ct);
                return Results.Created($"{ApiBase}/{flightId}/passengers", null);
            })
            .Accepts<RegisterPassengersRequest>("application/json")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public record AddBagRequest(double WeightKg);

    public record RegisterPassengersRequest(IReadOnlyList<Guid> PassengerIds);
}
