# Base image used to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build the application using the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["InternalRequestSystem.csproj", "./"]
RUN dotnet restore "InternalRequestSystem.csproj"

# Copy the remaining source code and publish the application
COPY . .
RUN dotnet publish "InternalRequestSystem.csproj" -c Release -o /app/publish

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InternalRequestSystem.dll"]