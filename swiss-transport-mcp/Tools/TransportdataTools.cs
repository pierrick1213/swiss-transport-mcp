using System.ComponentModel;
using System.Text.Json;
using System.Web;
using ModelContextProtocol.Server;

internal class TransportdataTools
{
    private readonly HttpClient _httpClient;

    public TransportdataTools(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [McpServerTool]
    [Description("Returns the matching locations for the given parameters. Either query or (x and y) are required.")]
    public async Task<string> GetLocations(
        [Description("Specifies the location name to search for (e.g., 'Basel')")] string? query = null,
        [Description("Latitude (e.g., 47.476001)")] double? x = null,
        [Description("Longitude (e.g., 8.306130)")] double? y = null,
        [Description("Specifies the location type: 'all' (default), 'station', 'poi', or 'address'")] string? type = null)
    {
        if (string.IsNullOrEmpty(query) && (!x.HasValue || !y.HasValue))
            return "Error: You must provide either a 'query' or both 'x' and 'y' coordinates.";

        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrEmpty(query)) queryParams["query"] = query;
        if (x.HasValue) queryParams["x"] = x.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (y.HasValue) queryParams["y"] = y.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (!string.IsNullOrEmpty(type)) queryParams["type"] = type;

        var (locationData, error) = await _httpClient.GetFromJsonSafeAsync<LocationResponse>($"locations?{queryParams}");

        if (error != null) return error;
        if (locationData?.Stations == null || !locationData.Stations.Any()) return "No locations found.";

        var cleanStations = locationData.Stations
        .Where(s => !string.IsNullOrEmpty(s.Id))
        .Select(s => new
        {
            s.Id,
            s.Name,
            Latitude = s.Coordinate?.X,
            Longitude = s.Coordinate?.Y,
            s.Distance,
            Type = s.Icon
        })
        .Take(10)
        .ToList();

        return JsonSerializer.Serialize(cleanStations);
        
    }

    [McpServerTool]
    [Description("Returns the next connections from a location to another.")]
    public async Task<string> GetConnections(
        [Description("Specifies the departure location of the connection (e.g., 'Lausanne')")] string from,
        [Description("Specifies the arrival location of the connection (e.g., 'Genève')")] string to,
        [Description("Specifies up to five via locations.")] string[]? via = null,
        [Description("Transportation means; one or more of train, tram, ship, bus, cableway.")] string[]? transportations = null,
        [Description("Date of the connection, in the format YYYY-MM-DD")] string? date = null,
        [Description("Time of the connection, in the format hh:mm")] string? time = null,
        [Description("Defaults to 0, if set to 1 the passed date and time is the arrival time")] int? isArrivalTime = null,
        [Description("1 - 16. Specifies the number of connections to return.")] int? limit = null,
        [Description("0 - 3. Allows pagination of connections.")] int? page = null)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams["from"] = from;
        queryParams["to"] = to;

        if (!string.IsNullOrEmpty(date)) queryParams["date"] = date;
        if (!string.IsNullOrEmpty(time)) queryParams["time"] = time;
        if (isArrivalTime.HasValue) queryParams["isArrivalTime"] = isArrivalTime.Value.ToString();
        if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
        if (page.HasValue) queryParams["page"] = page.Value.ToString();

        if (via != null && via.Any())
        {
            // On limite à 5 éléments maximum comme le demande l'API
            foreach (var v in via.Take(5))
            {
                queryParams.Add("via[]", v);
            }
        }

        if (transportations != null && transportations.Any())
        {
            foreach (var transport in transportations)
            {
                queryParams.Add("transportations[]", transport);
            }
        }

        var (connectionData, error) = await _httpClient.GetFromJsonSafeAsync<ConnectionsResponse>($"connections?{queryParams}");
        if (error != null) return error;
        if (connectionData?.Connections == null || !connectionData.Connections.Any()) return "No connections found.";

        var cleanConnections = connectionData.Connections.Select(c => new
        {
            DepartureTime = c.From?.Departure,
            ArrivalTime = c.To?.Arrival,
            Duration = c.Duration?.Replace("00d", ""),
            c.Transfers,

            Steps = c.Sections?.Select(s => new
            {
                Type = s.Walk != null ? "Walk" : $"{s.Journey?.Category} {s.Journey?.Number}".Trim(),

                From = s.Departure?.Station?.Name,
                To = s.Arrival?.Station?.Name,

                DepartureTime = s.Journey != null ? s.Departure?.Departure : null,
                Platform = s.Journey != null ? s.Departure?.Platform : null,

                DurationMinutes = s.Walk?.Duration.HasValue == true ? s.Walk.Duration.Value / 60 : (int?)null
            })
        });

        return JsonSerializer.Serialize(cleanConnections);
    }

    [McpServerTool]
    [Description("Returns the next connections leaving from a specific location.")]
    public async Task<string> GetStationboard(
        [Description("Specifies the location of which a stationboard should be returned (e.g., 'Aarau')")] string? station = null,
        [Description("The id of the station. Alternative to 'station' parameter; one of these two is required.")] string? id = null,
        [Description("Number of departing connections to return.")] int? limit = null,
        [Description("Transportation means; one or more of: train, tram, ship, bus, cableway.")] string[]? transportations = null,
        [Description("Date and time of departing connections, in the format YYYY-MM-DD hh:mm")] string? datetime = null,
        [Description("'departure' (default) or 'arrival'")] string? type = null)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrEmpty(station)) queryParams["station"] = station;
        if (!string.IsNullOrEmpty(id)) queryParams["id"] = id;
        if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
        if (!string.IsNullOrEmpty(datetime)) queryParams["datetime"] = datetime;
        if (!string.IsNullOrEmpty(type)) queryParams["type"] = type;

        if (transportations != null && transportations.Any())
        {
            foreach (var transport in transportations)
            {
                queryParams.Add("transportations[]", transport);
            }
        }

        var (StationboardData, error) = await _httpClient.GetFromJsonSafeAsync<StationboardResponse>($"stationboard?{queryParams}");

        if (error != null) return error;
        if (StationboardData?.Stationboard == null || !StationboardData.Stationboard.Any()) return "No stationboard found.";

        var cleanStationboard = StationboardData.Stationboard
        .Select(s => new
        {
            Destination = s.To,
            Line = $"{s.Category} {s.Number}".Trim(),
            DepartureTime = s.Stop?.Departure,
            Platform = s.Stop?.Platform ?? s.Stop?.Prognosis?.Platform,
            DelayMinutes = s.Stop?.Delay ?? 0
        })
        .ToList();

        return JsonSerializer.Serialize(cleanStationboard);
    }
}
