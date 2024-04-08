#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GamePluggable/Tgs.GamePluggable.csproj", "Tgs.GamePluggable/"]
COPY ["Tgs.GamePluggable.Core/Tgs.GamePluggable.Core.csproj", "Tgs.GamePluggable.Core/"]
COPY ["Tgs.GamePluggable.Repositories/Tgs.GamePluggable.Repositories.csproj", "Tgs.GamePluggable.Repositories/"]
COPY ["Tgs.GameProviders.Pluggable.ViewModels/Tgs.GameProviders.Pluggable.ViewModels.csproj", "Tgs.GameProviders.Pluggable.ViewModels/"]
RUN dotnet restore "./Tgs.GamePluggable/./Tgs.GamePluggable.csproj"
COPY . .
WORKDIR "/src/Tgs.GamePluggable"
RUN dotnet build "./Tgs.GamePluggable.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GamePluggable.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GamePluggable.dll"]