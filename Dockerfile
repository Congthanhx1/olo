# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution and projects
COPY *.sln .
COPY src/DigitalStore.Domain/*.csproj src/DigitalStore.Domain/
COPY src/DigitalStore.Application/*.csproj src/DigitalStore.Application/
COPY src/DigitalStore.Infrastructure/*.csproj src/DigitalStore.Infrastructure/
COPY src/DigitalStore.API/*.csproj src/DigitalStore.API/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /source/src/DigitalStore.API
RUN dotnet publish -c Release -o /app

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Create SecureStorage directory
RUN mkdir -p SecureStorage

# Set environment variables for Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "DigitalStore.API.dll"]
