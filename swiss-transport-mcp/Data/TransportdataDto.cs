using System.Text.Json.Serialization;
public record LocationResponse([property: JsonPropertyName("stations")] List<Station> Stations);

public record Station(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("coordinate")] Coordinate? Coordinate,
    [property: JsonPropertyName("distance")] double? Distance,
    [property: JsonPropertyName("icon")] string? Icon
);

public record Coordinate(
    [property: JsonPropertyName("x")] double? X,
    [property: JsonPropertyName("y")] double? Y
);

public record StationboardResponse([property: JsonPropertyName("station")] Station Stations, [property: JsonPropertyName("stationboard")] List<Stationboard> Stationboard);

public record Stationboard(
    [property: JsonPropertyName("stop")] Stop? Stop,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("category")] string? Category,
    [property: JsonPropertyName("subcategory")] string? Subcategory,
    [property: JsonPropertyName("categoryCode")] string? CategoryCode,
    [property: JsonPropertyName("number")] string? Number,
    [property: JsonPropertyName("operator")] string? Operator,
    [property: JsonPropertyName("to")] string? To,
    [property: JsonPropertyName("passList")] List<Stop>? PassList,
    [property: JsonPropertyName("capacity1st")] int? Capacity1st,
    [property: JsonPropertyName("capacity2nd")] int? Capacity2nd
);

public record Stop(
    [property: JsonPropertyName("station")] Station? Station,
    [property: JsonPropertyName("arrival")] string? Arrival,
    [property: JsonPropertyName("arrivalTimestamp")] long? ArrivalTimestamp,
    [property: JsonPropertyName("departure")] string? Departure,
    [property: JsonPropertyName("departureTimestamp")] long? DepartureTimestamp,
    [property: JsonPropertyName("delay")] int? Delay,
    [property: JsonPropertyName("platform")] string? Platform,
    [property: JsonPropertyName("prognosis")] Prognosis? Prognosis,
    [property: JsonPropertyName("realtimeAvailability")] string? RealtimeAvailability,
    [property: JsonPropertyName("location")] Station? Location
);

public record Prognosis(
    [property: JsonPropertyName("platform")] string? Platform,
    [property: JsonPropertyName("arrival")] string? Arrival,
    [property: JsonPropertyName("departure")] string? Departure,
    [property: JsonPropertyName("capacity1st")] int? Capacity1st,
    [property: JsonPropertyName("capacity2nd")] int? Capacity2nd
);


public record ConnectionsResponse([property: JsonPropertyName("connections")] List<Connection>? Connections);

public record Connection(
    [property: JsonPropertyName("from")] ConnectionPoint? From,
    [property: JsonPropertyName("to")] ConnectionPoint? To,
    [property: JsonPropertyName("duration")] string? Duration,
    [property: JsonPropertyName("transfers")] int? Transfers,
    [property: JsonPropertyName("products")] List<string>? Products,
    [property: JsonPropertyName("sections")] List<Section>? Sections
);

public record ConnectionPoint(
    [property: JsonPropertyName("station")] Station? Station,
    [property: JsonPropertyName("arrival")] string? Arrival,
    [property: JsonPropertyName("departure")] string? Departure,
    [property: JsonPropertyName("delay")] int? Delay,
    [property: JsonPropertyName("platform")] string? Platform
);

public record Section(
    [property: JsonPropertyName("journey")] Journey? Journey,
    [property: JsonPropertyName("walk")] Walk? Walk,
    [property: JsonPropertyName("departure")] ConnectionPoint? Departure,
    [property: JsonPropertyName("arrival")] ConnectionPoint? Arrival
);

public record Journey(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("category")] string? Category, 
    [property: JsonPropertyName("number")] string? Number,    
    [property: JsonPropertyName("operator")] string? Operator, 
    [property: JsonPropertyName("to")] string? To
);

public record Walk(
    [property: JsonPropertyName("duration")] int? Duration // Durée de la marche
);