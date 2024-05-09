# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY WorkoutHelper-Docker.sln .
COPY WorkoutHelper.Shared/WorkoutHelper.Shared.csproj ./WorkoutHelper.Shared/
COPY WorkoutHelperAPI/WorkoutHelperAPI.csproj ./WorkoutHelperAPI/
RUN dotnet restore

# Copy the rest of the source code and build the API project
COPY . .
WORKDIR /src/WorkoutHelperAPI
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port your application will run on
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "WorkoutHelperAPI.dll"]