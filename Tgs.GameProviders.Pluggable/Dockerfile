#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.GameProviders.Pluggable/Tgs.GameProviders.Pluggable.csproj", "Tgs.GameProviders.Pluggable/"]
COPY ["Tgs.GameProviders.Pluggable.Core/Tgs.GameProviders.Pluggable.Core.csproj", "Tgs.GameProviders.Pluggable.Core/"]
COPY ["Tgs.GameProviders.Pluggable.ViewModels/Tgs.GameProviders.Pluggable.ViewModels.csproj", "Tgs.GameProviders.Pluggable.ViewModels/"]
RUN dotnet restore "Tgs.GameProviders.Pluggable/Tgs.GameProviders.Pluggable.csproj"
COPY . .
WORKDIR "/src/Tgs.GameProviders.Pluggable"
RUN dotnet build "Tgs.GameProviders.Pluggable.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameProviders.Pluggable.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameProviders.Pluggable.dll"]