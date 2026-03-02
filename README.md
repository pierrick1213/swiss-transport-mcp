# Swiss Transport MCP Server 🇨🇭🚉

[![.NET CI](https://github.com/pierrick1213/swiss-transport-mcp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/pierrick1213/swiss-transport-mcp/actions/workflows/dotnet.yml)
[![Docker Image Version (latest by date)](https://img.shields.io/docker/v/pickcool/swiss-transport-mcp)](https://hub.docker.com/repository/docker/pickcool/swiss-transport-mcp)

A **Model Context Protocol (MCP)** server that provides real-time access to the Swiss public transport network. This server gives AI models (like Claude, ChatGPT, etc.) the ability to search for stations, query departures, and find route connections across Switzerland.

The data is powered by the excellent OpenData.ch API: [https://transport.opendata.ch/docs.html](https://transport.opendata.ch/docs.html)

## 🛠️ Features (Tools)

This MCP server exposes three main tools to your AI agent:

*   **`GetLocations`**: Search for locations, stations, points of interest (POI), or addresses (e.g., "Basel", "Lausanne"). Returns coordinates and distance.
*   **`GetStationboard`**: Get the next departing connections leaving from a specific station (e.g., "Aarau"). Shows destinations, lines, departure times, platforms, and current delays.
*   **`GetConnections`**: Find a journey from point A to point B (e.g., from "Genève" to "Zürich"). Returns the duration, number of transfers, and detailed steps for the journey.

## 🚀 How to use this MCP Server

You can run this server either via **Docker** (recommended) or locally using **.NET**.

### Option 1: Using Docker (Recommended)

The easiest way to integrate this server into your MCP client (like Claude Desktop) is to use the official Docker image available on Docker Hub:
👉 [https://hub.docker.com/repository/docker/pickcool/swiss-transport-mcp](https://hub.docker.com/repository/docker/pickcool/swiss-transport-mcp)

Add the following configuration to your MCP client config file (e.g., `claude_desktop_config.json`):

```json
{
  "mcpServers": {
    "swiss-transport": {
      "command": "docker",
      "args": [
        "run",
        "-i",
        "--rm",
        "pickcool/swiss-transport-mcp:latest"
      ]
    }
  }
}
```

### Option 2: Running locally with .NET

If you prefer to run it locally or want to contribute to the code, you need [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) installed.

#### 1. Test via HTTP locally
You can run the API normally to test the endpoints using the provided `.http` file:
```bash
git clone https://github.com/pierrick1213/swiss-transport-mcp.git
cd swiss-transport-mcp/swiss-transport-mcp
dotnet run
```
Once running, you can use the `swiss-transport-mcp.http` file in Visual Studio or VS Code to send test requests.

#### 2. Configure in your MCP Client
To configure your AI client to spawn the server directly from the source code, add this to your MCP configuration file:

```json
{
  "mcpServers": {
    "swiss-transport": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\absolute\\path\\to\\your\\swiss-transport-mcp\\swiss-transport-mcp\\swiss-transport-mcp.csproj"
      ]
    }
  }
}
```
*(Make sure to replace the path with your actual absolute path).*

## 🧠 How it works under the hood

This server is built using **ASP.NET Core 10.0** and the `ModelContextProtocol.AspNetCore` package. 
It operates over **stdio** (standard input/output), which is the standard communication method for MCP servers spawned by clients like Claude Desktop. 

When your AI model decides it needs train information, the MCP client sends a JSON-RPC request to this server via `stdin`. The server queries the `transport.opendata.ch` API, cleans and formats the JSON response specifically to be easily digestible by LLMs, and returns the result via `stdout`.

## 🤝 Contributing

Contributions are welcome! Feel free to open an issue or submit a Pull Request.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request
