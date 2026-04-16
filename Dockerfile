# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Hris.Demo.sln global.json ./
COPY src/Hris.Demo.Shared/ src/Hris.Demo.Shared/
COPY src/Hris.Demo.Api/ src/Hris.Demo.Api/

RUN dotnet restore src/Hris.Demo.Api/Hris.Demo.Api.csproj
RUN dotnet publish src/Hris.Demo.Api/Hris.Demo.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render (and many hosts) inject PORT at runtime; Program.cs binds http://0.0.0.0:{PORT} when set.
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Hris.Demo.Api.dll"]
