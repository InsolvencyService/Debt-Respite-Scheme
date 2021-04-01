#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Insolvency.Portal/Insolvency.Portal.csproj", "Insolvency.Portal/"]
COPY ["Insolvency.RestClient/Insolvency.RestClient.csproj", "Insolvency.RestClient/"]
COPY ["Insolvency.Interfaces/Insolvency.Interfaces.csproj", "Insolvency.Interfaces/"]
COPY ["Insolvency.Portal.Gateways/Insolvency.Portal.Gateways.csproj", "Insolvency.Portal.Gateways/"]
COPY ["Insolvency.Integration.Models/Insolvency.Integration.Models.csproj", "Insolvency.Integration.Models/"]
COPY ["Insolvency.Portal.Interfaces/Insolvency.Portal.Interfaces.csproj", "Insolvency.Portal.Interfaces/"]
RUN dotnet restore "Insolvency.Portal/Insolvency.Portal.csproj"
COPY . .
WORKDIR "/src/Insolvency.Portal"
RUN dotnet build "Insolvency.Portal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Insolvency.Portal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Insolvency.Portal.dll"]