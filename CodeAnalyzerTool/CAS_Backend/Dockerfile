#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CAS_Backend/CAS_Backend.csproj", "CAS_Backend/"]
RUN dotnet restore "CAS_Backend/CAS_Backend.csproj"
COPY . .
WORKDIR "/src/CAS_Backend"
RUN dotnet build "CAS_Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CAS_Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CAS_Backend.dll"]