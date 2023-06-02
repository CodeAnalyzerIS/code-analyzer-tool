# Stage 1: Build the React app
FROM node:18-alpine AS build-react
WORKDIR /app

# Copy and install React app dependencies
COPY CodeAnalyzerService.UI/code-analyzer-service.ui/package*.json ./CodeAnalyzerService.UI/
RUN cd CodeAnalyzerService.UI && npm ci

# Copy and build React app source code
COPY CodeAnalyzerService.UI/code-analyzer-service.ui/ ./CodeAnalyzerService.UI/
RUN cd CodeAnalyzerService.UI && npm run build

# Stage 2: Build the .NET Web API
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ./CodeAnalyzerService.Backend .
COPY --from=build-react /app/CodeAnalyzerService.UI/build ./wwwroot/
RUN dotnet restore
RUN dotnet build -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Combine the .NET Web API and React app into a single image
FROM base AS final
WORKDIR /app

# Copy the built .NET Web API
COPY --from=publish /app/publish .

# Set the entry point to start the .NET Web API
ENTRYPOINT ["dotnet", "CodeAnalyzerService.Backend.dll"]