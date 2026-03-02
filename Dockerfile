FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["swiss-transport-mcp/swiss-transport-mcp.csproj", "swiss-transport-mcp/"]
RUN dotnet restore "./swiss-transport-mcp/swiss-transport-mcp.csproj"
COPY . .
WORKDIR "/src/swiss-transport-mcp"
RUN dotnet build "./swiss-transport-mcp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./swiss-transport-mcp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "swiss-transport-mcp.dll"]
