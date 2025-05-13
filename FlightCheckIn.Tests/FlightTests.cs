using System;
using System.Collections.Generic;
using System.Linq;
using FlightCheckIn.Domain;
using FlightCheckIn.Domain.Policies;
using FluentAssertions;
using Xunit;

namespace FlightCheckIn.Tests;

public class FlightTests
{
    private readonly FlightId _flightId = FlightId.New();
    private readonly PassengerId _passengerId = PassengerId.New();

    [Fact]
    public void RegisterPassengers_ShouldAddDistinctPassengers()
    {
        // Arrange
        var flight = new Flight(_flightId, 3);
        var ids = new List<PassengerId> {_passengerId, PassengerId.New(),};

        // Act
        flight.RegisterPassengers(ids);

        // Assert
        flight.Passengers.Select(p => p.Id)
            .Should()
            .BeEquivalentTo(ids, options => options.WithStrictOrdering());
    }

    [Fact]
    public void RegisterPassengers_ShouldIgnoreDuplicateIds()
    {
        // Arrange
        var flight = new Flight(_flightId, 2);
        var duplicateId = PassengerId.New();
        var ids = new List<PassengerId> {duplicateId, duplicateId,};

        // Act
        flight.RegisterPassengers(ids);

        // Assert
        flight.Passengers.Should()
            .HaveCount(1)
            .And.ContainSingle(p => p.Id == duplicateId);
    }

    [Fact]
    public void RegisterPassengers_ShouldThrowOverbookingException_WhenCapacityExceeded()
    {
        // Arrange
        var flight = new Flight(_flightId, 1);
        var ids = new List<PassengerId> {PassengerId.New(), PassengerId.New(),};

        // Act
        var act = () => flight.RegisterPassengers(ids);

        // Assert
        act.Should()
            .Throw<OverbookingException>();
    }

    [Fact]
    public void CheckInPassenger_ShouldSetCheckedInAtAndSeatNumber()
    {
        // Arrange
        var flight = new Flight(_flightId, 1);
        flight.RegisterPassengers(new List<PassengerId> {_passengerId,});
        var expectedTime = new DateTime(2025, 01, 01, 12, 00, 00, DateTimeKind.Utc);
        var clock = new FakeDateTimeProvider {UtcNow = expectedTime,};

        // Act
        flight.CheckInPassenger(_passengerId, clock);

        // Assert
        var passenger = flight.Passengers.Single(p => p.Id == _passengerId);
        passenger.CheckedInAt.Should().Be(expectedTime);
        // Seat number is computed as number of already checked-in passengers
        var expectedSeat = flight.Passengers.Count(p => p.CheckedInAt != null).ToString();
        passenger.SeatNumber.Should().Be(expectedSeat);
    }

    [Fact]
    public void CheckInPassenger_ShouldThrowPassengerNotFoundException_WhenNotRegistered()
    {
        // Arrange
        var flight = new Flight(_flightId, 1);
        var clock = new FakeDateTimeProvider {UtcNow = DateTime.UtcNow,};

        // Act
        var act = () => flight.CheckInPassenger(_passengerId, clock);

        // Assert
        act.Should()
            .Throw<PassengerNotFoundException>();
    }

    [Fact]
    public void AddBaggage_ShouldAddBag_WhenWithinLimit()
    {
        // Arrange
        var flight = new Flight(_flightId, 1);
        flight.RegisterPassengers(new List<PassengerId> {_passengerId,});
        var clock = new FakeDateTimeProvider {UtcNow = DateTime.UtcNow,};
        flight.CheckInPassenger(_passengerId, clock);
        var bag = BaggageItem.Create(new BaggageTag("T1"), Weight.FromKilograms(20));
        var policy = new StandardBaggagePolicy();

        // Act
        flight.AddBaggage(_passengerId, bag, policy);

        // Assert
        var passenger = flight.Passengers.Single(p => p.Id == _passengerId);
        passenger.Bags.Should().ContainSingle().Which.Should().Be(bag);
    }

    [Fact]
    public void AddBaggage_ShouldThrowPassengerNotFoundException_WhenNotRegistered()
    {
        // Arrange
        var flight = new Flight(_flightId, 1);
        var bag = BaggageItem.Create(new BaggageTag("T1"), Weight.FromKilograms(10));
        var policy = new StandardBaggagePolicy();

        // Act
        var act = () => flight.AddBaggage(_passengerId, bag, policy);

        // Assert
        act.Should()
            .Throw<PassengerNotFoundException>();
    }

    [Fact]
    public void AddBaggage_ShouldThrowPassengerNotCheckedInException_WhenPassengerNotCheckedIn()
    {
        // Arrange
        var flight = new Flight(_flightId, 1);
        flight.RegisterPassengers(new List<PassengerId> {_passengerId,});
        var bag = BaggageItem.Create(new BaggageTag("T1"), Weight.FromKilograms(10));
        var policy = new StandardBaggagePolicy();

        // Act
        var act = () => flight.AddBaggage(_passengerId, bag, policy);

        // Assert
        act.Should()
            .Throw<PassengerNotCheckedInException>();
    }

    [Fact]
    public void AddBaggage_ShouldThrowBaggageLimitExceededException_WhenPolicyExceeded()
    {
        // Arrange
        var flight = new Flight(_flightId, 1);
        flight.RegisterPassengers(new List<PassengerId> {_passengerId,});
        var clock = new FakeDateTimeProvider {UtcNow = DateTime.UtcNow,};
        flight.CheckInPassenger(_passengerId, clock);
        var bag1 = BaggageItem.Create(new BaggageTag("T1"), Weight.FromKilograms(30));
        var bag2 = BaggageItem.Create(new BaggageTag("T2"), Weight.FromKilograms(30));
        var policy = new StandardBaggagePolicy();
        flight.AddBaggage(_passengerId, bag1, policy);

        // Act
        var act = () => flight.AddBaggage(_passengerId, bag2, policy);

        // Assert
        act.Should()
            .Throw<BaggageLimitExceededException>();
    }
}

// Fake date time provider for deterministic check-in tests
internal class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; set; }
}
