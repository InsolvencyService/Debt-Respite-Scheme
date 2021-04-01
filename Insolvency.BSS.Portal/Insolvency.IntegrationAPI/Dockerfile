#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Insolvency.IntegrationAPI/Insolvency.IntegrationAPI.csproj", "Insolvency.IntegrationAPI/"]
COPY ["Insolvency.Integration.Gateways/Insolvency.Integration.Gateways.csproj", "Insolvency.Integration.Gateways/"]
COPY ["Insolvency.Dynamics.Models/Insolvency.Dynamics.Models.csproj", "Insolvency.Dynamics.Models/"]
COPY ["Insolvency.Interfaces/Insolvency.Interfaces.csproj", "Insolvency.Interfaces/"]
COPY ["Insolvency.Integration.Interfaces/Insolvency.Integration.Interfaces.csproj", "Insolvency.Integration.Interfaces/"]
COPY ["Insolvency.RestClient/Insolvency.RestClient.csproj", "Insolvency.RestClient/"]
COPY ["Insolvency.Integration.Models/Insolvency.Integration.Models.csproj", "Insolvency.Integration.Models/"]
RUN dotnet restore "Insolvency.IntegrationAPI/Insolvency.IntegrationAPI.csproj"
COPY . .
WORKDIR "/src/Insolvency.IntegrationAPI"
RUN dotnet build "Insolvency.IntegrationAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Insolvency.IntegrationAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Insolvency.IntegrationAPI.dll"]