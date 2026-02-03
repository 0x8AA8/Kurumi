# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY nhitomi.sln ./
COPY nhitomi/nhitomi.csproj nhitomi/
COPY nhitomi.Core/nhitomi.Core.csproj nhitomi.Core/
COPY nhitomi.Core.UnitTests/nhitomi.Core.UnitTests.csproj nhitomi.Core.UnitTests/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Publish project
RUN dotnet publish nhitomi -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy publish output
COPY --from=build /app/publish .

# Expose ports
EXPOSE 80

# Entrypoint
ENTRYPOINT ["dotnet", "nhitomi.dll"]
